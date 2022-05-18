namespace Boreleans.Services
{
    public record struct MessageResult<T>(MessageError Error, T? Value);
}
