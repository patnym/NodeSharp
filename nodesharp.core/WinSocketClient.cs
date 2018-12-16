using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;

namespace nodesharp.core
{
    class WinSocketClient : ISocketClient
    {
        private const int MSG_BUFFER = 20480;
        private string _pipeName;
        private NamedPipeClientStream _pipe;

        public WinSocketClient(string pipeName = "tmp-app.world")
        {
        }

        public void SendMessage(ISocketMessage message)
        {
            var data = Serialize(new Parcel(message));
            _pipe.Write(data);
        }

        private byte[] Serialize(Parcel parcel)
        {
            var json = JsonConvert.SerializeObject(parcel);
            return Encoding.UTF8.GetBytes(json);
        }

        public T WaitForResponse<T>() where T : ISocketMessage, new()
        {
            byte[] res = new byte[MSG_BUFFER];
            _pipe.Read(res, 0, MSG_BUFFER - 1);
            var msg = new T();
            msg.Deserialize(res);
            return msg;
        }

        public void Connect() {
            _pipe = new NamedPipeClientStream(".", "tmp-app.world",
                PipeDirection.InOut, PipeOptions.Asynchronous);
            _pipe.Connect();
        }
    }
}
