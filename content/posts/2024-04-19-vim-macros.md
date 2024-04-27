+++
title = "The Most Important Vim Mapping (it just runs a macro)"
date = 2024-04-19
+++

<!--
    Structure:
    - 'Q' mapping
    - Macros vs multiple cursors 
        - Macros are harder, but the `Q` mapping 
        - There are definitely use cases for multiple cursors as well, macros have a learning curve and are harder to use
    - Caveats
        - You have to think about how the macro is going to play out the other times (there's no preview)
        - Sometimes you start doing something repetitive and only remember to record a macro after a few times (or don't remember at all)
-->

This is a post about how I finally started using macros in neovim, after many
times of trying them and giving up. If you don't use vim macros yet or would
like to learn more, I definitely recommend watching
[That One Micro Talk on Macros (NeovimConf 2023)](https://www.youtube.com/watch?v=5x3dXo8aDCI) by Jesse Leite.
In the video, Jesse introduces this wonderful mapping:

```lua
vim.keymap.set('n', 'Q', '@qj', {remap=true})
```

Ok, that was for neovim. In vimscript, it's

```vim
nmap Q @qj
```

And that's it! The most important mapping in my config! It just runs the macro
`q` and goes down a line. Honestly I'm thinking about changing it to map to
`@q` only and it would still be the most important mapping in my config. Let me
explain.

## The Problems With Macros

It's common to hear people talking about how _powerful_ vim macros are, but I
always thought they were too clunky. It's easy to make a macro that will do
unexpected things when replayed and `@q`/`@@` are so awkward to type. This lead
me to search for alternatives like dot-repeating and substitution with `:s`, but
these only work in simple cases and can be repetitive.

## The Solution

Then, everything changed when NeovimConf published the talk. Only Jesse Leite,
master of all macros, could teach us, and when the world needed him most, he was
there. `Q` doesn't only reduce the barrier to replay a macro, it completely
destroys it. It's impressive how much cheaper macros become when they're so
spammable!

I also started getting much better at making macros that do well on replays, not
because `Q` changes anything fundamentally, but because I was getting a lot more
practice.

Since I began using `Q`, I've used macros in a number of interesting
scenarios, including:

### Transforming gRPC service definitions into Go functions

Suppose I had a gRPC service like the one below

```proto
service ThingService {
  rpc AddThing(AddThingRequest) returns (AddThingResponse)
  rpc DeleteThing(DeleteThingRequest) returns (DeleteThingResponse)
  rpc RenameThing(RenameThingRequest) returns (RenameThingResponse)
  rpc GetThing(GetThingRequest) returns (GetThingResponse)
}
```

To implement this service in Golang, I copied the 4 lines of rpcs (it was more
like 7 in the real case) into a `.go` file and recorded a macro, turning it into

```go
func (s *ThingService) AddThing(
    ctx context.Context,
    r *proto.AddThingRequest
) (*proto.AddThingResponse, error) {
    return nil, nil
}

  rpc DeleteThing(DeleteThingRequest) returns (DeleteThingResponse)
  rpc RenameThing(RenameThingRequest) returns (RenameThingResponse)
  rpc GetThing(GetThingRequest) returns (GetThingResponse)
```

For the other three functions, the macro does its job

```go
func (s *ThingService) AddThing(
    ctx context.Context,
    r *proto.AddThingRequest,
) (*proto.AddThingResponse, error) {
    return nil, nil
}

func (s *ThingService) DeleteThing(
    ctx context.Context,
    r *proto.DeleteThingRequest,
) (*proto.DeleteThingResponse, error) {
    return nil, nil
}

func (s *ThingService) RenameThing(
    ctx context.Context,
    r *proto.RenameThingRequest,
) (*proto.RenameThingResponse, error) {
    return nil, nil
}

func (s *ThingService) GetThing(
    ctx context.Context,
    r *proto.GetThingRequest,
) (*proto.GetThingResponse, error) {
    return nil, nil
}
```

### Writing the gRPC service example above

That whole service definition is very repetitive! I actually initially wrote

```proto
service ThingService {
  Add
  Delete
  Rename
  Get
}
```

Recorded a macro to modify the first rpc into

```proto
service ThingService {
  rpc AddThing(AddThingRequest) returns (AddThingResponse)
  Delete
  Rename
  Get
}
```

and replayed the macro for the other 3 rpcs, remembering to yank the first word
to paste before `ThingRequest` and `ThingResponse`.

### Adding a property to certain items in a yaml

In another case, I had a huge yaml and needed to modify about 30 list items by
adding a new `flag: true` property to them. For this, I opened two buffers
side-by-side -- yaml on the left, a list of IDs that should be modified on the
right. The macro starts on the right, hovering the current ID, then goes to the
left, searches and modifies an item and goes back to the right. By pressing `Q`
multiple times, we jump around the yaml making changes one by one, but always
going back to the next ID on the list!  

Maybe this could be done using the quickfix list as well, but the macro is so
simple I would probably do it this way again, if I need.

## Caveats

I argue macros are much more convenient after mapping `Q` to `@qj`, but they
_can_ still feel clunky and hard to use, mostly because of replayability issues.
It does get better with practice, but there's definitely a learning curve. For
those of you who swear by multiple cursors, I'll give you that: they're much
easier to use. Oh and there's no "preview" for macros! If you get it wrong,
you'll only know after recording and you'll have to record again.

Using the right text-objects does make a big difference, though, because it can
make motions more "generic". I find that
[chrisgrieser/nvim-various-textobjs](https://github.com/chrisgrieser/nvim-various-textobjs)
has been very useful in that regard.

## Conclusion

Watch [That One Micro Talk on Macros (NeovimConf 2023)](https://www.youtube.com/watch?v=5x3dXo8aDCI),
map `Q` to `@qj` and be happy.
