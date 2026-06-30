namespace Opus.Mobile.Shared.Models.Exceptions;

public class UnknownRequestException : Exception
{
    public UnknownRequestException(string message = ErrorMessages.UnknownRequestError) : base(message) { }

    public UnknownRequestException(Exception? innerException) : base(ErrorMessages.UnknownRequestError, innerException) { }
}
