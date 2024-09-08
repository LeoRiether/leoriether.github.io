+++
title = "The Relational Model Had It All Figured Out From the Start"
+++

This is a post about the Zen of Python, system boundaries, software design, relational databases,
ECS, and how everything became connected when I realized the mistake I made at work.

The main character is a gRPC API, who we'll call Bob so as not to reveal real details. Bob
internally manages entities called `Tree`s. We don't want to complicate it right now, so every
`Tree` has exactly one `Branch`, and that's it. How does a user of the API retrieve existing
`Tree`s? Simple:

```proto

service Bob {
    rpc GetTree(GetTreeRequest) returns (GetTreeResponse);
    rpc GetAllTrees(GetAllTreesRequest) returns (GetAllTreesResponse);
}

message GetTreeRequest {
    int id = 1;
}

message GetTreeResponse {
    Tree tree = 1; 
}

message GetAllTreesRequest {}

message GetAllTreesResponse {
    repeated Tree trees = 1; 
}

message Tree {
    int id = 1;
    bool is_healthy = 2; 
    Branch main_branch = 3;
}

message Branch {
    int id = 1;
    int angle = 2;
}
```

Great! Let's write some others methods to create and update trees (which we
won't discuss here), implement everything and then publish the API so everyone
uses it!

We're storing trees on Postgres, the queries are really easy:

```sql
-- schema:
create table tree (
    id         serial primary key,
    is_healthy bool
);

create table branch (
    id      serial primary key,
    angle   int
    tree_id int    references tree (id), -- it's in this table for a good reason,
                                         -- which also won't be discussed
);

create unique index unique_branch_tree
    on branch (tree_id);

-- GetTree:
SELECT tree.*, branch.*
FROM tree
JOIN branch ON branch.tree_id = tree.id
WHERE tree.id = $1;
```

Now that we're done, there's a new requirement: every branch can now have multiple leaves. Because
of the big amount of leaves though, `GetAllTrees` should **not** return any leaves. Users of
`GetAllTrees` don't need to know about the leaves and it would waste too much network. `GetTree`,
however, should return the leaves. How do we change the gRPC interface to reflect that? Adding a
`repeated Leaf leaves = 3;` on the `Branch` message won't work because `GetAllTrees` uses it. I
guess we could wrap a `Branch` with another message that also includes leaves? How does that look?

```proto
// We need to add this message
message BranchWithLeaves {
    Branch branch = 1;
    repeated Leaf leaves = 2;
}

// And modify the response of GetTree...
message GetTreeResponse {
    Tree tree = 1;
    // ...
}
```

Wait, how do we modify `GetTree`? There's no way to put `BranchWithLeaves` as a field in `Tree`
because it would affect `GetAllTrees`! Scrap that idea... Maybe put the leaves directly in the
`GetTreeResponse`?

```proto
message GetTreeResponse {
    Tree tree = 1;
    repeated Leaf main_branch_leaves = 2; 
}
```

Ah, nice, that works! No breaking change needed, users are unaffected, job done.

But what's that, right around the corner? Another requirement!? Now a tree can have many branches.
Ok... `Tree` needs to change... But it's easy to make at least, the users can continue using
`main_branch` until they migrate to `branches`:

```proto
message Tree {
    int id = 1;
    bool is_healthy = 2; 
    Branch main_branch = 3 [deprecated = true]; // deprecate this field...
    repeated Branch branches = 4;               // ...and add this one
}
```

Now to update `GetTree`. Remember that the response looked like this:

```proto
message GetTreeResponse {
    Tree tree = 1;
    repeated Leaf main_branch_leaves = 2; 
}
```

The tree has a list of branches, and every branch has a list of leaves,
therefore we should add...

```proto
message GetTreeResponse {
    Tree tree = 1;
    repeated Leaf main_branch_leaves = 2 [deprecated = true];
    repeated LeafList branches_leaves = 3; // the m-th leaf of the n-th branch
                                           // is accessed by
                                           // branches_leaves[n][m]
}

message LeafList {
    repeated Leaf leaves = 1;
}
```

Yeah... That doesn't look like a pleasant API to use at all. Branches are nested within the Tree,
but leaves are outside (by necessity!) and iterating over this structure requires keeping track of
indices and is harder than it should be.

In fact, to add another "optional" nesting level -- like leaves that have cells but only in another
route -- it would be necessary to add a list of list of list of cells in the `GetTreeResponse`,
unless there's another approach that's not equally bad and I'm missing.

