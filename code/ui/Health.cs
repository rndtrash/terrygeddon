using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Health : Panel
{
	public Label Label;

	public Health()
	{
		Label = Add.Label( "100", "value" );
	}

	public override void Tick()
	{
		var player = Local.Pawn as TerrygeddonPlayer;
		if ( player == null || player.Vehicle == null ) return;

		Label.Text = $"{player.Vehicle.Health.CeilToInt()}";
	}
}
