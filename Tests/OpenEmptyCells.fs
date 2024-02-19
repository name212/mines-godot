module TestsOpenEmptyCells

open NUnit.Framework
open FsUnit
open Logic
open Types

let testField: Field = {width = 8; height = 8; mines = 0}

[<Test>]
let ``Open any position in field without mines should open all field`` () =
    let opn pos =
      let startPos = {x = 0; y = 0}
      let f = generateRandomField startPos {testField with mines = 0 } id
      openEmptyCellsAround f pos
    
    let openedCell = fun c -> c.state = Opened
    let shouldOpenedAllField cells = should haveLength (testField.width * testField.height) cells
    
    opn {x = 0; y = 0} |> List.filter openedCell |> shouldOpenedAllField
    opn {x = 1; y = 1} |> List.filter openedCell |> shouldOpenedAllField
    opn {x = 0; y = 7} |> List.filter openedCell |> shouldOpenedAllField
    opn {x = 7; y = 7} |> List.filter openedCell |> shouldOpenedAllField
    opn {x = 7; y = 0} |> List.filter openedCell |> shouldOpenedAllField
    opn {x = 4; y = 4} |> List.filter openedCell |> shouldOpenedAllField
    opn {x = 0; y = 4} |> List.filter openedCell |> shouldOpenedAllField
    opn {x = 4; y = 7} |> List.filter openedCell |> shouldOpenedAllField
    