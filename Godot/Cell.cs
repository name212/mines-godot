using Godot;
using System;

public partial class Cell : Control
{
	private static readonly Texture2D MarkAsBombTxt = GD.Load<Texture2D>("res://assets/marked.png");
	private static readonly Texture2D MarkAsProbablyBombTxt = GD.Load<Texture2D>("res://assets/probablyMarked.png");
	private static readonly Texture2D BombTxt = GD.Load<Texture2D>("res://assets/mine.png");

	public Types.Cell c;
	private readonly Vector2 _size;

	public Cell(Types.Cell c, Vector2 s) {
		this.c = c;
		_size = s;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Control cld = null ;
		switch (c.state.Tag)
		{
			case Types.CellState.Tags.Closed:
				var b = new Button();
				cld = b;
				break;
			case Types.CellState.Tags.Opened:
				if (c.hasBomb)
				{
					var btn2 = new Button();
					btn2.Icon = BombTxt;
					btn2.IconAlignment = HorizontalAlignment.Center;
					btn2.VerticalIconAlignment = VerticalAlignment.Center;
					btn2.Disabled = true;
					cld = btn2;
				} else if (c.bombsAround > 0)
				{
					var lbl = new Label();
					lbl.MouseFilter = MouseFilterEnum.Pass;
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
		
		if (cld == null)
		{
			GD.PrintErr($"Incorrect state {c.state.ToString()} for {c.pos.x}x{c.pos.y}");
			return;
		}

		cld.Size = _size;
		
		AddChild(cld);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		foreach (var cc in GetChildren())
		{
			cc.QueueFree();
		}
	}


	public override Vector2 _GetMinimumSize()
	{
		return _size;
	}

	private void OnInput(InputEvent e)
	{
		
	}
}
