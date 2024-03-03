using Godot;
using System;

public partial class GameField : Node2D
{
	private const string LabelsContainerPath = "ScrollContainer/VBoxContainer/HBoxContainer/VBoxContainer";

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
			var minesCountLbl = GetNode<Label>($"{LabelsContainerPath}/HBoxContainer/MinesCountLabel");
			minesCountLbl.Text = minesCount.ToString();
			_curMinesCount = minesCount;
		}

		var markedMinesCount = game.MarkedAsBombCount();
		if (markedMinesCount != _curMarkedMinesCount)
		{
			var markedMinesCountLbl = GetNode<Label>($"{LabelsContainerPath}/HBoxContainer/MinesMarkedLabel");
			markedMinesCountLbl.Text = markedMinesCount.ToString();
			_curMarkedMinesCount = markedMinesCount;
		}
		
		var dur = (int)game.Duration();
		if (dur > _curDuration)
		{
			var timerLbl = GetNode<Label>($"{LabelsContainerPath}/TimerLabel");
			TimeSpan time = TimeSpan.FromSeconds(dur);
			timerLbl.Text = time.ToString(@"hh\:mm\:ss");
			_curDuration = dur;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var game = Game.GetGame(this);
		UpdateCounters(game);
		
	}
	
	private void _on_new_game_button_pressed()
	{
		GD.Print("_on_new_game_button_pressed");
		GetTree().ChangeSceneToFile("res://MainScene.tscn");
	}
}

