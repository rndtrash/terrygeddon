using Sandbox;
using Sandbox.UI;

[Library]
public partial class TerrygeddonHud : HudEntity<RootPanel>
{
	public enum UIState
	{
		None,
		WaitingForPlayers,	// Lobby
		PreGame,			// Selecting a car
		InGame,				// In game
		GameOver,			// Player has won or lost
		PostGame			// Game is over
	}

	public UIState State
	{
		get
		{
			return _state;
		}

		set
		{
			Log.Info( $"Changing UI state to {value}..." );

			if ( _state == value )
				return;

			_state = value;
			switch ( _state )
			{
				case UIState.WaitingForPlayers:
					RootPanel.SetClass( "state_wfp", true );
					RootPanel.SetClass( "state_prep", false );
					RootPanel.SetClass( "state_ingame", false );
					RootPanel.SetClass( "state_gameover", false );
					RootPanel.SetClass( "state_postgame", false );
					break;
				case UIState.PreGame:
					// TODO: play music and shit
					RootPanel.SetClass( "state_wfp", false );
					RootPanel.SetClass( "state_pregame", true );
					break;
				case UIState.InGame:
					RootPanel.SetClass( "state_prep", false );
					RootPanel.SetClass( "state_ingame", true );
					break;
				case UIState.GameOver:
					// TODO: game over yeeeaaaaah
					RootPanel.SetClass( "state_gameover", true );
					break;
				case UIState.PostGame:
					RootPanel.SetClass( "state_ingame", false );
					RootPanel.SetClass( "state_gameover", false );
					RootPanel.SetClass( "state_postgame", true );
					break;
				default:
					Log.Error( $"FIXME: Add {_state}" );
					break;
			}
		}
	}

	private UIState _state = UIState.None;

	public TerrygeddonHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/TerrygeddonHud.scss" );

		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<TerrygeddonScoreboard>();

		var p_wfp = RootPanel.AddChild<Panel>( "sp wfp" );

		var p_pregame = RootPanel.AddChild<Panel>( "sp pregame" );

		var p_ingame = RootPanel.AddChild<Panel>( "sp ingame" );
		p_ingame.AddChild<Health>();

		var p_gameover = RootPanel.AddChild<Panel>( "sp gameover" );

		var p_postgame = RootPanel.AddChild<Panel>( "sp postgame" );
	}

	[ClientRpc]
	public void SetUIStateRPC(UIState newState)
	{
		State = newState;
	}
}
