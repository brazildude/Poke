namespace Poke.Server.Infrastructure.GameLogic;

public class MatchFinishedDTO
{
    public string? WinnerPlayerID { get; set; }

    public static MatchFinishedDTO New(string? winnerPlayerID = null)
    {
        return new MatchFinishedDTO
        {
            WinnerPlayerID = winnerPlayerID
        };
    }
}
