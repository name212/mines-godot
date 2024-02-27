module ``Func generateRandomField``

open NUnit.Framework
open FsUnit
open Logic
open Types


let startPosition = {x = 1; y = 1}
let minesCount = 10

let testField = generateRandomField startPosition {width = 8; height = 8; mines = minesCount } Utils.shuffle
let hasBomb p =
    (testField.MustCell p).hasBomb
    
let bombsCount c =
    c.bombsAround

[<Test>]
let ``should set mines to equal in the field argument`` () =
    testField.cells
    |> List.filter (_.hasBomb)
    |> should haveLength minesCount

[<Test>]
let ``should not set bomb in start position `` () =
    hasBomb startPosition |> should equal false

[<Test>]  
let ``should not set bomb around start position `` () =
    cellsAround testField startPosition
    |> List.filter (_.hasBomb)
    |> should haveLength 0

[<Test>]  
let ``should not set bomb around start position when all another cells should be with mines`` () =
    let w, h = 8, 8
    // 9 - is start position with around cells 
    let tstField = generateRandomField startPosition {width = w; height = h; mines = (w * h - 9) } Utils.shuffle
    cellsAround tstField startPosition
    |> List.filter (_.hasBomb)
    |> should haveLength 0

[<Test>]
let ``should set count bombs equal 1 around bomb cell`` () =
    let field = {width = 8; height = 8; mines = 1 }
    let posWithBomb = { x = 1; y = 6 }
    let tstField = generateRandomField startPosition field (fun l -> [field.Linear posWithBomb])
    cellsAround tstField posWithBomb 
    |> List.filter (fun c -> c.bombsAround = 1)
    |> should haveLength 8

[<Test>]
let ``should set count bombs equal 2 for cell with around 2 bomb`` () =
    let field = {width = 8; height = 8; mines = 2 }
    let positionsWithBombs = [
        field.Linear { x = 1; y = 6 };
        field.Linear { x = 2; y = 6 };
    ]
    let tstField = generateRandomField startPosition field (fun l -> positionsWithBombs)
    tstField.MustCell {x = 1; y = 7} |> bombsCount |> should equal 2  
    tstField.MustCell {x = 2; y = 7} |> bombsCount |> should equal 2  
    tstField.MustCell {x = 1; y = 5} |> bombsCount |> should equal 2  
    tstField.MustCell {x = 2; y = 5} |> bombsCount |> should equal 2  

[<Test>]
let ``should set count bombs equal 3 for cell with around 3 bomb`` () =
    let field = {width = 8; height = 8; mines = 3 }
    let positionsWithBombs = [
        field.Linear { x = 0; y = 6 };
        field.Linear { x = 1; y = 6 };
        field.Linear { x = 2; y = 6 };
    ]
    let tstField = generateRandomField startPosition field (fun l -> positionsWithBombs)
    tstField.MustCell {x = 1; y = 7} |> bombsCount |> should equal 3  
    tstField.MustCell {x = 1; y = 5} |> bombsCount |> should equal 3

[<Test>]
let ``should set count bombs equal 4 for cell with around 3 bomb`` () =
    let field = {width = 8; height = 8; mines = 4 }
    let positionsWithBombs = [
        field.Linear { x = 0; y = 6 };
        field.Linear { x = 0; y = 7 };
        field.Linear { x = 1; y = 6 };
        field.Linear { x = 2; y = 6 };
    ]
    let tstField = generateRandomField startPosition field (fun l -> positionsWithBombs)
    tstField.MustCell {x = 1; y = 7} |> bombsCount |> should equal 4 

[<Test>]
let ``should set count bombs equal 5 for cell with around 5 bomb`` () =
    let field = {width = 8; height = 8; mines = 5 }
    let positionsWithBombs = [
        field.Linear { x = 0; y = 6 };
        field.Linear { x = 0; y = 7 };
        field.Linear { x = 1; y = 6 };
        field.Linear { x = 2; y = 6 };
        field.Linear { x = 2; y = 7 };
    ]
    let tstField = generateRandomField startPosition field (fun l -> positionsWithBombs)
    tstField.MustCell {x = 1; y = 7} |> bombsCount |> should equal 5 

[<Test>]
let ``should set count bombs equal 6 for cell with around 6 bomb`` () =
    let field = {width = 8; height = 8; mines = 6 }
    let positionsWithBombs = [
        field.Linear { x = 0; y = 5 };
        field.Linear { x = 0; y = 6 };
        field.Linear { x = 0; y = 7 };
        field.Linear { x = 1; y = 5 };
        field.Linear { x = 2; y = 5 };
        field.Linear { x = 1; y = 7 };
    ]
    let tstField = generateRandomField startPosition field (fun l -> positionsWithBombs)
    tstField.MustCell {x = 1; y = 6} |> bombsCount |> should equal 6 

[<Test>]
let ``should set count bombs equal 7 for cell with around 7 bomb`` () =
    let field = {width = 8; height = 8; mines = 7 }
    let positionsWithBombs = [
        field.Linear { x = 0; y = 5 };
        field.Linear { x = 0; y = 6 };
        field.Linear { x = 0; y = 7 };
        field.Linear { x = 1; y = 5 };
        field.Linear { x = 2; y = 5 };
        field.Linear { x = 1; y = 7 };
        field.Linear { x = 2; y = 7 };
    ]
    let tstField = generateRandomField startPosition field (fun l -> positionsWithBombs)
    tstField.MustCell {x = 1; y = 6} |> bombsCount |> should equal 7

[<Test>]
let ``should set count bombs equal 8 for cell with around 8 bomb`` () =
    let field = {width = 8; height = 8; mines = 8 }
    let positionsWithBombs = [
        field.Linear { x = 0; y = 5 };
        field.Linear { x = 0; y = 6 };
        field.Linear { x = 2; y = 6 };
        field.Linear { x = 0; y = 7 };
        field.Linear { x = 1; y = 5 };
        field.Linear { x = 2; y = 5 };
        field.Linear { x = 1; y = 7 };
        field.Linear { x = 2; y = 7 };
    ]
    let tstField = generateRandomField startPosition field (fun l -> positionsWithBombs)
    tstField.MustCell {x = 1; y = 6} |> bombsCount |> should equal 8
