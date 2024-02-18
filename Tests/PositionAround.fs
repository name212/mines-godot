module Tests

open NUnit.Framework
open FsUnit
open Logic
open Types

let testField: Field = {width = 8; height = 8; mines = 10}

let testMinesField = generateRandomField {x = 0; y = 0} testField id

[<Test>]
let ``Top left position should return 3 positions around`` () =
    let ps = positionsAround {x = 0; y = 0} testMinesField
    ps |> should haveLength 3
    
let ``Top middle position should return 5 positions around`` () =
    let ps = positionsAround {x = 4; y = 0} testMinesField
    ps |> should haveLength 5
    
let ``Top right position should return 3 positions around`` () =
    let ps = positionsAround {x = 7; y = 0} testMinesField
    ps |> should haveLength 3
    
let ``Left middle position should return 5 positions around`` () =
    let ps = positionsAround {x = 0; y = 4} testMinesField
    ps |> should haveLength 5
    
let ``Left bottom position should return 3 positions around`` () =
    let ps = positionsAround {x = 0; y = 7} testMinesField
    ps |> should haveLength 3

let ``Middle of bottom position should return 5 positions around`` () =
    let ps = positionsAround {x = 4; y = 7} testMinesField
    ps |> should haveLength 5
    
let ``Right of bottom position should return 3 positions around`` () =
    let ps = positionsAround {x = 7; y = 7} testMinesField
    ps |> should haveLength 3

let ``Bottom of right position should return 5 positions around`` () =
    let ps = positionsAround {x = 7; y = 4} testMinesField
    ps |> should haveLength 5
    
let ``Center position should return 8 positions around`` () =
    let ps = positionsAround {x = 4; y = 4} testMinesField
    ps |> should haveLength 8