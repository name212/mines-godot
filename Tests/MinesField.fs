module Field

open NUnit.Framework
open FsUnit
open Types
open Logic

let field = {width = 8; height = 10; mines = 10 }

let testMinesField = generateRandomField {x = 1; y = 1} field Utils.shuffle

let cellPos p =
    (testMinesField.MustCell p).pos

[<Test>]
let ``Should calculate correct size`` () =
    testMinesField.game.Size |> should equal 80
    
[<Test>]
let ``should calculate correct linear position`` () =
    field.Linear {x = 0; y = 0} |> should equal 0
    field.Linear {x = 0; y = 1} |> should equal 8
    field.Linear {x = 7; y = 0} |> should equal 7
    field.Linear {x = 0; y = 7} |> should equal 56
    field.Linear {x = 7; y = 7} |> should equal 63
    field.Linear {x = 5; y = 0} |> should equal 5
    field.Linear {x = 7; y = 2} |> should equal 23
    field.Linear {x = 4; y = 4} |> should equal 36
    field.Linear {x = 1; y = 9} |> should equal 73
    field.Linear {x = 7; y = 9} |> should equal 79

[<Test>]
let ``should calculate correct position from linear`` () =
    field.Position 0 |> should equal (0, 0)
    field.Position 8 |> should equal (0, 1)
    field.Position 7 |> should equal (7, 0)
    field.Position 56 |> should equal (0, 7)
    field.Position 63 |> should equal (7, 7)
    field.Position 5 |> should equal (5, 0)
    field.Position 23 |> should equal (7, 2)
    field.Position 36 |> should equal (4, 4)
    field.Position 73 |> should equal (1, 9)
    field.Position 79 |> should equal (7, 9)

[<Test>]    
let ``should return correct cell for position`` () =
    cellPos {x = 0; y = 0} |> should equal {x = 0; y = 0}
    cellPos {x = 7; y = 0} |> should equal {x = 7; y = 0}
    cellPos {x = 0; y = 1} |> should equal {x = 0; y = 1}
    cellPos {x = 0; y = 9} |> should equal {x = 0; y = 9}
    cellPos {x = 7; y = 9} |> should equal {x = 7; y = 9}
    cellPos {x = 4; y = 4} |> should equal {x = 4; y = 4}