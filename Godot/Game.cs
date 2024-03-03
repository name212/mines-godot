using Godot;
using mines;

public partial class Game : Node
{
	private Mines _gameField;

	// using for indicate re-render field in UI
	public Types.MinesField CurrentField;

	public void NewGame(int width, int height, int mines)
	{
		_gameField = new Mines(width, height, mines, new StdTimer());
	}

	public void Open(int x, int y)
	{
		_gameField?.Open(new Types.Position(x, y));
		CurrentField = _gameField?.Field;
	}

	public void Mark(int x, int y)
	{
		_gameField?.Mark(new Types.Position(x, y));
		CurrentField = _gameField?.Field;
	}

	public int MarkedAsBombCount()
	{
		return _gameField != null ? _gameField.Field.MarkedAsBombsCount() : 0; 
	}
	public int MinesCount()
	{
		return _gameField != null ? _gameField.Field.game.mines : 0; 
	}
	
	public void PauseOrResume()
	{
		_gameField?.PauseOrResume();
	}

	public double Duration()
	{
		return _gameField?.Duration ?? 0.0;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
