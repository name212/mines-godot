using Godot;
using System;

public partial class MainScene : Node2D
{
	private const string InputsContainerPath = "MainContainer/InputContainer";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetWindow().Size = new Vector2I(400, 400);
		GetWindow().MinSize = new Vector2I(400, 400);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_start_btn_pressed()
	{
		GD.Print("_on_start_btn_pressed");

		var width = (int) GetNode<SpinBox>($"{InputsContainerPath}/Width").Value;
		var height = (int) GetNode<SpinBox>($"{InputsContainerPath}/Height").Value;
		var mines = (int) GetNode<SpinBox>($"{InputsContainerPath}/Mines").Value;
		
		var game = Game.GetGame(this);
		
		game.NewGame(width, height, mines);

		GD.Print($"Start new game with {width}x{height} field and {mines} bombs");
		
		GetTree().ChangeSceneToFile("res://GameField.tscn");
	}
	
}

