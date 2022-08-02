open System
open FSharp.Core
open FSharp.Collections

type Node = {
    To: Map<char, int>
    Main: option<int>
    Len: int
    mutable Link: int
}

module Node =
    let empty = {
        To = Map.empty;
        Main = None;
        Len = 0;
        Link = 0
    }

type SuffixAutomaton() =
    let mutable last = 1
    let mutable nodes = ResizeArray<Node>([ Node.empty; Node.empty ])

    let (>->) nodeID input =
        nodes.[nodeID].To.TryFind input
        |> Option.defaultWith (fun () -> 0)

    let NewNode node =
        let id = nodes.Count
        nodes.Add node
        id

    let PushEdge from chr _to =
        nodes.[from] <- { nodes.[from] with To = nodes.[from].To.Add (chr, _to)   }


    member this.Push(c: char) =
        let u = NewNode { Node.empty with
                            Main = Some (nodes.Count - 1);
                            Len = nodes.[last].Len + 1 }

        let mutable p = last
        last<- u

        // For all superthreadsets that die on input c, we add an edge p -c-> u
        while p >-> c = 0 do
            PushEdge p c u
            p <- nodes.[p].Link

        // The current `p` superthreadset does not die on input c!
        // Instead, it goes to `q`
        let q = p >-> c

        // Actually, there's no such `p`
        if p = 0 then
            nodes.[u].Link <- 1

        // p -c-> q is actually the biggest path from `p` to `q`
        // It's enough to add `u` to the threadset of `q`
        else if nodes.[p].Len + 1 = nodes.[q].Len then
            nodes.[u].Link <- q

        // If we add `u` to the threadset of `q`, we'll ruin some bigger path
        // from `p` to `q`...
        // Solution: clone `q` and reroute `p -c-> q` to `p -c-> clone`
        else
            let clone = NewNode { nodes.[q] with
                                    Main = None;
                                    Len = nodes.[p].Len + 1 }

            //? Unclear
            nodes.[u].Link <- clone
            nodes.[q].Link <- clone

            //? Unclear
            while p >-> c = q do
                PushEdge p c clone
                p <- nodes.[p].Link

    member val Nodes = nodes

    member this.SortedNodes() =
        [| 1..nodes.Count-1 |]
        |> Array.sortBy (fun u -> nodes.[u].Len)


let formatSet =
    Seq.map string
    >> String.concat ", "
    >> sprintf "{%s}"

let buildThreadset (sa: SuffixAutomaton) =
    let nodes = sa.SortedNodes()
    let mutable threadset = [| for _ in 0..nodes.Length -> Set.empty<int> |]
    for u in Array.rev nodes do
        match sa.Nodes.[u].Main with
        | Some x -> (threadset.[u] <- threadset.[u].Add x)
        | None -> ()

        let p = sa.Nodes.[u].Link
        threadset.[p] <- Set.union threadset.[p] threadset.[u]

    threadset


let printNFA (input: string) =
    let n = input.Length

    let fedges = String.concat "\n" [
        for u in 1..n do
            yield sprintf "0 -> %d [ label = \"%c\" ];" u input.[u-1]
            yield sprintf "%d -> %d [ label = \"%c\" ];" (u-1) u input.[u-1]
    ]

    let nodes = String.concat ", " ([ 0..n ] |> List.map string)

    printfn """
digraph {
    rankdir=TB;

    node [
        shape = circle;
    ]

%s
    { rank=same; %s }
}
    """ fedges nodes


let printDFA input =
    let mutable sa = SuffixAutomaton()
    for c in input do
        sa.Push(c)

    let nodes = sa.SortedNodes()
    let threadset = buildThreadset sa

    let fnodes = String.concat "\n" [
        for u in nodes ->
            sprintf "    %d [ label = \"%s\" ];" u (formatSet threadset.[u])
    ]

    let fedges = String.concat "\n" [
        for u in nodes do
            for pr in sa.Nodes.[u].To do
                let (c, v) = pr.Key, pr.Value
                yield sprintf "    %d -> %d [ label = \"%c\" ]" u v c
    ]

    let flinks = String.concat "\n" [
        for u in nodes ->
            sprintf "    %d -> %d [ color = lightblue ];" u sa.Nodes.[u].Link
    ]

    printfn """
digraph {
    rankdir = "LR";

    node [
        shape = none;
    ]

%s

%s

%s
}
    """ fnodes fedges flinks


[<EntryPoint>]
let main argv =

    let input = Console.ReadLine ()

    if argv.Length >= 1 && argv.[0] = "nfa" then
        printNFA input
    else
        printDFA input

    0
