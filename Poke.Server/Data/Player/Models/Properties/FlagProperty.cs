using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models.Properties;

public class FlagProperty
{
    public int FlagPropertyID { get; set; }
    public bool IsTrue { get; set; }
    public PropertyName PropertyName { get; set; }
}
