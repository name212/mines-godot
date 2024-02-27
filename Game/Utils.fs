module Utils

open System

let public shuffle lst =
  let rec shfl acc l =
    match l with
    | [] -> acc
    | ll -> match List.splitAt (Random.Shared.Next(ll.Length)) ll with
            | (f, []) -> shfl acc f
            | (f, [a]) -> shfl (acc @ [a]) f
            | (f, s) -> shfl (acc @ [s.Head]) (f @ s.Tail) 
  shfl [] lst