+++
title = "Testing Resources — A Literature Review"
date = 2024-04-04
[taxonomies]
categories = ["tests"]
+++

<!--
    Links:
    - https://matklad.github.io/2021/05/31/how-to-test.html
    - https://concerningquality.com/model-based-testing/
    - [Use weird tests to capture tacit knowledge · Applied Cartography](https://jmduke.com/posts/essays/weird-tests-tacit-knowledge/)
    - https://martinfowler.com/articles/mocksArentStubs.html
    - https://www.brandons.me/blog/thoughts-on-testing
    - https://buttondown.email/hillelwayne/archive/cross-branch-testing/

    Techniques:
    - Example testing / Table-based
    - Snapshots
        - <https://matklad.github.io/2021/05/31/how-to-test.html>
        - <https://docs.rs/expect-test/latest/expect_test/>
        - <https://selfie.dev/>
    - Property testing
        - <https://matklad.github.io/2021/05/31/how-to-test.html>
        - <https://www.hillelwayne.com/post/metamorphic-testing/>
        - [Property-testing async code in Rust to build reliable distributed systems - Antonio Scandurra](https://www.youtube.com/watch?v=ms8zKpS_dZE)
    - Model-based testing
    - Weird tests
    - Formal methods
-->

I've learned a lot about testing in the last few months, so the idea of this
post is to aggregate resources I think are useful in a structured way, both for
future reference and maybe, who knows, it might be useful to someone else. I'll
also give a brief introduction to each technique as motivation, but I expect the
real value of this post to be the links contained in it.

Before delving into more specific techniques, it's worth mentioning [How to Test](https://matklad.github.io/2021/05/31/how-to-test.html)
by matklad, as it contains a wealth of information, techniques and practices.
If I had to pick only one link to recommend, it would be this one. Many of the
other sections will overlap with How to Test.

So with that out of the way, let's talk about...

## Example testing

One of the easiest and most common ways of testing: come up with some example
inputs and check if the output of a method is as expected. 

```rust
#[test]
fn test_capitalize() {
    let input = "the quick Brown fOx JUMPS OVer THE LazY dog";
    let expected = "The Quick Brown Fox Jumps Over The Lazy Dog";
    let actual = capitalize(input);
    assert_eq!(actual, expected);
}
```

One variation of this method is table-driven testing, or data-driven testing. As
far as I know (and I have no source for this), this technique was popularized by
Go (see <https://go.dev/wiki/TableDrivenTests>), but is also recommended in [How to Test -- Data Driven Testing](https://matklad.github.io/2021/05/31/how-to-test.html#Data-Driven-Testing)
and has a similar implementation in JUnit's `@ParameterizedTest`s (see
<https://junit.org/junit5/docs/current/user-guide/#writing-tests-parameterized-tests>).

In my opinion, Go's implementation of table-driven tests is still the best.
Because the built-in testing library makes it easy to create subtests, you can
write something like this:

```go
func TestCapitalize(t *testing.T) {
    type testcase struct {
        Input    string
        Expected string
    }

    testcases := []testcase{
        {
            Input: "the quick Brown fOx JUMPS OVer THE LazY dog",
            Expected: "The Quick Brown Fox Jumps Over The Lazy Dog",
        },
        {
            Input: "the quick brown fox jumps over the lazy dog",
            Expected: "The Quick Brown Fox Jumps Over The Lazy Dog",
        },
        {
            Input: "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG",
            Expected: "The Quick Brown Fox Jumps Over The Lazy Dog",
        },
        {
            Input: "hello. world!",
            Expected: "Hello. World!",
        },
    }

    for i, tc := range testcases {
        description := fmt.Sprintf("(#%d) %s", i, tc.Input)
        t.Run(description, func(t *testing.T) {
            t.Parallel()
            actual := Capitalize(tc.Input)
            if actual != tc.Expected {
                t.Fatalf("Expected <%v> but got <%v>", tc.Expected, actual)
            }
        })
    }
}
```

Notice we start the subtest with a `description` in `t.Run(description, ...)`.
If this test fails, that description is reported in the failure message, making
it easy to find which test failed! This is a pain point reported in How to Test:

> But there’s a drawback as well — without literal #[test] attributes,
> integration with tooling suffers. For example, you don’t automatically get “X
> out of Y tests passed” at the end of test run.

Indeed, in Rust's standard testing lib, there's no way to run subtests and have
them reported neatly, unfortunately.

## Snapshot testing

Also known as golden tests or expect tests, snapshot testing is example-based
testing with a twist. When a snapshot test breaks, instead of manually fixing
the expected values there's a mode of running the tests that **updates the
expectations based on the real outputs**, potentially saving you a lot of time.

Althout matklad recommends this technique in [How to Test -- Expect-Tests](https://matklad.github.io/2021/05/31/how-to-test.html#Expect-Tests),
I'll be using the (very recent!) library [selfie](https://selfie.dev/), which
runs in the JVM, for an example. With selfie, you'd write a test like this
(blatantly stolen from their frontpage):

```java
@Test
public void primesBelow100() {
  expectSelfie(primesBelow(100).toString()).toBe_TODO();
}
```

When running the test, selfie recognizes the `_TODO()` invocation and replaces
it in your own files by

```java
@Test
public void primesBelow100() {
  expectSelfie(primesBelow(100).toString())
    .toBe("[2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97]");
}
```

Of course, you still need to verify that the output is really what you want. If
that prime list contained a `42` in the middle and no one noticed, the tests
would pass anyway, but that's why it's called snapshot testing: the expectation
captures a snapshot of `primesBelow(100)` and if it ever changes we'll be
notified by a failing test.

Selfie also supports saving the expected string in a file, as well as some more
advanced features which are less common in the snapshot testing libraries I've
seen. The [facets](https://selfie.dev/jvm/facets) page exemplifies one of those.

In Go land, it's more common to see this pattern of testing named as "golden
files". I'm not sure if the naming influences the implementations or the
opposite, but indeed it's less common to see "inline" expectations such as
`.toBe("[list of primes...]")`. Instead, libraries such as
[goldie](https://github.com/sebdah/goldie) always store expected strings in a
(golden) file. This makes sense for longer strings, but I think this approach
makes it harder to review the correctness of the output, given that it's always
in another file.

## Model-based Testing

A characteristic of example and snapshot-based testing is that they require that
the expected output of the program is "hardcoded" into the test. Because this
requires hand-crafting expectations, this can be impractical, especially when
the output is complex or hard to verify.

In some situations, we can implement another version of the program that does
the same thing but implemented in an easier way (a model), perhaps with
different performance characteristics, and compare the outputs of the two
programs.

For example, to test an optimizing compiler, one could implement a much simpler
interpreter and verify for various inputs that both compiled and interpreted
programs produce the same output. 

Although I haven't read much on model-based testing,
[Property-Based Testing Against a Model of a Web Application](https://concerningquality.com/model-based-testing/)
seems like a good starting point.

## Property Testing

Example, snapshot and model-based tests all need a test oracle: the expected
output needs to be known and compared against the real one. This approach has
some limitations, of course, because the expected output is a lot of times too
big, complex or hard to generate. Property testing is the first way I'll
introduce of testing **without an oracle**!

The idea here is to come up with a kind of "mathematical" property of the system
under test, and check if the property holds for the inputs (these are often
randomly generated).

For example, suppose we're writing a UUID library. UUIDs have a `ToString`
method, but we also wrote a `FromString` function. It makes sense, then, to
test that calling `ToString` and then `FromString` should yield the original
UUID, a sort of "inversibility" or "round-tripness" property. This is by far the
most commonly cited property, it can be tricky to find others!

F# for Fun and Profit has
[an entire series on property-based testing](https://fsharpforfunandprofit.com/series/property-based-testing/),
which covers the topic quite well, from the basics to ways of finding new
properties.

A talk that's pretty specific to Rust, but nonetheless very interesting is
[Property-testing async code in Rust to build reliable distributed systems](https://www.youtube.com/watch?v=ms8zKpS_dZE)
by Antonio Scandurra. In it, Antonio explains how [Zed](https://zed.dev/)
wraps the async executor in test code to randomize the order of concurrent
tasks, thus covering cases that would be much harder to trigger otherwise,
also making the concurrency deterministic (randomized with a known seed) in
the process (actually, I'm not sure it's fully deterministic? Not sure about
this part, please go watch the video :).

Surprisingly (for me at least), Go's standard testing library has a
property-testing framework called [testing/quick](https://pkg.go.dev/testing/quick),
based on Haskell's [QuickCheck](https://hackage.haskell.org/package/QuickCheck).
In Go's version, you write a `func (x Input) bool` and the library generates
values of `x` to try and make it return `false`, in which case the test would
fail.

## Metamorphic Testing

Metamorphic testing is actually a flavor of property testing, a subset of ways
you could pick a property. This technique relies on generating inputs like
in property tests, then modifying it in a way that we know what should change in
the output. For example, suppose we're testing a `StableSort` function. If we
shuffle the input, the output should not change at all, but if we concatenate
the input to itself (e.g. `[1, 2, 3] => [1, 2, 3, 1, 2, 3]`), the output will be
interspersed with itself (e.g. `[1, 2, 3] => [1, 1, 2, 2, 3, 3]`).

[Metamorphic Testing](https://www.hillelwayne.com/post/metamorphic-testing/) by Hillel Wayne
gives great examples of the technique and links some academic papers, if you're
into that. Unfortunately, it does not link [Cross-Branch Testing](https://buttondown.email/hillelwayne/archive/cross-branch-testing/),
also by Hillel Wayne, about a method that could be considered snapshot testing,
model-based testing, property testing and metamorphic testing all at once! In
short, the idea consists in using another git branch as a model and checking the
property `thing(X) == thing_from_another_branch(X)`. Kind of like a snapshot!

## Weird Tests

Surely this can't be the only term used for these kinds of tests, but [Use weird tests to capture tacit knowledge](https://jmduke.com/posts/essays/weird-tests-tacit-knowledge/)
uses it and so will I.

Weird tests test things you wish were true in your codebase, but are not
enforced by any other tooling. matklad explains it better than I could in [Basic Things -- Not Rocket Science Rule](https://matklad.github.io/2024/03/22/basic-things.html#Not-Rocket-Science-Rule):

> Maintain a well-defined set of automated checks that pass on the main branch
> at all times. If you don’t want large blobs in git repository, write a test
> rejecting large git objects and run that right before updating the main
> branch. No merge commits on feature branches? Write a test which fails with a
> pageful of Git self-help if one is detected. Want to wrap .md at 80 columns?
> Write a test :)
>
> It is perhaps worth you while to re-read the original post:
>           <https://graydon2.dreamwidth.org/1597.html>
>
> This mindset of monotonically growing set of properties that are true about
> the codebase is incredibly powerful. You start seeing code as temporary, fluid
> thing that can always be changed relatively cheaply, and the accumulated set
> of automated tests as the real value of the project.

Note: matklad calls "weird tests" the "not rocket science rule", but I don't get
the impression these are the same things from the original NRSR post? I'll
continue calling them weird tests :)

These are a great way of "documenting" processes and standards in a given
project! It helps both new team members and veterans that may have forgotten a
thing or two.

As a side note, I think running linters and format-checkers could be considered
"weird tests" as well. These are standard practice, why not make your own lints
as well?

## Formal Methods

So you like property testing, but worry that the random test cases might miss
something? Why not _mathematically prove_ those properties instead, then? Formal
methods allow us to verify properties for _all_ inputs, without actually running
the program! A lot of effort has been put into making this kind of technique
more mainstream, but it hasn't really catched on, unfortunately.

One way of writing these proofs is with languages that implement
[dependent typing](https://en.wikipedia.org/wiki/Dependent_type), like [Coq](https://coq.inria.fr/), [Agda](https://agda.readthedocs.io/en/v2.6.0.1/getting-started/what-is-agda.html), [Idris](https://idris2.readthedocs.io/en/latest/tutorial/theorems.html) and [Kind](https://github.com/HigherOrderCO/kind).
Types in these languages can encode proofs, so in some cases you don't even need
to write a test, instead choosing to return a type that proves the test would
pass (if it doesn't pass, it doesn't compile!). A big barrier to entry for
dependent typing is that you need to use these very specific languages to get
the benefit, you can't just `go get` a "dependent types library" to start using
it in a Go application that already exists, you have to write that application
from scratch in Agda or something. [Kind1](https://github.com/HigherOrderCO/Kind-Legacy) actually compiled
to JS, so you could make it work for some real-life applications, but this seems
to not be the case in the current version of Kind. I guess Kind compiles to C,
so you should be able to embed it in other stuff, but few people bother.

Another kind of formal method are [SMT Solvers](https://en.wikipedia.org/wiki/Satisfiability_modulo_theories), such as the [Z3 Theorem Prover](https://en.wikipedia.org/wiki/Z3_Theorem_Prover),
used by [Dafny](https://en.wikipedia.org/wiki/Dafny). In Dafny, you can specify
preconditions, postconditions, loop invariants and more, then given
enough information the compiler will automatically prove the constraints for
you, or give a counter-example. I haven't used SMT solvers myself, so I can't
say much here, but I think the overall idea is pretty cool. Again, though, the
code you want to prove needs to be written in Dafny if you want to prove things
with it, even if it does compile to a bunch of languages (currently C#, Go,
Python, Java and JavaScript). 

## Should You Really Write That Test?

Hopefully I've shown you some new and shiny testing techniques in this post and
you're now eager to try them all, get 100% coverage on every git repo you've
ever touched and spend the next few years fixing all those bugs you've found!

Calm down there, I don't hope that at all. In fact, I like the takes by Brandon
Smith in [Thoughts on Testing](https://www.brandons.me/blog/thoughts-on-testing).
As highlighted in the post, tests have to be maintained just like the rest of
the code in your codebase. It can be easy to add tests for the sake of adding
them, but this can do more harm than good. Tests that just mirror the
implementation, for example, can end up needing a lot of maintenance without
adding much benefit at all.

Another good post is [Testing on the Toilet: Testing State vs. Testing Interactions](https://testing.googleblog.com/2013/03/testing-on-toilet-testing-state-vs.html)
on the Google Testing Blog. The second test in it is an example of a test that
can give a false sense of security because even though the code is technically
tested, an incorrect implementation would still pass. 

## Test Scoping

It's common to restrict the scope of what you're testing by replacing
dependencies with [fakes, stubs or mocks](https://martinfowler.com/articles/mocksArentStubs.html).

As supported by [Thoughts on Testing](https://www.brandons.me/blog/thoughts-on-testing) and (again) [How to Test](https://matklad.github.io/2021/05/31/how-to-test.html), tests with a
really small scope (many unit tests fall into this category) tend to test very
little, but the amount of tests you write is large, increasing maintenance
burden while providing less security and confidence in the code. A good idea
seems to be [Testing at the boundaries](https://www.tedinski.com/2019/03/19/testing-at-the-boundaries.html),
alongside [Testing Features, Not Code](https://matklad.github.io/2021/05/31/how-to-test.html#Test-Features-Not-Code).
This keeps the implementation flexible and the public API more rigid.

Some people will argue for the complete opposite side of the discussion and
defend unit tests that mock everything, but I am not one of them, you'll have to
find the resources for this on your own.

## Conclusion

The main takeaway of this article is that you should read [How to Test](https://matklad.github.io/2021/05/31/how-to-test.html)
and other posts by matklad -- highly recommend [Basic Things](https://matklad.github.io/2024/03/22/basic-things.html) as well.

Aside from that, we've discussed testing techniques that require a test oracle,
like example-based, snapshot and model-based testing; as well as techniques that
don't, such as property, metamorphic and cross-branch testing.

These are good methods to apply to test the behavior of code, but you can test
properties of the code itself as well! Like custom linters, weird tests verify
that everyone is following a convention, or hasn't forgotten to add a thing
where it needs to be added, or didn't commit a typo in the docs, etc etc. These
tests can aid in knowledge distribution (if you break a test, it may specify the
way to fix it) and eases cognitive load (the tests can remind you of how to do
things). 

But sometimes the best test is no test at all! Some tests do more harm than
good and can give you a false sense of security while simultaneously making the
codebase harder to maintain. By testing features and system boundaries, it's
easier to maintain software that does what it's supposed to while minimizing
friction for extension and modification.

And that's it, folks! Happy testing :)

