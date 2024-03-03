using Godot;
using System;

public partial class Cell(Types.Cell c) : Control
{
	private static readonly Vector2 MinSize = new Vector2(20, 20);
	private static readonly Texture2D MarkAsBombTxt = GD.Load<Texture2D>("res://assets/marked.png");
	private static readonly Texture2D MarkAsProbablyBombTxt = GD.Load<Texture2D>("res://assets/probablyMarked.png");
	private static readonly Texture2D BombTxt = GD.Load<Texture2D>("res://assets/probablyMarked.png");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Control cld = new Label();
		switch (c.state.Tag)
		{
			case Types.CellState.Tags.Closed:
				var btn = new Button();
				btn.Disabled = false;
				cld = btn;
				break;
			case Types.CellState.Tags.Opened:
				if (c.hasBomb)
				{
					var btn2 = new Button();
					btn2.Icon = BombTxt;
					btn2.IconAlignment = HorizontalAlignment.Center;
					btn2.VerticalIconAlignment = VerticalAlignment.Center;
					cld = btn2;
				} else if (c.bombsAround > 0)
				{
					var lbl = new Label();
					lbl.Text = c.bombsAround.ToString();
					lbl.HorizontalAlignment = HorizontalAlignment.Center;
					lbl.VerticalAlignment = VerticalAlignment.Center;
					cld = lbl;
				} else if (c.bombsAround == 0)
				{
					var btn3 = new Button();
					btn3.Disabled = true;
					cld = btn3;
				}
				
				break;
			case Types.CellState.Tags.MarkAsBomb:
				var btn4 = new Button();
				btn4.Icon = MarkAsBombTxt;
				btn4.IconAlignment = HorizontalAlignment.Center;
				btn4.VerticalIconAlignment = VerticalAlignment.Center;
				cld = btn4;
				break;
			case Types.CellState.Tags.MarkAsProbablyBomb:
				var btn5 = new Button();
				btn5.Icon = MarkAsProbablyBombTxt;
				btn5.IconAlignment = HorizontalAlignment.Center;
				btn5.VerticalIconAlignment = VerticalAlignment.Center;
				cld = btn5;
				break;
		}
		
		AddChild(cld);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override Vector2 _GetMinimumSize()
	{
		return MinSize;
	}

}
