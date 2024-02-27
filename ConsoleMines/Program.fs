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
                let posX = input.Split " "
                let x = Int32.Parse posX[0]
                let y = Int32.Parse posX[1]
                let curPos = {x = x; y = y}
                if game.Game.isValid curPos then
                    game.Open curPos
                    match game.State with
                    | Win -> printfn $"WIN! Time: {game.Duration}"
                    | Lose -> printfn "Lose! :-("
                    | _ -> ()
                else
                    yield Some
                    
        } |> Seq.toList |> ignore
        0