Where did I go wrong? Could I have prevented this? It was at this moment I realized... I literally
saw [this video](https://www.youtube.com/watch?v=o9pEzgHorH0) yesterday and in the first minute,
when the guy says "Flat is better than nested" when referring to a principle of the Zen of Python, I
thought I understood what he meant, but now, only now I truly understood! **The problem above only
arises because Branch is nested within the Tree message**! When making that decision, I implicitly
required that Tree contained not only the current Branch, but everything a Branch would ever
contain, today's **and tomorrow's fields**.

How can a flat structure help? Let's go back to the beginning, before those new requirements, the
times when a Tree could only have a single Branch and leaves didn't exist. Here's the alternative
design for the responses:

```proto
// Notice there's no `Branch main_branch` inside the Tree.
// The Branch message is unchanged.
message Tree {
    int id = 1;
    bool is_healthy = 2; 
    int main_branch_id = 3; // not really necessary in this case, but here anyway
}

message GetTreeResponse {
    Tree tree = 1;
    Branch main_branch = 2;
}

message GetAllTreesResponse {
    repeated TreeBranchPair tree_branch_pair = 1;
}

message TreeBranchPair {
    Tree tree = 1;
    Branch main_branch = 2;
}
```

The `TreeBranchPair` message is a bit ugly, sure, but I blame protobuf for this, capn'proto has a
cleaner way of defining it.

Is this any harder to use than our first design? Not really, trees and branches are still right next
to each other, easily iterable in `GetAllTreesResponse`, without adding any cognitive load. There's
__more code__, yes, but it's still simple.

Let's add the next requirement: a branch has multiple leaves, but `GetAllTrees` shouldn't include
them. Sure!

```proto
message GetTreeResponse {
    Tree tree = 1;
    Branch main_branch = 2;
    repeated Leaf leaves = 3; // <- added this
}
```

That's it! `GetAllTreesResponse` doesn't change at all. Could we nest `repeated Leaf` inside the
`Branch`? Sure we could, but we've seen how that can lead to problems down the line, and the current
structure seems perfectly fine, no? 

Great, let's try the final test: adding multiple branches per tree. There's no way to do this
without deprecating some things as far as I'm aware, but it's possible to update the responses like
this:

```proto
message GetTreeResponse {
    Tree tree = 1;
    Branch main_branch = 2 [deprecated = true];       // deprecated this...
    repeated Leaf leaves = 3 [deprecated = true];     // ...and this...
    repeated BranchLeavesPair branch_leaves_pair = 4; // ...and added this
}

message BranchLeavesPair {
    Branch branch = 1;
    repeated Leaves = 2;
}

message GetAllTreesResponse {
    repeated TreeBranchPair tree_branch_pair = 1 [deprecated = true]; // big deprecation here!
    repeated TreeBranchesPair tree_branches_pair = 2;                 // and added this
}

message TreeBranchesPair {
    Tree tree = 1;
    repeated Branch branches = 2;
}
```

We did it! The structure is a bit hard to visualize in protobuf, but this is the schema:

```haskell
GetTreeResponse {
    Tree,
    list of {
        Branch,
        list of Leaves
    }
}

GetAllTreesResponse {
    list of {
        Tree,
        list of Branches
    }
}
```

I think that's much better :)

Plus, this design is a ton more flexible. Imagine some new route doesn't care about branches at all,
only about trees and it's leaves. It's easy to define it like this:

```haskell
OnlyTreesAndLeavesPlease {
    list of {
        Tree,
        list of Leaves
    }
}
```

But notice we didn't sacrifice today's simplicity to accomodate for tomorrow's vague and fuzzy
possibilities. It's simple whether `OnlyTreesAndLeavesPlease` is added or not.

Another point here is that the only nesting that exists in these messages occurs because of repeated
items. If a tree could only have one branch, which in turn could only have one leaf, the structure
would be completely flat, like this:

```haskell
GetAllTreesResponse {
    Tree,
    Branch,
    Leaf
}
```

Side note: once all clients update their code to remove deprecated fields, we can `reserve` them,
cleaning the protobuf code up.

One caveat of this approach is that you always need to define all of the entities (trees, branches,
leaves) that will be returned, leading to more code repetition. In my case, I think it was a well
worthwhile tradeoff, but your mileage may vary.

---

I'd like to interlude for a moment to talk about **system boundaries** and **software design**. Ted
Kaminski describes it in detail in [System boundaries: the focus of design](https://www.tedinski.com/2018/02/06/system-boundaries.html),
but I'll give an overview.

Why am I writing this long rant about a design mistake instead of just fixing it? Isn't rapid
iteration The Way to write software?

If instead of a public API all of these interfaces were internal to a module, [...]
