namespace Poke.Server.Infrastructure.GameLogic;

public class ErrorDTO
{
    public string? Message { get; set; }

    public static ErrorDTO New(string? message = null)
    {
        return new ErrorDTO
        {
            Message = message
        };
    }
}
