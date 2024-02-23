module ``Func recalculateField``

open NUnit.Framework
open FsUnit
open Logic
open Types

let testField: Field = {width = 8; height = 8; mines = 0}
let openedCell = fun c -> c.state = Opened
let closedCell = fun c -> c.state = Closed

[<Test>]
let ``should open all field when open any position in field without mines `` () =
    let open' pos =
      let startPos = {x = 0; y = 0}
      let f = generateRandomField startPos {testField with mines = 0 } id
      let newField = recalculateField pos f
      newField.cells
    
    let shouldOpenedAllField cells = should haveLength (testField.width * testField.height) cells
    
    open' {x = 0; y = 0} |> List.filter openedCell |> shouldOpenedAllField
    open' {x = 1; y = 1} |> List.filter openedCell |> shouldOpenedAllField
    open' {x = 0; y = 7} |> List.filter openedCell |> shouldOpenedAllField
    open' {x = 7; y = 7} |> List.filter openedCell |> shouldOpenedAllField
    open' {x = 7; y = 0} |> List.filter openedCell |> shouldOpenedAllField
    open' {x = 4; y = 4} |> List.filter openedCell |> shouldOpenedAllField
    open' {x = 0; y = 4} |> List.filter openedCell |> shouldOpenedAllField
    open' {x = 4; y = 7} |> List.filter openedCell |> shouldOpenedAllField

[<Test>]
let ``should open only one cell when open position near closed cell with bomb`` () =
    let startPos = {x = 7; y = 7}
    let f = generateRandomField startPos {testField with mines = 1 } (fun l -> [0])
    let positionForOpen = {x = 1; y = 1}
    let resultCells = (recalculateField positionForOpen f).cells 
    
    resultCells |> List.filter openedCell |> should haveLength 1
    resultCells |> List.filter openedCell |> List.head |> position |> should equal positionForOpen
    resultCells |> List.filter closedCell |> should haveLength (testField.Size - 1)

[<Test>]
let ``should open all field with one bomb when cell with bomb marked as bomb correctly`` () =
    let startPos = {x = 7; y = 7}
    let bombPos = {x = 1; y = 1}
    let positionForOpen = {x = 1; y = 0}
    let f1 = generateRandomField startPos {testField with mines = 1 } (fun l -> [testField.Linear bombPos])
    let f1AfterChange = changeCellState f1 bombPos MarkAsBomb
    let resultCells = (recalculateField positionForOpen f1AfterChange).cells
    
    resultCells |> List.filter openedCell |> should haveLength (testField.Size - 1)
    resultCells |> List.filter (fun c -> c.state = MarkAsBomb) |> should haveLength 1
    