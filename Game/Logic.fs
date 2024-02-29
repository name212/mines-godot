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

let positionsAround pos (field: Field): Position list =
  let toPositions sum  =
    let x, y = sum
    { x = pos.x + x; y = pos.y + y }
  aroundMap |> List.map toPositions |> List.filter field.isValid

let cellsAround field pos: Cell list =
  let positions = positionsAround pos field.game
  List.map field.MustCell positions

let recalculateField pos (field: MinesField) =
  let cellProcessor (cellsForProcess: Cell list) (processedCells: Cell list) =
    let processCell = cellsForProcess.Head
    let cellsAround = cellsAround field processCell.pos
    let notProcessed cell =
      match List.tryFind (fun c -> c.pos = cell.pos) processedCells with
      | Some(_) -> false
      | None -> true
      
    let open' cell =
      {cell with state = Opened}
    
    let positionHash c = c.pos.x * 10000 + c.pos.y * 1000000
      
    let notOpenedCells = cellsAround
                         |> List.distinctBy positionHash
                         |> List.filter notProcessed
                         |> List.filter (fun c -> c.state <> Opened)
    let markedAsBombs = notOpenedCells |> List.filter (fun c -> c.state = MarkAsBomb)
    let incorrectMarked = markedAsBombs |> List.filter (fun c -> not c.hasBomb)
    
    if incorrectMarked.Length = 0 then
      if markedAsBombs.Length <> processCell.bombsAround then
        // not all marked, open current cell and return
        ([], [open' processCell] @ processedCells)
      else
        // all right, open not opened cells and add to process all empty cells
        let openedCells = notOpenedCells
                           |> List.filter (fun c -> c.state <> MarkAsBomb)
                           |> List.map open'
        let emptyCells = openedCells |> List.filter (fun c -> c.bombsAround = 0)
        let toAddForProcessed = openedCells |> List.except emptyCells
        (emptyCells @ cellsForProcess.Tail, [open' processCell] @ toAddForProcessed @ processedCells )
    else
      // have none bomb cell marked as bomb open all cells for finish game (cells with bomb will be opened)
      let openedCells = notOpenedCells |> List.map open'
      ([], openedCells @ processedCells)
    
  let rec actor (cellsForProcess: Cell list, processedCells: Cell list) =
    match cellsForProcess with
    | [] -> processedCells
    | l -> actor (cellProcessor l processedCells)
    
  let cell = field.MustCell pos
  let processedCells = match cell.state with
                       | Closed | MarkAsBomb | MarkAsProbablyBomb -> []
                       | _ -> actor ([cell], [])
  
  let applyStatesFromProcessedCells c =
    match List.tryFind (fun cc -> cc.pos = c.pos) processedCells with
    | Some(x) -> x
    | None -> c
  
  let finalCells = List.map applyStatesFromProcessedCells field.cells
  {game = field.game; cells = finalCells }

let changeCellState pos newState field =
  let buildNewField =
    let changeStateForActedCell c = 
      match pos <> c.pos with
      | true -> c
      | false -> { c with state = newState }
    let cellsAfterChangeState = List.map changeStateForActedCell field.cells
    { game = field.game; cells = cellsAfterChangeState }
    
  let cell = field.MustCell pos
  if cell.state <> newState then
    buildNewField
  else
    field

let generateRandomField (startPosition: Position) (game: Field) randomize : MinesField =
  let excludedPositions = positionsAround startPosition game @ [startPosition]
  let excludedLinearPositions = List.map game.Linear excludedPositions
  let bombs = [0 .. game.Size - 1]
              |> List.except excludedLinearPositions
              |> randomize
              |> List.take game.mines
  let linearToCell p =
    let isBomb = List.exists (fun pp -> pp = p) bombs
    let x, y = game.Position p
    {pos = { x = x; y = y }; hasBomb = isBomb; state = Closed; bombsAround = 0 }
  let cells = [0 .. game.Size - 1]
              |> List.map linearToCell
  
  let fieldWithCells = { game = game; cells = cells }
  let setBombsAround cell =
    let cells = cellsAround  fieldWithCells cell.pos
    let bombs = (List.filter (fun c -> c.hasBomb) cells).Length
    {cell with bombsAround =  bombs}
  
  let finalCells = List.map setBombsAround fieldWithCells.cells
  {game = game; cells = finalCells }

let markToState (field: MinesField) position =
  let c = field.MustCell position
  match c.state with
  | Closed -> MarkAsBomb
  | MarkAsBomb -> MarkAsProbablyBomb
  | MarkAsProbablyBomb -> Closed
  | Opened -> Opened

let calcGameState f =
  let openedCells = List.filter (fun c -> c.state = Opened) f.cells
  let hasOpenedBomb = (List.filter (_.hasBomb) openedCells).Length > 0
  match hasOpenedBomb with
  | true ->  Lose
  | false -> match f.game.Size - openedCells.Length = f.game.mines with
             | true -> Win
             | false -> InGame
