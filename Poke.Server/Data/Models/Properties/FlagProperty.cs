using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models.Properties;

public class FlagProperty
{
    public int FlagPropertyID { get; set; }
    public bool IsTrue { get; set; }
    public PropertyName PropertyName { get; set; }
}
