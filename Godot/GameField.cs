using Godot;
using System;

public partial class GameField : Node2D
{
	private const string LabelsContainerPath = "ScrollContainer/VBoxContainer/HBoxContainer/VBoxContainer";

	private int curDuration = -1;
	
	private Types.MinesField _currentField;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetWindow().Size = new Vector2I(800, 800);
		GetWindow().MinSize = new Vector2I(400, 400);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var markedMinesCount = GetNode<Label>($"{LabelsContainerPath}/HBoxContainer/MinesMarkedLabel");
		var minesCount = GetNode<Label>($"{LabelsContainerPath}/HBoxContainer/MinesCountLabel");
		var timer = GetNode<Label>($"{LabelsContainerPath}/TimerLabel");
		
		var game = Game.GetGame(this);

		minesCount.Text = game.MinesCount().ToString();
		markedMinesCount.Text = game.MarkedAsBombCount().ToString();
		var dur = (int)game.Duration();
		if (dur > curDuration)
		{
			TimeSpan time = TimeSpan.FromSeconds(dur);
			timer.Text = time.ToString(@"hh\:mm\:ss\:fff");
		}
	}
	
	private void _on_new_game_button_pressed()
	{
		GD.Print("_on_new_game_button_pressed");
		GetTree().ChangeSceneToFile("res://MainScene.tscn");
	}
}

