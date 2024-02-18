module TestsMinesField

open NUnit.Framework
open FsUnit
open Types
open Logic

let testField = generateRandomField {x = 1; y = 1} {width = 8; height = 10; mines = 10 } Utils.shuffle

let cellPos p =
    (testField.MustCell p).pos

[<Test>]
let ``Should calculate correct size`` () =
    testField.Size |> should equal 80
    
[<Test>]
let ``Should calculate correct linear position`` () =
    testField.Linear {x = 0; y = 0} |> should equal 0
    testField.Linear {x = 0; y = 1} |> should equal 8
    testField.Linear {x = 7; y = 0} |> should equal 7
    testField.Linear {x = 0; y = 7} |> should equal 56
    testField.Linear {x = 7; y = 7} |> should equal 63
    testField.Linear {x = 5; y = 0} |> should equal 5
    testField.Linear {x = 7; y = 2} |> should equal 23
    testField.Linear {x = 4; y = 4} |> should equal 36
    testField.Linear {x = 1; y = 9} |> should equal 73
    testField.Linear {x = 7; y = 9} |> should equal 79

[<Test>]
let ``Should calculate correct position from linear`` () =
    testField.Position 0 |> should equal (0, 0)
    testField.Position 8 |> should equal (0, 1)
    testField.Position 7 |> should equal (7, 0)
    testField.Position 56 |> should equal (0, 7)
    testField.Position 63 |> should equal (7, 7)
    testField.Position 5 |> should equal (5, 0)
    testField.Position 23 |> should equal (7, 2)
    testField.Position 36 |> should equal (4, 4)
    testField.Position 73 |> should equal (1, 9)
    testField.Position 79 |> should equal (7, 9)

[<Test>]    
let ``Should return correct cell for position`` () =
    cellPos {x = 0; y = 0} |> should equal {x = 0; y = 0}
    cellPos {x = 7; y = 0} |> should equal {x = 7; y = 0}
    cellPos {x = 0; y = 1} |> should equal {x = 0; y = 1}
    cellPos {x = 0; y = 9} |> should equal {x = 0; y = 9}
    cellPos {x = 7; y = 9} |> should equal {x = 7; y = 9}
    cellPos {x = 4; y = 4} |> should equal {x = 4; y = 4}