namespace Opus.Mobile.API.Models.Exceptions;

public class BadRequestException(string exception) : Exception(exception)
{
}
