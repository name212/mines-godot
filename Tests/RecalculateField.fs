module ``Func recalculateField``

open NUnit.Framework
open FsUnit
open Logic
open Types

let testField: Field = {width = 8; height = 8; mines = 0}
let openedCell = fun c -> c.state = Opened
let closedCell = fun c -> c.state = Closed
let markedAsBombCell = fun c -> c.state = MarkAsBomb

[<Test>]
let ``should open all field when open any position in field without mines `` () =
    let open' pos =
      let startPos = {x = 0; y = 0}
      let f = generateRandomField startPos {testField with mines = 0 } id
      let newField = changeCellState pos Opened f |> recalculateField pos
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
    let resultField = changeCellState positionForOpen Opened f |> recalculateField positionForOpen 
    
    resultField.cells |> List.filter openedCell |> should haveLength 1
    resultField.cells |> List.filter openedCell |> List.head |> position |> should equal positionForOpen
    resultField.cells |> List.filter closedCell |> should haveLength (testField.Size - 1)

[<Test>]
let ``should open all field with one bomb when cell with bomb marked as bomb correctly`` () =
    let startPos = {x = 7; y = 7}
    let bombPos = {x = 1; y = 1}
    let positionForOpen = {x = 1; y = 0}
    let f1 = generateRandomField startPos {testField with mines = 1 } (fun l -> [testField.Linear bombPos])
    let resultField = changeCellState bombPos MarkAsBomb f1
                      |> recalculateField bombPos
                      |> changeCellState positionForOpen Opened
                      |> recalculateField positionForOpen

    resultField.cells |> List.filter openedCell |> should haveLength (testField.Size - 1)
    resultField.cells |> List.filter (fun c -> c.state = MarkAsBomb) |> should haveLength 1
 
 // next tests operate with real field
 // _ _ 1 v v v v v      _ _ 1 * 1 v v v      _ _ 1 * o 1 v v     _ _ 1 * o 1 v v     _ _ 1 * 1 1 v v     _ _ 1 * 1 1 v v
 // _ o 1 1 v v v v      _ _ 1 o 2 v v v      _ _ 1 1 2 2 v v     _ _ 1 1 2 2 v v     _ _ 1 1 2 2 v v     _ _ 1 1 2 2 v v
 // _ _ _ 1 v v v v      _ _ _ 1 2 v v v      _ _ _ 1 2 v v v     _ _ _ 1 2 * v v     _ _ _ 1 2 * v v     _ _ _ 1 2 * 2 v
 // _ _ _ 1 v v v v      _ _ _ 1 v v v v      _ _ _ 1 v v v v     _ _ _ 1 * v v v     _ _ _ 1 * 2 v v     _ _ _ 1 * o 1 v
 // _ _ _ 1 1 v v v -->  _ _ _ 1 1 v v v -->  _ _ _ 1 1 v v v --> _ _ _ 1 1 v v v --> _ _ _ 1 o 2 v v --> _ _ _ 1 1 2 1 v -->
 // _ _ _ _ 1 v v v      _ _ _ _ 1 v v v      _ _ _ _ 1 v v v     _ _ _ _ 1 v v v     _ _ _ _ 1 3 v v     _ _ _ _ 1 3 v v
 // 1 2 2 1 1 v v v      1 2 2 1 1 v v v      1 2 2 1 1 v v v     1 2 2 1 1 v v v     1 2 2 1 1 v v v     1 2 2 1 1 v v v
 // v v v v v v v v      v v v v v v v v      v v v v v v v v     v v v v v v v v     v v v v v v v v     v v v v v v v v
 ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 // _ _ 1 * 1 1 v v     _ _ 1 * 1 1 v v      _ _ 1 * 1 1 v v     _ _ 1 * 1 1 v v     _ _ 1 * 1 1 v v     _ _ 1 * 1 1 v v
 // _ _ 1 1 2 2 v v     _ _ 1 1 2 2 v v      _ _ 1 1 2 2 v v     _ _ 1 1 2 2 v v     _ _ 1 1 2 2 v v     _ _ 1 1 2 2 v v
 // _ _ _ 1 2 * 2 1     _ _ _ 1 2 * 2 1      _ _ _ 1 2 * 2 1     _ _ _ 1 2 * 2 1     _ _ _ 1 2 * 2 1     _ _ _ 1 2 * 2 1
 // _ _ _ 1 * 2 o _     _ _ _ 1 * 2 1 _      _ _ _ 1 * 2 1 _     _ _ _ 1 * 2 1 _     _ _ _ 1 * 2 1 _     _ _ _ 1 * 2 1 _
 // _ _ _ 1 1 2 1 1 --> _ _ _ 1 1 2 o 1 -->  _ _ _ 1 1 2 1 1 --> _ _ _ 1 1 2 1 1 --> _ _ _ 1 1 2 1 1 --> _ _ _ 1 1 2 1 1 -->
 // _ _ _ _ 1 3 v v     _ _ _ _ 1 3 * 3      _ _ _ _ 1 3 * 3     _ _ _ _ 1 3 * 3     _ _ _ _ 1 3 * 3     _ _ _ _ 1 3 * 3
 // 1 2 2 1 1 v v v     1 2 2 1 1 * * v      1 2 2 o 1 * * *     1 o 2 1 1 * * *     1 2 2 1 1 * * *     1 2 2 1 1 * * *
 // v v v v v v v v     v v v v v v v v      v * * 1 1 v v v     1 * * 1 1 v v v     1 * * 1 1 v v v     1 * * 1 1 v 3 v
 ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 // _ _ 1 * 1 1 v v     _ _ 1 * 1 1 v v --> _ _ 1 * 1 1 v 1     _ _ 1 * 1 1 1 1
 // _ _ 1 1 2 2 v v     _ _ 1 1 2 2 v v --> _ _ 1 1 2 2 v v     _ _ 1 1 2 2 * 1
 // _ _ _ 1 2 * 2 1     _ _ _ 1 2 * 2 1 --> _ _ _ 1 2 * 2 1     _ _ _ 1 2 * 2 1
 // _ _ _ 1 * 2 1 _     _ _ _ 1 * 2 1 _ --> _ _ _ 1 * 2 1 _     _ _ _ 1 * 2 1 _
 // _ _ _ 1 1 2 1 1 --> _ _ _ 1 1 2 1 1 --> _ _ _ 1 1 2 1 1 --> _ _ _ 1 1 2 1 1 
 // _ _ _ _ 1 3 * 3     _ _ _ _ 1 3 * 3 --> _ _ _ _ 1 3 * 3     _ _ _ _ 1 3 * 3
 // 1 2 2 1 1 * * *     1 2 2 1 1 * * * --> 1 2 2 1 1 * * *     1 2 2 1 1 * * *
 // 1 * * 1 1 2 o 2     1 * * 1 1 2 o 2 --> 1 * * 1 1 2 3 2     1 * * 1 1 2 3 2
 // ////////////////
 // _ _ 1 * 1 1 1 1
 // _ _ 1 1 2 2 * 1
 // _ _ _ 1 2 * 2 1
 // _ _ _ 1 * 2 1 _
 // _ _ _ 1 1 2 1 1
 // _ _ _ _ 1 3 * 3
 // 1 2 2 1 1 * * *
 // 1 * * 1 1 2 3 2
