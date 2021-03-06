namespace SpeedDial.Classic.UI;

[UseTemplate]
public partial class ClassicKillFeed : Panel
{
	public static ClassicKillFeed Current { get; private set; }

	public ClassicKillFeed()
	{
		Current = this;
	}

	public virtual Panel AddEntry( long lsteamid, string left, long rsteamid, string right, string method, bool domination = false )
	{
		var e = Current.AddChild<KillFeedEntry>();

		e.Left.Text = left;
		e.Left.SetClass( "me", lsteamid == (Local.Client?.PlayerId) );

		// set kill icon based on Cause Of Death
		e.Method.SetClass( "domination", domination );
		e.Method.SetTexture( $"materials/ui/killicons/{method.ToLower()}.png" );
		if ( e.Method.Texture is null )
		{
			e.Method.SetTexture( "materials/ui/killicons/generic.png" );
		}

		e.Right.Text = right;
		e.Right.SetClass( "me", rsteamid == (Local.Client?.PlayerId) );

		e.Important = lsteamid == (Local.Client?.PlayerId) || rsteamid == (Local.Client?.PlayerId);

		e.SetClass( "kill", lsteamid == (Local.Client?.PlayerId) );
		e.SetClass( "death", rsteamid == (Local.Client?.PlayerId) );

		// suicide hack
		if ( method.ToLower() == "suicide" )
		{
			e.Left.Text = "";
			e.Right.Text = left;
			e.Right.SetClass( "me", true );
			e.SetClass( "death", true );
		}

		if ( string.IsNullOrWhiteSpace( e.Left.Text ) )
		{
			e.Left.AddClass( "noname" );
		}

		if ( string.IsNullOrWhiteSpace( e.Right.Text ) )
		{
			e.Right.AddClass( "noname" );
		}

		return e;
	}

	[ClientRpc]
	public static void AddDeath( long lsteamid, string left, long rsteamid, string right, string method )
	{
		if ( Current is null ) return;
		Current.AddEntry( lsteamid, left, rsteamid, right, method );
	}

	[ClientRpc]
	public static void AddDeath( long lsteamid, string left, long rsteamid, string right, string method, bool domination )
	{
		if ( Current is null ) return;
		Current.AddEntry( lsteamid, left, rsteamid, right, method, domination );
	}
}
