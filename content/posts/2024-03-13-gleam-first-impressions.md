+++
title = "Gleam First Impressions"
draft = true
[taxonomies]
#categories = ["programming-languages"]
#tags = ["gleam", "programming-languages"]
+++

I decided to give another try to [Gleam](https://gleam.run) the other day, given that their
[version 1](https://gleam.run/news/gleam-version-1/) came out recently.

I remembered very little about the language from the other time I tried it, so even if these were
not technically my very first impressions, they're the first impressions after I forgot about it.

So, what do I think about this cute little tiny language? 

## LSP

## Early Returns

One of the things I miss the most in functional programming languages is early returns. Something I
might do in Rust like

```rust
fn the_best_example_I_could_think_of() -> Vec<i32> {
    let x = match f() {
        Some(x) => x,
        None => return vec![],
    };

    let y = g(x) {
        Some(y) => y,
        None => return vec![0],
    };

    vec![x, y]
}
```

I may have to do in Gleam like

```gleam
fn the_best_example_i_could_think_of_but_in_gleam() -> List(int) {
    case f() {
        None -> [] 
        Some(x) ->
            case g(x) {
                None -> [0]
                Some(y) -> [x, y]
            }
    }
}
```

Maybe there's a way to avoid this level of indentation using the `use` keyword? 
