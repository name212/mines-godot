module Types

type CellState = Closed | Opened | MarkAsBomb | MarkAsProbablyBomb

type Position = {x: int; y: int} 

type Cell = {pos: Position; hasBomb: bool; state: CellState; bombsAround: int}

type CellWalker = Cell -> Cell

type Field = {width: int; height: int; mines: int} with
    member f.Size = f.height * f.width
    member f.isValid position = position.x >= 0 && position.y >= 0 && position.x < f.width && position.y < f.height
    member f.Linear p = p.y * f.width + p.x
    member f.Position index =
      let x = index % f.width
      let y = index / f.width
      (x, y)
end

type MinesField = {game: Field; cells: Cell list} with
  member f.Cell linearPos = f.cells.Item linearPos
  member f.MustCell p = f.cells.Item (f.game.Linear p)
  member f.MarkedAsBombsCount () = (f.cells |> List.filter (fun c -> c.state = MarkAsBomb)).Length
end

type GameState = Win | Lose | InGame | Paused

type GameAction = Open | Mark