let bombsList = [
    testField.Linear {x = 3; y = 0}
    testField.Linear {x = 6; y = 1}
    testField.Linear {x = 5; y = 2}
    testField.Linear {x = 4; y = 3}
    testField.Linear {x = 6; y = 5}
    testField.Linear {x = 5; y = 6}
    testField.Linear {x = 6; y = 6}
    testField.Linear {x = 7; y = 6}
    testField.Linear {x = 1; y = 7}
    testField.Linear {x = 2; y = 7}
 ]
let testField2 = generateRandomField {x = 1; y = 1} {testField with mines = 10 } (fun l -> bombsList)
let firstStepOpened = [
        { x = 0; y = 0 }
        { x = 1; y = 0 }
        { x = 2; y = 0 }
        
        { x = 0; y = 1 }
        { x = 1; y = 1 }
        { x = 2; y = 1 }
        { x = 3; y = 1 }
        
        { x = 0; y = 2 }
        { x = 1; y = 2 }
        { x = 2; y = 2 }
        { x = 3; y = 2 }
               
        { x = 0; y = 3 }
        { x = 1; y = 3 }
        { x = 2; y = 3 }
        { x = 3; y = 3 }
        
        { x = 0; y = 4 }
        { x = 1; y = 4 }
        { x = 2; y = 4 }
        { x = 3; y = 4 }
        { x = 4; y = 4 }
        
        { x = 0; y = 5 }
        { x = 1; y = 5 }
        { x = 2; y = 5 }
        { x = 3; y = 5 }
        { x = 4; y = 5 }
        
        { x = 0; y = 6 }
        { x = 1; y = 6 }
        { x = 2; y = 6 }
        { x = 3; y = 6 }
        { x = 4; y = 6 }
    ]

let in' lst cell =
    match List.tryFind (fun p -> p = cell.pos) lst with
    | None -> false
    | Some(_) -> true

