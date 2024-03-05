using Godot;
using System;

public partial class GameField : Node2D
{
	private const string LabelsContainerPath = "ScrollContainer/VBoxContainer/HBoxContainer/VBoxContainer";
	private const string FieldContainerPath = "ScrollContainer/VBoxContainer";

	private int _curDuration = -1;
	private int _curMinesCount = -1;
	private int _curMarkedMinesCount = -1;
	
	private Types.MinesField _currentField;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetWindow().Size = new Vector2I(800, 800);
		GetWindow().MinSize = new Vector2I(400, 400);
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

		const string gridContainerName = "Field";
		
		GD.Print("Field should update");
		
		var settings = field.game;
		
		var fieldView = new GridContainer();
		fieldView.Name = gridContainerName;
		fieldView.Columns = settings.width;
		fieldView.GrowHorizontal = Control.GrowDirection.Both;
		
		for (int pos = 0; pos < settings.Size; pos++)
		{
			var cell = field.Cell(pos);
			var cellView = new Cell(cell);
			cellView.LeftClick += HandleCellLeftClick;
			fieldView.AddChild(cellView);
		}

		const string fullPath = $"{FieldContainerPath}";
		var fieldContainer = GetNode<VBoxContainer>(fullPath);
		var oldFieldView = fieldContainer.GetNode<GridContainer>(gridContainerName);
		if (oldFieldView != null)
		{
			fieldContainer.RemoveChild(oldFieldView);
		}
		
		fieldContainer.AddChild(fieldView);
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
	private void HandleCellLeftClick(int x, int y)
	{
		GD.Print($"HandleCellLeftClick ({x}; {y})");
	}
	
}

