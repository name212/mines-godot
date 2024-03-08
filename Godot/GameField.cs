using Godot;
using System;

public partial class GameField : Control
{
	private const string LabelsContainerPath = "VBoxContainer/HBoxContainer/VBoxContainer";
	private const string FieldContainerPath = "VBoxContainer/CenterContainer";
	private const string GridContainerName = "Field";


	private int _curDuration = -1;
	private int _curMinesCount = -1;
	private int _curMarkedMinesCount = -1;
	
	private Types.MinesField _currentField;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private void UpdateCounters(Game game)
	{
		var minesCount = game.MinesCount();
		if (minesCount != _curMinesCount)
		{
			GD.Print("Mines count should update");
			
			var minesCountLbl = GetNode<Label>($"{LabelsContainerPath}/HBoxContainer/MinesCountLabel");
			minesCountLbl.Text = minesCount.ToString();
			_curMinesCount = minesCount;
		}

		var markedMinesCount = game.MarkedAsBombCount();
		if (markedMinesCount != _curMarkedMinesCount)
		{
			GD.Print("Mines marked count should update");
			
			var markedMinesCountLbl = GetNode<Label>($"{LabelsContainerPath}/HBoxContainer/MinesMarkedLabel");
			markedMinesCountLbl.Text = markedMinesCount.ToString();
			_curMarkedMinesCount = markedMinesCount;
		}
		
		var dur = (int)game.Duration();
		if (dur > _curDuration)
		{
			GD.Print("Duration should update");

			var timerLbl = GetNode<Label>($"{LabelsContainerPath}/TimerLabel");
			TimeSpan time = TimeSpan.FromSeconds(dur);
			timerLbl.Text = time.ToString(@"hh\:mm\:ss");
			_curDuration = dur;
		}
	}

	private void UpdateField(Game game)
	{
		var field = game.Field();
		if (field == null || field == _currentField)
		{
			return;
		}
		
		GD.Print("Field should update");
		
		var settings = field.game;
		
		var fieldView = new GridContainer();
		fieldView.Name = GridContainerName;
		fieldView.Columns = settings.width;
		fieldView.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;

		for (int pos = 0; pos < settings.Size; pos++)
		{
			var cell = field.Cell(pos);
			var cellView = new Cell(cell);
			cellView.LeftClick += HandleCellLeftClick;
			cellView.RightClick += HandleCellRightClick;
			fieldView.AddChild(cellView);
		}
		
		var fieldContainer = GetNode<CenterContainer>(FieldContainerPath);
		var oldFieldView = fieldContainer.GetNode<GridContainer>(GridContainerName);
		if (oldFieldView != null)
		{
			fieldContainer.RemoveChild(oldFieldView);
			var cldren = oldFieldView.GetChildren();
			foreach (var c in cldren)
			{
				c.QueueFree();
			}
			oldFieldView.QueueFree();
		}
		
		fieldContainer.AddChild(fieldView);
		var size = fieldContainer.GetNode<GridContainer>(GridContainerName).Size;
		var headerSize = GetNode<HBoxContainer>("VBoxContainer/HBoxContainer").Size;
		var windowSize = new Vector2I((int)size.X, (int) size.Y + (int) headerSize.Y + 10);
		
		GD.Print($"New window size {windowSize.X}x{windowSize.Y}");
		GetWindow().Size = windowSize;
		
		_currentField = field;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var game = Game.GetGame(this);
		UpdateCounters(game);
		UpdateField(game);
	}
	
	private void _on_new_game_button_pressed()
	{
		GD.Print("_on_new_game_button_pressed");
		GetTree().ChangeSceneToFile("res://MainScene.tscn");
	}
	
	private void _on_pause_button_pressed()
	{
		GD.Print("_on_new_game_button_pressed");
		Game.GetGame(this).PauseOrResume();
		PrintOrphanNodes();
	}
	
	private void HandleCellLeftClick(int x, int y)
	{
		GD.Print($"HandleCellLeftClick ({x}; {y})");
		Game.GetGame(this).Open(x, y);
	}

	private void HandleCellRightClick(int x, int y)
	{
		GD.Print($"HandleCellRightClick ({x}; {y})");
		Game.GetGame(this).Mark(x, y);
	}
}
