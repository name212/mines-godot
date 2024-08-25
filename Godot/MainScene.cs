using Godot;

public partial class MainScene : Node2D
{
	private const string InputsContainerPath = "MainContainer/InputContainer";

	private readonly Types.Field[] _gamesSelected = new[]
	{
		new Types.Field(8, 8, 10),
		new Types.Field(16, 16, 40),
		new Types.Field(30, 16, 99),
	};

	private void OnStartButtonPressed()
	{
		GD.Print("OnStartButtonPressed");

		var width = (int) GetNode<SpinBox>($"{InputsContainerPath}/Width").Value;
		var height = (int) GetNode<SpinBox>($"{InputsContainerPath}/Height").Value;
		var mines = (int) GetNode<SpinBox>($"{InputsContainerPath}/Mines").Value;
		
		var game = Game.GetGame(this);
		
		game.NewGame(width, height, mines);

		GD.Print($"Start new game with {width}x{height} field and {mines} bombs");
		
		GetTree().ChangeSceneToFile("res://GameField.tscn");
	}
	
	private void GameKindSelected(long index)
	{
		GD.Print("OnStartButtonPressed");

		var gm = _gamesSelected[index];
		
		GetNode<SpinBox>($"{InputsContainerPath}/Width").Value = gm.width;
		GetNode<SpinBox>($"{InputsContainerPath}/Height").Value = gm.height;
		GetNode<SpinBox>($"{InputsContainerPath}/Mines").Value = gm.mines;
	}
}
