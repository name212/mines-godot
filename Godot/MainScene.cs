using Godot;
using System;

public partial class MainScene : Node2D
{
	
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
		GD.Print("Hello, World!");
	}
	
}

