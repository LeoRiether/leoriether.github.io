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
the time complexity **if the tree is balanced**?.
> TODO: translate
Caso bom interessante: a árvore é bem balanceada, a complexidade de fazer uma dfs em cada nó é semelhante à complexidade de um mergesort. O número de operações fica `N + N/2 + N/2 + N/4 + N/4 + N/4 + N/4 + ... = O(N log N)`.

![t2](https://user-images.githubusercontent.com/8211902/120910404-1ca49500-c655-11eb-92db-914a99183ef1.png)

### Ideias Intuitivas de Otimização

#### 1. Offline
Em vez de responder as queries na ordem que foram dadas, nesse problema é possível calcular as respostas na ordem que for mais fácil, e só responder depois de ter calculado todas as respostas.

#### 2. Reutilizar DFSs Dos Filhos
Voltando ao exemplo do "caso ruim", a gente realmente precisa visitar todos os filhos em toda dfs? Resposta: não, é possível reutilizar o array de frequências de um filho e apenas acrescentar o nó atual da dfs para atualizar o array.

Mas isso sempre é possível? O que fazer nesse caso?

![t3](https://user-images.githubusercontent.com/8211902/120910622-bb7dc100-c656-11eb-909a-4db0bc26e36b.png)

Resposta: sim, mas com uma modificação. Primeiro temos que fazer a dfs no vértice 7 e **limpar** a tabela de frequências dele. Depois, prosseguimos com a dfs dos outros nós do jeito otimizado que discutimos anteriormente.

# Small to Large

As técnicas mostradas já são suficientes para encontrar um algoritmo `O(N log N)` para o problema exemplo! A ideia geral de uma chamada da dfs é: calcular a resposta para os filhos "pequenos", **limpar a estrutura de dados que eles criaram** (caso seja global, geralmente é), calcular a resposta para o filho "grande" e **reutilizar a estrutura que ele montou**.

__O resto é WIP.__


