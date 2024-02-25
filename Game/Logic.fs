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

let position c =
  c.pos

let recalculateField pos field =
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
  let processedCells = match cell.state with
                       | Closed | MarkAsBomb | MarkAsProbablyBomb -> []
                       | _ -> openCells [cell] [] 
  
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

let gameLoop (game: Field) (controller: GameController) =
  let calcGameState f =
    let openedCells = List.filter (fun c -> c.state = Opened) f.cells
    let hasOpenedBomb = (List.filter (_.hasBomb) openedCells).Length > 0
    match hasOpenedBomb with
    | true ->  Lose
    | false -> match f.game.Size - openedCells.Length = f.game.mines with
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
  if game.isValid startPosition then
    failwith "Incorrect position"

  let mutable field = generateRandomField startPosition game Utils.shuffle
  field <- changeCellState startPosition Opened field |> recalculateField startPosition
  
  seq {
    while true do
      let nextPosition, nextAction = controller.WaitNextStep()
      if game.isValid nextPosition then
        failwith "Incorrect position"

      let newState = match nextAction with
                     | Open -> Opened
                     | Mark -> markToState field nextPosition
       
      field <- changeCellState nextPosition newState field |> recalculateField nextPosition

      match calcGameState field with
      | Lose -> lose field; yield None
      | Win -> win field; yield None
      | InGame -> continueGame field; yield Some()

  } |> Seq.toList |> ignore

