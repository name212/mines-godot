open System
open Types
open mines

[<EntryPoint>]
let main argv =
    if argv.Length < 3 then
        failwith "Should width height mines"
        1
    else
        let widthStr, heightStr, minesStr = argv[0], argv[1], argv[2]
        let width = Int32.Parse widthStr
        let height = Int32.Parse heightStr
        let bombs = Int32.Parse minesStr
        let game = Mines(width, height, bombs, fun () -> DateTime.UtcNow)
        seq {
            while true do
                printf "Enter position x y: "
                let input = Console.ReadLine()
                let splitInput = input.Split " " |> List.ofArray
                let pos, action = match splitInput with
                                    | [_] -> None, game.Open
                                    | [x; y] -> Some(x, y), game.Open
                                    | [x; y; _] -> Some(x, y), game.Mark
                                    | _ -> None, game.Open
                if pos.IsSome then
                    let xStr, yStr = pos.Value
                    let parsedPos = try
                                        let x = Int32.Parse xStr
                                        let y = Int32.Parse yStr
                                        {x = x; y = y}
                                    with
                                    | ex -> {x = -1; y = -1} 
                    if game.Game.isValid parsedPos then
                        action parsedPos
                        match game.State with
                        | Win -> printfn $"WIN! Time: {game.Duration} s"; yield None
                        | Lose -> printfn "Lose! :-("; yield None
                        | _ -> printfn $"%s{Debug.logField game.Field.Value}" ; yield Some()
                    else
                        printfn "Incorrect position"
                        yield Some()
                    
        } |> Seq.takeWhile (_.IsSome) |> Seq.toList |> ignore
        0