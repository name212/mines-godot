module TestsOpenEmptyCells

open NUnit.Framework
open FsUnit
open Logic
open Types

let testField: Field = {width = 8; height = 8; mines = 0}
let openedCell = fun c -> c.state = Opened
let closedCell = fun c -> c.state = Closed

[<Test>]
let ``Open any position in field without mines should open all field`` () =
    let open' pos =
      let startPos = {x = 0; y = 0}
      let f = generateRandomField startPos {testField with mines = 0 } id
      openEmptyCellsAround f pos
    
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
let ``Open position near closed cell with bomb should open only one cell`` () =
    let startPos = {x = 7; y = 7}
    let f = generateRandomField startPos {testField with mines = 1 } (fun l -> [0])
    let positionForOpen = {x = 1; y = 1}
    let resultCells = openEmptyCellsAround f positionForOpen 
    
    resultCells |> List.filter openedCell |> should haveLength 1
    resultCells |> List.filter openedCell |> List.head |> position |> should equal positionForOpen
    resultCells |> List.filter closedCell |> should haveLength (testField.Size - 1)

let ``Open position near marked as bomb cell should open only one cell`` () =
    let startPos = {x = 7; y = 7}
    let f = generateRandomField startPos {testField with mines = 1 } (fun l -> [0])
    let positionForOpen = {x = 1; y = 1}
    let resultCells = openEmptyCellsAround f positionForOpen 
    
    resultCells |> List.filter openedCell |> should haveLength 1
    resultCells |> List.filter openedCell |> List.head |> position |> should equal positionForOpen
    resultCells |> List.filter closedCell |> should haveLength (testField.Size - 1)
    