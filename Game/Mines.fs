namespace mines

open System
open Types
open System.Collections.Generic

type public NowFunc = unit -> DateTime

type public Timer = interface
    abstract member Now : unit -> DateTime
end

type public StdTimer() =
    interface Timer with
        member this.Now() = DateTime.Now 
    end

type public Mines(width, height, bombs, timer: Timer) =
    let game = {width = width; height = height; mines = bombs }
    let EmptyField = Logic.generateEmptyField game
    let mutable field: MinesField option = None
    
    let mutable duration = 0.0
    let mutable lastPause: DateTime option = None
    let mutable gameState: GameState = InGame
    
    let resume nowTime =
        lastPause <- Some(nowTime)
        gameState <- InGame
        
    let totalDuration nowTime =
        (nowTime - lastPause.Value).TotalSeconds + duration
        
    let pause nowTime =
        duration <- totalDuration nowTime
        lastPause <- None
        gameState <- Paused
    
    let open' pos state =
        let f = Logic.changeCellState pos state field.Value |> Logic.recalculateField pos
        
        let onWin t fld =
            pause t
            let fl = Logic.changeBombsCellsStateTo MarkAsBomb fld
            Some(fl)
        let onLose t fld =
            pause t
            let fl = Logic.changeBombsCellsStateTo Opened fld
            Some(fl)
   
        gameState <- Logic.calcGameState f
        let t = timer.Now()
        field <- match gameState with
                 | Win -> onWin t f
                 | Lose ->  onLose t f
                 | _ -> Some(f)
    
    let startGame pos =
        if not (game.isValid pos) then
            failwith "Incorrect position"
        field <- Some(Logic.generateRandomField pos game Utils.shuffle)
        lastPause <- Some(timer.Now())
        duration <- 0.0
        open' pos Opened
    
    member this.Game = game
    member this.Field with get() =
        match field with
        | None -> EmptyField
        | Some(f) -> f

    member this.State with get() = gameState
    
    member this.Duration with get() =
        match lastPause with
        | None -> duration
        | Some _ -> totalDuration (timer.Now())
    
    member this.Open pos =
        match gameState with
        | Win | Lose | Paused -> ()
        | _ -> match field with
               | None -> startGame pos
               | Some _ -> open' pos Opened
    
    member this.Mark pos =
        match gameState with
        | Win | Lose | Paused -> ()
        | _ -> match field with
               | None -> ()
               | Some f ->  open' pos (Logic.markToState f pos)
        
    member this.PauseOrResume () =
        let pauseOrResume =
            let nowTime = timer.Now()
            if field.IsSome then
                match lastPause with
                | None -> resume nowTime
                | Some _ -> pause nowTime
        
        match gameState with
        | Win | Lose -> ()
        | _ -> pauseOrResume
    
module public Debug =
    let logField (field: MinesField) =
        let printOpened c =
          if c.hasBomb then
            "*"
          else
            if c.bombsAround = 0 then
              "_"
            else
              c.bombsAround.ToString()
        let header = "  | " + ([0 .. field.game.width - 1] |> List.map (fun i -> i.ToString()) |> String.concat "  ")
        let mutable s = [
           header;
           [1 .. header.Length] |> List.map (fun _ -> "-") |> String.concat ""   
        ] 
        for y in [ 0 .. field.game.height - 1] do
          let arr = new List<string>()
          for x in [0 .. field.game.width - 1] do
            let cell = field.MustCell {x = x; y = y}
            let sym = match cell.state with
                      | Opened -> printOpened cell
                      | Closed -> "#"
                      | MarkAsBomb -> "!"
                      | MarkAsProbablyBomb -> "?"
            arr.Add sym
          s <- s @ [
              y.ToString() + " | " + (String.concat "  " arr)
          ]
        
        String.concat "\n" s
        