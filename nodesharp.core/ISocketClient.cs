namespace nodesharp.core
{
    public interface ISocketClient {
        void Connect();
        void SendMessage(ISocketMessage message);
        T WaitForResponse<T>() where T : ISocketMessage, new();
    }
}