let notIn lst cell = not (in' lst cell)

[<Test>]
let ``should open correct cells on first step in normally generated field`` () =
    let positionForOpen = {x = 1; y = 1}
    let resultCells = changeCellState positionForOpen Opened testField2 |> recalculateField positionForOpen
    
    let inOpened = in' firstStepOpened
    let notInOpened = notIn firstStepOpened    
    
    resultCells.cells |> List.filter inOpened |> List.filter openedCell |> should haveLength (firstStepOpened.Length)
    resultCells.cells |> List.filter notInOpened |> List.filter closedCell |> should haveLength (resultCells.game.Size - firstStepOpened.Length)

[<Test>]
let ``should not open any cells when retry open opened without marked as bombs around and without bomb`` () =
    let positionForOpen = {x = 1; y = 1}
    let resultCells = changeCellState positionForOpen Opened testField2
                      |> recalculateField positionForOpen
                      |> recalculateField positionForOpen
    
    let inOpened = in' firstStepOpened
    let notInOpened = notIn firstStepOpened    
    
    resultCells.cells |> List.filter inOpened |> List.filter openedCell |> should haveLength (firstStepOpened.Length)
    resultCells.cells |> List.filter notInOpened |> List.filter closedCell |> should haveLength (resultCells.game.Size - firstStepOpened.Length)

[<Test>]
let ``should not open any cells when open opened without marked as bombs around`` () =
    let positionForOpen = {x = 1; y = 1}
    let resultCells = changeCellState positionForOpen Opened testField2
                      |> recalculateField positionForOpen
                      |> recalculateField { x = 2; y = 1 }
    
    let inOpened = in' firstStepOpened
    let notInOpened = notIn firstStepOpened    
    
    resultCells.cells |> List.filter inOpened |> List.filter openedCell |> should haveLength (firstStepOpened.Length)
    resultCells.cells |> List.filter notInOpened |> List.filter closedCell |> should haveLength (resultCells.game.Size - firstStepOpened.Length)

[<Test>]    
let ``should not open any cells when cell marked as bomb`` () =
    let startPosition = {x = 1; y = 1}
    let resultCells = changeCellState startPosition Opened testField2 
                      |> recalculateField startPosition
                      |> changeCellState { x = 4; y = 3 } MarkAsBomb
                      |> recalculateField { x = 4; y = 3 }
    
    let inOpened = in' firstStepOpened
    let notInOpened = notIn firstStepOpened    
    
    resultCells.cells
    |> List.filter inOpened
    |> List.filter openedCell
    |> should haveLength (firstStepOpened.Length)
    
    resultCells.cells
    |> List.filter notInOpened
    |> List.filter closedCell
    |> should haveLength (resultCells.game.Size - firstStepOpened.Length - 1)
    
    resultCells.cells
    |> List.filter markedAsBombCell
    |> should haveLength 1

[<Test>]    
let ``should open not open cells when cells marked as probably bomb`` () =
    let startPosition = {x = 1; y = 1}
    let resultCells = changeCellState startPosition Opened testField2 
                      |> recalculateField startPosition
                      |> changeCellState { x = 3; y = 0} MarkAsProbablyBomb
                      |> recalculateField { x = 3; y = 0 }
                      |> changeCellState { x = 3; y = 1 } Opened
                      |> recalculateField { x = 3; y = 1 }
    
    let inOpened = in' firstStepOpened
    let notInOpened = notIn firstStepOpened    
    
    resultCells.cells
    |> List.filter inOpened
    |> List.filter openedCell
    |> should haveLength (firstStepOpened.Length)
    
    resultCells.cells
    |> List.filter notInOpened
    |> List.filter closedCell
    |> should haveLength (resultCells.game.Size - firstStepOpened.Length - 1)
    
    resultCells.cells
    |> List.filter (fun c -> c.state = MarkAsProbablyBomb)
    |> should haveLength 1

[<Test>]    
let ``should open nearly cells for marked as bomb`` () =
    let secondOpenedCells = firstStepOpened @ [
        {x = 4; y = 0}
        {x = 4; y = 1}
        {x = 4; y = 2}
    ]
    let startPosition = {x = 1; y = 1}
    let resultCells = changeCellState startPosition Opened testField2 
                      |> recalculateField startPosition
                      |> changeCellState { x = 3; y = 0} MarkAsBomb
                      |> recalculateField { x = 3; y = 0 }
                      |> changeCellState { x = 3; y = 1 } Opened
                      |> recalculateField { x = 3; y = 1 }
    
    let inOpened = in' firstStepOpened
    let notInOpened = notIn firstStepOpened    
    
    resultCells.cells
    |> List.filter inOpened
    |> List.filter openedCell
    |> should haveLength (secondOpenedCells.Length)
    
    resultCells.cells
    |> List.filter notInOpened
    |> List.filter closedCell
    |> should haveLength (resultCells.game.Size - secondOpenedCells.Length - 1)
    
    resultCells.cells
    |> List.filter (fun c -> c.state = MarkAsBomb)
    |> should haveLength 1