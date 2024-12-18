+++
title = "Law of Values"
+++

Stated somewhat abstractly, the Law of Values says that

> Once you define your **core values**, your most primordial beliefs and priorities, most
> of the **design** follows immediately

Here are concrete variations:
- Once you define the core values to be prioritized in a project, most of the solution
  follows immediately.
- Once you define what you value in a city, most of its architecture and and
  infrastructure characteristics follow immediately
- Once you define what you value in a society, most of your political and economic beliefs
  follow immediately
- Once you define what you value in life, most of your actions and objectives _should_
  follow immediately

The law is useful in two ways. I'll use a software engineering context here, but it's not
too hard to translate these into other contexts as well:

First, it can be helpful to state explicitly what you value to get more clarity about
what you should and shouldn't put in a project design --- instead of purely exploring
design alternatives directly.

You can see matklad doing just that in [this TigerBeetle video](https://youtu.be/hPUL8Xo6MJw?si=v4JG3GgHBnKsGBAB&t=197).
For TigerBeetle, some of the core tenets of the solution are Reliability and Correctness.
Because of the Reliability tenet, it soon follows that Performance should be a major
concern as well, which leads to the choice of language (Zig in their case), a big part of
their data model, the kind of operations the interface exposes, and a lot of other design
decisions.

Second, there's a kind of contrapositive of the law: different designs follow from
different sets of values. Because of this, whenever a team disagrees on the direction a
project should take, it can be more fruitful to discuss "what should we value/prioritize
in this project" instead of the direction itself. For example, if some members believe a
system should be monolithic, while others think it should be distributed, instead of
discussing pros and cons of monolithic and distributed systems, it can be easier to reach
an agreement by defining "Fault-Tolerance" as a core value of the project (or the
opposite). Once that's defined, it's clear that the system should be distributed.

Note that there's a fundamental difference between discussing pros/cons and discussing
core values. In the above example, even though fault-tolerance could be listed as a pro of
the "distributed" alternative, by listing it separately we get a better chance to make it
a top priority (or the opposite!).

Plus, if there's disagreement on whether it should be a top priority or not but the
discussion is pros/cons-oriented, it's inevitable someone will say "distributed is
fault-tolerant!", to which the response will be "but the monolith is simpler and easier to
deploy!", leading to a standstill. Neither is wrong, they just value different things.

