+++
title = "How Floyd-Warshall is used to convert DFAs to regexes"
date = 2022-02-20
[taxonomies]
categories = ["automata"]
tags = ["shortest paths", "automata", "regex", "floyd-warshall", "dfas"]
+++

It's really fun to find unexpected connections between different areas of
knowledge, like how recursion and mathematical induction are essentially the
same thing, or how linear independence in vector spaces correlates with cycles
in graphs (so much so that [you can run Kruskal on either](https://acm.timus.ru/problem.aspx?space=1&num=1041)). The topic for
today is neither of these, it's actually about how the algorithm that converts a
deterministic finite automaton to a regular expression is actually very very
similar to Floyd-Warshall's shortest path algorithm. Let's start with the automata, shall we? 

# Deterministic Finite Automata

