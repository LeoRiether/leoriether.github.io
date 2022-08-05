+++
title = "Árvores Para Programação Competitiva"
[taxonomies]
categories = ["trees", "lectures"]
tags = ["trees", "paths", "subtrees", "euler tour"]
+++

> link da aula: [https://leoriether.github.io/posts/trees](https://leoriether.github.io/posts/trees)

O que é uma árvore?

- Grafo com `N` nós e `N-1` arestas
- Conexo
- Existe um caminho único entre quaisquer dois vértices

*Escolha dois.*

---

Uma árvore pode ou não ter raíz. Se não tiver, é bem provável que a gente possa
escolher uma raíz qualquer!

# ★ Subárvores
## DFS

Muito útil para quando vc quer
1. informação das subárvores pra calcular algo pro vértice, ou
2. informação do caminho `raíz -> u` pra calcular algo pra `u`
3. por que no los dos? (acho que nunca vi problema que usa os dois ao mesmo
   tempo, mas fiquem de olho que eu posso settar um a qualquer momento)
4. fazer basicamente qualquer coisa em árvore (basicamente suas duas únicas
   opções são DFS ou BFS)

```cpp
vector<int> g[N];

void dfs(int u, int p = 0) {
    for (int v : g[u]) if (v != p) {
        dfs(v, u);
    }
}
```

> tamanho de subárvore: [https://cses.fi/problemset/task/1674](https://cses.fi/problemset/task/1674)

## DFS Time / Preorder Time / Euler Tour 

> A única estrutura mais fácil que uma árvore é um **array** 

Acho que o único jeito de explicar isso é fazendo uns exemplos? 🤔

# ★ Caminhos
## Lowest Common Ancestor (LCA)

- O que é?
- Ideia: **todo caminho entre `u` e `v` em uma árvore pode ser dividido em dois
  caminhos: `u -> LCA(u, v)` + `v -> LCA(u, v)`**

### Às vezes você só precisa saber que o LCA existe...
#### Diâmetro
> link: [https://cses.fi/problemset/task/1131](https://cses.fi/problemset/task/1131)

Truque: transformar a árvore em binária

### ...mas outras vezes você precisa calculá-lo

Problema: Q queries de encontrar a distância entre dois vértices

### Binary lifting
> cses: [https://cses.fi/problemset/task/1687](https://cses.fi/problemset/task/1687)

## Problema facinho mas tricky do cses
> link: [https://cses.fi/problemset/task/1138](https://cses.fi/problemset/task/1138)

## Problema Chefão: Empresa de Festas
> link: [http://maratona.sbc.org.br/hist/2020/primfase20/maratona20.pdf](http://maratona.sbc.org.br/hist/2020/primfase20/maratona20.pdf)

<details>
<summary>Dica</summary>
A gente pode resolver offline!</details>

# Coisas que só vou falar se der tempo
## Resolver offline
- Muitas vezes é mais fácil resolver um problema **offline** em árvore, porque
  podemos reaproveitar os resultados das subárvores para calcular o resultado de
  um nó!
- Outras vezes, você pode resolver offline mantendo uma estrutura de dados sobre
  um caminho do pai até `u`

## Rerooting
> problema: [https://cses.fi/problemset/task/1132](https://cses.fi/problemset/task/1132)
>
> problema: [https://cses.fi/problemset/task/1133/](https://cses.fi/problemset/task/1133/)

## Centroid Decomposition 
- Modo de decompor a árvore
- De forma parecida com o LCA, todo caminho entre dois vértices **em subárvores

# ★ Ler Mais
- [Small to Large / DSU on Tree / Sack](https://codeforces.com/blog/entry/44351)
- [Decomposição Heavy-Light](https://cp-algorithms.com/graph/hld.html#proof-of-correctness)
- [7th Litte Technique](https://codeforces.com/blog/entry/100910) (nos
  comentários você encontra mais uns 2 ou 3 truques relacionados!)
- [Árvores Link-Cut](https://www.youtube.com/watch?v=XZLN6NxEQWo)
- [Virtual Trees](https://codeforces.com/blog/entry/76955)
