using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class TerrygeddonScoreboardEntry : Panel
{
	public PlayerScore.Entry Entry;

	public Label PlayerName;
	public Label Ping;

	public TerrygeddonScoreboardEntry()
	{
		AddClass( "entry" );

		PlayerName = Add.Label( "PlayerName", "name" );
		Ping = Add.Label( "", "ping" );
	}

	public void UpdateFrom( PlayerScore.Entry entry )
	{
		Entry = entry;

		PlayerName.Text = entry.GetString( "name" );
		Ping.Text = entry.Get<int>( "ping", 0 ).ToString();

		// FIXME: Local.Client is null
		SetClass( "me", Local.Client != null && entry.Get<ulong>( "steamid", 0 ) == Local.Client.SteamId );
	}
}
