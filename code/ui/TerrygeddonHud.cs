using Sandbox;
using Sandbox.UI;

[Library]
public partial class TerrygeddonHud : HudEntity<RootPanel>
{
	public TerrygeddonHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/TerrygeddonHud.scss" );

		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<CrosshairCanvas>();
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<TerrygeddonScoreboard>();
		RootPanel.AddChild<Health>();
	}
}
