module GenerateRandomField

open NUnit.Framework
open FsUnit
open Logic
open Types


let startPosition = {x = 1; y = 1}
let minesCount = 10

let testField = generateRandomField startPosition {width = 8; height = 8; mines = minesCount } Utils.shuffle
let hasBomb p =
    (testField.MustCell p).hasBomb

[<Test>]
let ``Set count of mines equals in the field argument`` () =
    testField.cells
    |> List.filter (_.hasBomb)
    |> should haveLength minesCount

[<Test>]
let ``Field generator should not set bomb in start position `` () =
    hasBomb startPosition |> should equal false

[<Test>]  
let ``Field generator should not set bomb around start position `` () =
    cellsAround testField startPosition
    |> List.filter (_.hasBomb)
    |> should haveLength 0

[<Test>]  
let ``Field generator should not set bomb around start position when all another cells should be with mines`` () =
    let w, h = 8, 8
    // 9 - is start position with around cells 
    let tstField = generateRandomField startPosition {width = w; height = h; mines = (w * h - 9) } Utils.shuffle
    cellsAround tstField startPosition
    |> List.filter (_.hasBomb)
    |> should haveLength 0