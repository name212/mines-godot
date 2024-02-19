module internal Logic

open System.Runtime.CompilerServices
open Types

[<assembly: InternalsVisibleTo("Tests")>]
do()

let private aroundMap = [
  (-1, -1)
  (-1, 0)
  (-1, 1)
  (0, 1)
  (0, -1)
  (1, -1)
  (1, 0)
  (1, 1)
]

let positionsAround pos field: Position list =
  let toPositions sum  =
    let x, y = sum
    { x = pos.x + x; y = pos.y + y }
  let unboundPositions p = p.x >= 0 && p.y >= 0 && p.x < field.game.width && p.y < field.game.height
  aroundMap |> List.map toPositions |> List.filter unboundPositions

let cellsAround field pos: Cell list =
  let positions = positionsAround pos field
  List.map field.MustCell positions

let openEmptyCellsAround field pos =
  let processCell cell =
    let notOpened = cellsAround field cell.pos |> List.filter (fun c -> c.state <> Opened)
    let markedAsProbably = List.filter (fun c -> c.state = MarkAsProbablyBomb) notOpened
    if markedAsProbably.Length = 0 then
      let markedAsBombs = List.filter (fun c -> c.state = MarkAsBomb) notOpened
      if markedAsBombs.Length = cell.bombsAround then
        List.filter (fun c -> c.state <> MarkAsBomb && c.state <> MarkAsProbablyBomb) notOpened
      else
        []
    else  
     []
      
  let rec openCells cellsForProcess processedCells =
    let notProcessed cell =
      match List.tryFind (fun c -> c.pos = cell.pos) processedCells with
      | Some(_) -> false
      | None -> true
    
    let positionHash c = c.pos.x * 10000 + c.pos.y * 1000000
      
    let cells = cellsForProcess
                |> List.distinctBy positionHash
                |> List.filter notProcessed
    match cells with
    | [] -> processedCells
    | h :: t -> openCells ((processCell h) @ t) ([{h with state = Opened}] @ processedCells)
 
  let cell = field.MustCell pos
  let processedCells = openCells [cell] []
  
  let statesFromProcessedCells c =
    match List.tryFind (fun cc -> cc.pos = c.pos) processedCells with
    | Some(x) -> x
    | None -> c
  
  List.map statesFromProcessedCells field.cells

let changeCellState field pos newState : MinesField =
   let buildNewField c =
     let changeStateForActedCell c = 
       match pos <> c.pos with
       | true -> c
       | false -> { c with state = newState }
     let cellsAfterChangeState = List.map changeStateForActedCell field.cells
     let cells = match (c.state, newState) with
                 | (Closed, Opened) | (Opened, Opened) -> openEmptyCellsAround { game = field.game; cells = cellsAfterChangeState } c.pos
                 | (_, _) -> cellsAfterChangeState
     {game = field.game; cells = cells }

   let hasCell c =
     match (c.state, newState) with
     | (Opened, MarkAsBomb ) -> buildNewField c
     | (MarkAsBomb, MarkAsProbablyBomb) -> buildNewField c
     | (MarkAsProbablyBomb, Opened) -> buildNewField c 
     | (MarkAsProbablyBomb, Closed) -> buildNewField c
     | (Closed, Opened) -> buildNewField c 
     | (_, _) -> field
     
   match field.Cell pos with
   | Some(c) -> hasCell c
   | None -> failwith "Incorrect position"

let generateRandomField (startPosition: Position) (game: Field) randomize : MinesField =
  let field = {game = game; cells = [] }
  let excludedPositions = positionsAround startPosition field @ [startPosition]
  let excludedLinearPositions = List.map field.Linear excludedPositions
  let bombs = [0 .. field.Size - 1]
              |> List.except excludedLinearPositions
              |> randomize
              |> List.take game.mines
  let linearToCell p =
    let isBomb = List.exists (fun pp -> pp = p) bombs
    let x, y = field.Position p
    {pos = { x = x; y = y }; hasBomb = isBomb; state = Closed; bombsAround = 0 }
  let cells = [0 .. field.Size - 1]
              |> List.map linearToCell
  
  let fieldWithCells = { game = game; cells = cells }
  let setBombsAround cell =
    let cells = cellsAround  fieldWithCells cell.pos
    let bombs = (List.filter (fun c -> c.hasBomb) cells).Length
    {cell with bombsAround =  bombs}
  
  let finalCells = List.map setBombsAround fieldWithCells.cells
  {game = game; cells = finalCells }

let gameLoop (game: Field) (controller: GameController) =
  let calcGameState f =
    let openedCells = List.filter (fun c -> c.state = Opened) f.cells
    let hasOpenedBomb = (List.filter (_.hasBomb) openedCells).Length > 0
    match hasOpenedBomb with
    | true ->  Lose
    | false -> match f.Size - openedCells.Length = f.game.mines with
               | true -> Win
               | false -> InGame
  
  let lose field =
    controller.UpdateField field
    controller.Lose()
    
  let win field =
    controller.UpdateField field
    controller.Win()
  
  let continueGame field =
    controller.UpdateField field
               
  let startPosition, _ = controller.WaitNextStep()
  let mutable field = generateRandomField startPosition game Utils.shuffle      
  field <- changeCellState field startPosition Opened
  
  seq {
    while true do
      let nextPosition, newState = controller.WaitNextStep()
      field <- changeCellState field nextPosition newState
      match calcGameState field with
      | Lose -> lose field; yield None
      | Win -> win field; yield None
      | InGame -> continueGame field; yield Some()
  } |> Seq.toList |> ignore
  
  ()

