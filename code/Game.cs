using Sandbox;
using System.Collections.Generic;

public partial class TerrygeddonGame : Game
{
	public enum GameState
	{
		None,
		WaitingForPlayers,
		PreGame,
		InGame,
		PostGame
	}

	public GameState State
	{
		get
		{
			return _state;
		}

		set
		{
			Log.Info( $"Changing game state to {value}..." );

			if ( _state == value )
				return;

			_state = value;
			var requiresRespawn = false;
			switch ( _state )
			{
				case GameState.WaitingForPlayers:
					_currentSpawnAction = null;
					requiresRespawn = true;
					HUDEntity.SetUIStateRPC( TerrygeddonHud.UIState.WaitingForPlayers );
					break;
				case GameState.PreGame:
					requiresRespawn = false;
					HUDEntity.SetUIStateRPC( TerrygeddonHud.UIState.PreGame );
					break;
				case GameState.InGame:
					_currentSpawnAction = PlayerSpawnAction;
					requiresRespawn = true;
					HUDEntity.SetUIStateRPC( TerrygeddonHud.UIState.InGame );
					break;
				case GameState.PostGame:
					requiresRespawn = false;
					HUDEntity.SetUIStateRPC( TerrygeddonHud.UIState.PostGame );
					break;
				default:
					Log.Error( $"FIXME: Add {_state}" );
					break;
			}

			if ( requiresRespawn )
				RespawnAllPawns();
		}
	}

	[ConVar.Replicated( "tg_gamestate" )]
	public static GameState GameStateConVar
	{
		get
		{
			return (Current as TerrygeddonGame).State;
		}
	}

	public TerrygeddonHud HUDEntity { get; private set; }

	delegate void ClientAction( Client cl );

	private GameState _state = GameState.None;
	private Dictionary<int, Client> _clients = new();
	private ClientAction _currentSpawnAction;

	public TerrygeddonGame()
	{
		if ( IsServer )
		{
			HUDEntity = new TerrygeddonHud();

			State = GameState.WaitingForPlayers;
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		_clients.Add( cl.UserId, cl );

		if ( cl.Pawn != null && _currentSpawnAction != null )
			_currentSpawnAction( cl );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[ServerCmd( "spawn" )]
	public static void Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = new Prop();
		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		ent.SetModel( modelname );

		// Drop to floor
		if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
		{
			var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );

			var delta = p - tr.EndPos;
			ent.PhysicsBody.Position -= delta;
			//DebugOverlay.Line( p, tr.EndPos, 10, false );
		}

	}

	[ServerCmd( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		var owner = ConsoleSystem.Caller.Pawn;

		if ( owner == null )
			return;

		var attribute = Library.GetAttribute( entName );

		if ( attribute == null || !attribute.Spawnable )
			return;

		var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = Library.Create<Entity>( entName );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}

		ent.Position = tr.EndPos;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		//Log.Info( $"ent: {ent}" );
	}

	public override void DoPlayerNoclip( Client player )
	{
		if ( player.Pawn is Player basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}

	protected void PlayerSpawnAction( Client cl )
	{
		var player = new TerrygeddonPlayer( cl );
		player.Respawn();

		cl.Pawn = player;
	}

	protected void RespawnAllPawns()
	{
		foreach ( var cl in _clients )
		{
			var pawn = cl.Value.Pawn;

			if ( pawn != null )
				pawn.Delete();

			if ( _currentSpawnAction != null )
				_currentSpawnAction( cl.Value );
		}
	}
}
