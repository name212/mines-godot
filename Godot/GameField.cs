using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class GameField : Control
{
	private const string LabelsContainerPath = "VBoxContainer/HBoxContainer/VBoxContainer";
	private const string FieldContainerPath = "VBoxContainer/CenterContainer";
	private const string GridContainerName = "Field";

	private const float PanSpeed = 1.0f; 

	private int _curDuration = -1;
	private int _curMinesCount = -1;
	private int _curMarkedMinesCount = -1;
	private long _startPressed = 0;


	private Dictionary<int, Vector2> _touchPoints = new Dictionary<int, Vector2>();
		
	private Types.MinesField _currentField;

	private Camera2D _camera;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera2D>("Camera2D"); 
	}

	private void HandleTouch(InputEventScreenTouch e)
	{
		if (e.Pressed)
		{
			if (e.DoubleTap)
			{
				_startPressed = 0;
				var pos = GetViewport().CanvasTransform.AffineInverse() * e.Position;
				var cell = FindCellByPosition(pos);
				if (cell == null)
				{
					return;
				}
				GD.Print($"Double tap on cell {cell.c.pos}");
				HandleCellLeftClick(cell.c.pos.x, cell.c.pos.y);
				return;
			}
			
			GD.Print("Handle touch: pressed");
			_touchPoints[e.Index] = e.Position;
			if (_startPressed == 0)
			{
				var now = DateTime.UtcNow;
				_startPressed = ((DateTimeOffset)now).ToUnixTimeMilliseconds();
				GD.Print($"Start touch {_startPressed}");
			}
			else
			{
				GD.Print($"Touch already started {_startPressed}");
			}
		}
		else
		{
			GD.Print("Handle touch: not pressed");
			_touchPoints.Remove(e.Index);
			
			var now = DateTime.UtcNow;
			var nowUnix = ((DateTimeOffset)now).ToUnixTimeMilliseconds();
			var start = _startPressed;
			GD.Print($"Touch finishes {nowUnix} - {_startPressed} = {nowUnix - start}");
			_startPressed = 0;
			if (nowUnix - start > 180)
			{
				var pos = GetViewport().CanvasTransform.AffineInverse() * e.Position;
				var cellR = FindCellByPosition(pos);
				if (cellR == null)
				{
					return;
				}
				GD.Print($"Long tap on cell {cellR.c.pos}");
				HandleCellRightClick(cellR.c.pos.x, cellR.c.pos.y);
				return;
			}
		}
	}
	
	private void HandleDrag(InputEventScreenDrag e)
	{
		_touchPoints[e.Index] = e.Position;
		if (_touchPoints.Keys.Count == 1)
		{
			_camera.Position -= e.Relative * PanSpeed;
			GD.Print($"HandleDrag set Position {Position}");
			AcceptEvent();
		}
		
	}

	private void Zoom(InputEventMagnifyGesture e)
	{
		var newZoom = _camera.Zoom * e.Factor;
			
		if (newZoom.X < 0.1f)
		{
			newZoom.X = 0.1f;
		}

		if (newZoom.X > 10.0f)
		{
			newZoom.X = 10.0f;
		}
			
		if (newZoom.Y < 0.1f)
		{
			newZoom.Y = 0.1f;
		}

		if (newZoom.Y > 10.0f)
		{
			newZoom.Y = 10.0f;
		}

		_camera.Zoom = newZoom;
		
		AcceptEvent();
	}

	public override void _Input(InputEvent e)
	{
		base._Input(e);
		
		var os = OS.Singleton.GetName();
		if (os != "Android")
		{
			if (!e.IsPressed()) return;
		
			if (e is not InputEventMouseButton mouse) return;
			switch (mouse.ButtonIndex)
			{
				case MouseButton.Left:
					var pos = GetViewport().CanvasTransform.AffineInverse() * mouse.Position;
					var cell = FindCellByPosition(pos);
					if (cell == null)
					{
						return;
					}
					GD.Print($"Left pressed on cell {cell.c.pos}");
					HandleCellLeftClick(cell.c.pos.x, cell.c.pos.y);
					return;
				case MouseButton.Right:
					var posR = GetViewport().CanvasTransform.AffineInverse() * mouse.Position;
					var cellR = FindCellByPosition(posR);
					if (cellR == null)
					{
						return;
					}
					GD.Print($"Right pressed on cell {cellR.c.pos}");
					HandleCellRightClick(cellR.c.pos.x, cellR.c.pos.y);
					return;
				default:
					GD.Print($"Unknown mouse button pressed in position {mouse.Position}");
					break;
			}	
			
			return;
		}

		switch (e)
		{
			case InputEventMagnifyGesture magnify:
				GD.Print(
					$"Magnify event handle in screen {magnify.Position.Y}X{magnify.Position.Y} magnify {magnify.Factor}");
				Zoom(magnify);
				break;
			case InputEventScreenTouch touch:
				GD.Print(
					$"Touch event {touch.Index} handle in screen {touch.Position.Y}X{touch.Position.Y} IsPressed {touch.Pressed} DoubleTap {touch.DoubleTap} Canceled {touch.Canceled}");
				HandleTouch(touch);
				break;
			case InputEventScreenDrag drag:
				GD.Print(
					$"Drag event {drag.Index} handle in screen {drag.Position.Y}X{drag.Position.Y} Pressure {drag.Pressure} PenInverted {drag.PenInverted} Tilt {drag.Tilt.ToString()} velocity {drag.Velocity}");
				HandleDrag(drag);
				break;
		}

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

	private Cell FindCellByPosition(Vector2 pos)
	{
		var fieldContainer = GetNode<CenterContainer>(FieldContainerPath);
		var fieldView = fieldContainer.GetNode<GridContainer>(GridContainerName);

		foreach (var node in fieldView.GetChildren())
		{
			var ctrt = node as Cell;
			if (ctrt.GetGlobalRect().HasPoint(pos))
			{
				GD.Print($"Found cell for rect {ctrt.GetRect()} at position {pos}");
				return ctrt;
			}
		}

		return null;
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
		fieldView.Size = new Vector2(1152.0f, 648.0f);

		var controlls = GetNode<HBoxContainer>("VBoxContainer/HBoxContainer");
		var w = (1152.0f / settings.width) - 5;
		var h = ((648.0f - controlls.Size.Y) / settings.height) - 5;
		GD.Print($"Cell {w}x{h}");
		var s = Math.Min(w, h);
		var sz = new Vector2(s, s);
		GD.Print($"New cell size {s}x{s}");
		for (int pos = 0; pos < settings.Size; pos++)
		{
			var cell = field.Cell(pos);
			var cellView = new Cell(cell, sz);
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
