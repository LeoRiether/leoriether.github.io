+++
title = "Intuition Behind the Small to Large Algorithm"
date = 2021-06-05
[taxonomies]
categories = ["trees"]
tags = ["trees", "algorithms", "optimization", "small to large"]
+++

# The Problem
What if we want to build some data structure for every node in a tree, to solve
a certain problem, but to build that structure for node `u` we need to trasverse
the entire subtree of `u`, thus making the algorithm at least `O(N²)`, which is
not fast enough in some cases, could we do better?

## Example
> You're given a tree with `N` nodes ($N \le 10^5$), in which every node has a
value between $1$ and $10^6$. Answer `Q` queries ($Q \le 10^5$) that give you an
integer `k`, a node `u`, and ask you how many nodes in the subtree of `u` have
value equal to `k`. 

![t0](https://user-images.githubusercontent.com/8211902/120910700-4bbc0600-c657-11eb-9df6-493f39562b62.png)

If the constraints were smaller, for example $N \le 10^3$ and $Q \le 10^3$, it
would be possible to solve this problem by running a DFS starting at the node
given in the query. The DFS would create a data structure -- a frequency array
-- that allows us to lookup the answer easily. That's not the simplest way to
solve this problem in particular, but it's one that exemplifies the kind of
reasoning behind Small to Large, so bear with me.

With the bigger constraints, however, this algorithm isn't fast enough because
we go through `O(N)` nodes each query, in the worst case.

![t1](https://user-images.githubusercontent.com/8211902/120910175-01d12100-c653-11eb-86d8-dd77f574beae.png) 

In some cases, however, we don't have `O(NQ)` complexity. Imagine, first, that
we cache the frequency tables for nodes we've already processed. Then, what's
the time complexity **if the tree is balanced and binary**?. Then, we would take
`N` time in the root node, `N/2 + N/2` time in the children of the root, `N/4 +
N/4 + N/4 + N/4` in the children of the children of the root node, ... It's
exactly like a mergesort: `O(N log N)`!

![t2](https://user-images.githubusercontent.com/8211902/120910404-1ca49500-c655-11eb-92db-914a99183ef1.png)

### Optimization Ideas
What are some heuristics we could use to optimize the naive algorithm?

#### 1. Offline
Instead of answering queries in the order they are given, we could compute them
in whichever order is most convenient to us, store the answers, then print them
in the right order.

#### 2. Reuse the result from some DFSs 
Let's go back to the example tree that gave us `O(N²)` runtime. Did we really
need to run a DFS for every single node? The answer is no! We could, in that
particular case, reuse the frequency table generated in the child and update it
in `O(1)` to get the frequency for the parent, like so:
```cpp
int frequency[MAX_K];
void dfs(int u) {
    dfs(child[u]);
    frequency[value[u]]++;
    // we now have a valid frequency table for `u`
}
```

But is this always possible? What should we do in this case?

![t3](https://user-images.githubusercontent.com/8211902/120910622-bb7dc100-c656-11eb-909a-4db0bc26e36b.png)

Answer: yes, but there's a catch. First, we should run DFS for node 7 and clear
the frequency table. Then, we carry on with the DFS of the other nodes in the
optimized way, reusing tables from the child.

# Small to Large

The optimizations shown are sufficient to make an `O(N log N)` algorithm for the
problem! The general idea is that a DFS call will

1. Compute the data structures for each "small" child (small in the sense that
   they have less nodes in their subtrees, like 7 only had 1 node in its
   subtree);
2. Clear the data structures in each of their DFS calls (assuming the DFS
   modifies one global data structure);
3. Compute the data structure for the "big" child (the one with the biggest
   subtree), but instead of clearing it, reuse it to compute the parent's
   answer.
4. Iterate over the subtrees of the small children and the parent node, updating
   the data structure accordingly.

The rest of this blog is still WIP ;)

