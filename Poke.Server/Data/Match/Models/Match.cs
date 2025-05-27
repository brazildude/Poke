
namespace Poke.Server.Data.Match.Models;

public class Match
{
    public Guid MatchID { get; set; }
    public string UserID01 { get; set; } = null!;
    public string UserID02 { get; set; } = null!;
    public int Team01ID { get; set; }
    public int Team02ID { get; set; }
    public string? UserWinnerID { get; set; }
    public bool IsMatchOver { get; set; }

    public MatchState State { get; set; } = null!;
}
