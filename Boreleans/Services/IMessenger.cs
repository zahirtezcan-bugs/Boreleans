namespace Boreleans.Services
{
    public interface IMessenger
    {
        ValueTask<MessageResult<int>> SendMessage(int parameter);
    }
}
