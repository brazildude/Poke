// See https://aka.ms/new-console-template for more information
using Poke.Core;
using Poke.Core.Models.Units;
using Poke.Debug.Match;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting Match context");

        var context = new Context
        {
            ContextID = 1,
            CurrentTeamID = 1,
            Round = 1,
            CurrentTeam = CreateTeam(1),
            NextTeam = CreateTeam(2),
        };

        SimulateMatch(context);
    }

    private static Team CreateTeam(int ID)
    {
        return new Team
        {
            TeamID = ID,
            UserID = ID,
            Units = new List<BaseUnit>()
            {
                new Mage()
            }
        };
    }

    private static void SimulateMatch(Context context)
    {
        context.CurrentTeam.Units[0].UseSkill(1, context.CurrentTeam.Units, context.NextTeam.Units);
    }
}
