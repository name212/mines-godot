module Types

type CellState = Closed | Opened | MarkAsBomb | MarkAsProbablyBomb

type Position = {x: int; y: int} 

type Cell = {pos: Position; hasBomb: bool; state: CellState; bombsAround: int}

type CellWalker = Cell -> Cell

type Field = {width: int; height: int; mines: int}

type MinesField = {game: Field; cells: Cell list} with
  member f.Linear p = p.y * f.game.width + p.x
  member f.Position index =
    let x = index % f.game.width
    let y = index / f.game.width
    (x, y)
  member f.Size = f.game.height * f.game.width
  member f.Cell p = List.tryItem (f.Linear p) f.cells
  member f.MustCell p = f.cells.Item (f.Linear p)
end

type GameState = Win | Lose | InGame

type GameController =
  abstract WaitNextStep : unit -> Position * CellState
  abstract Lose: unit -> unit
  abstract Win: unit -> unit
  abstract UpdateField: MinesField -> unit
