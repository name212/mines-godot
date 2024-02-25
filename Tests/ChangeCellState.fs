module ``Func changeCellState``

open Microsoft.FSharp.Collections
open NUnit.Framework
open FsUnit
open Types
open Logic

let field = {width = 8; height = 10; mines = 1 }
let startPosition = {x = 1; y = 1}
let testMinesField = generateRandomField startPosition field (fun _ -> [field.Linear startPosition])

[<Test>]
let ``should set provided cell state for provided position`` () =
    (testMinesField.MustCell startPosition).state |> should equal Closed
    let resultField = changeCellState startPosition Opened testMinesField
    (resultField.MustCell startPosition).state |> should equal Opened
    resultField.cells
    |> List.filter (fun c -> c.pos <> startPosition)
    |> List.filter (fun c -> c.state = Closed)
    |> should haveLength (field.Size - 1)
    