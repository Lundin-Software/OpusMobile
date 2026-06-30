namespace Opus.Mobile.Shared.Models.Exceptions;

public class DeserializationException(string message = ErrorMessages.DeserializationError) : Exception(message)
{
}
