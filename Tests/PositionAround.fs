module PositionAroundTests

open NUnit.Framework
open FsUnit
open Logic
open Types

let testField: Field = {width = 8; height = 8; mines = 10}

[<Test>]
let ``Top left position should return 3 positions around`` () =
    let p = {x = 0; y = 0}
    let ps = positionsAround p testField
    
    ps |> should haveLength 3
    ps |> should not' (contain p)
    
    ps |> should contain {x = 0; y = 1}
    ps |> should contain {x = 1; y = 0}
    ps |> should contain {x = 1; y = 1}
 
[<Test>]   
let ``Top middle position should return 5 positions around`` () =
    let p = {x = 4; y = 0}
    let ps = positionsAround p testField
    
    ps |> should haveLength 5
    ps |> should not' (contain p)
    
    ps |> should contain {x = 3; y = 0}
    ps |> should contain {x = 5; y = 0}
    ps |> should contain {x = 3; y = 1}
    ps |> should contain {x = 4; y = 1}
    ps |> should contain {x = 5; y = 1}
    
[<Test>]
let ``Top right position should return 3 positions around`` () =
    let p = {x = 7; y = 0}
    let ps = positionsAround p testField
    
    ps |> should haveLength 3
    ps |> should not' (contain p)
        
    ps |> should contain {x = 7; y = 1}
    ps |> should contain {x = 6; y = 0}
    ps |> should contain {x = 6; y = 1}

[<Test>]   
let ``Left middle position should return 5 positions around`` () =
    let p = {x = 0; y = 4}
    let ps = positionsAround p testField
    
    ps |> should haveLength 5
    ps |> should not' (contain p)
      
    ps |> should contain {x = 0; y = 3}
    ps |> should contain {x = 0; y = 5}
    ps |> should contain {x = 1; y = 3}
    ps |> should contain {x = 1; y = 4}
    ps |> should contain {x = 1; y = 5}

[<Test>]  
let ``Left bottom position should return 3 positions around`` () =
    let p = {x = 0; y = 7}
    let ps = positionsAround p testField
    
    ps |> should haveLength 3
    ps |> should not' (contain p)
    
    ps |> should contain {x = 0; y = 6}
    ps |> should contain {x = 1; y = 6}
    ps |> should contain {x = 1; y = 7}

[<Test>]
let ``Middle of bottom position should return 5 positions around`` () =
    let p = {x = 4; y = 7}
    let ps = positionsAround p testField
    
    ps |> should haveLength 5
    ps |> should not' (contain p)
    
    ps |> should contain {x = 3; y = 7}
    ps |> should contain {x = 3; y = 6}
    ps |> should contain {x = 4; y = 6}
    ps |> should contain {x = 5; y = 6}
    ps |> should contain {x = 5; y = 7}

[<Test>] 
let ``Right of bottom position should return 3 positions around`` () =
    let p = {x = 7; y = 7}
    let ps = positionsAround p testField
    
    ps |> should haveLength 3
    ps |> should not' (contain p)
    
    ps |> should contain {x = 6; y = 7}
    ps |> should contain {x = 6; y = 6}
    ps |> should contain {x = 7; y = 6}

[<Test>]
let ``Bottom of right position should return 5 positions around`` () =
    let p = {x = 4; y = 0}
    let ps = positionsAround p testField
    
    ps |> should haveLength 5
    ps |> should not' (contain p)
    
    ps |> should contain {x = 3; y = 0}
    ps |> should contain {x = 5; y = 0}
    ps |> should contain {x = 3; y = 1}
    ps |> should contain {x = 4; y = 1}
    ps |> should contain {x = 5; y = 1}

[<Test>]    
let ``Center position should return 8 positions around`` () =
    let p = {x = 4; y = 4}
    let ps = positionsAround p testField
    
    ps |> should haveLength 8
    ps |> should not' (contain p)
    
    ps |> should contain {x = 3; y = 3}
    ps |> should contain {x = 3; y = 4}
    ps |> should contain {x = 3; y = 5}
    ps |> should contain {x = 4; y = 3}
    ps |> should contain {x = 4; y = 5}
    ps |> should contain {x = 5; y = 3}
    ps |> should contain {x = 5; y = 4}
    ps |> should contain {x = 5; y = 5}