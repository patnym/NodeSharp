using System.Text;

namespace nodesharp.core
{
    class TestMessage : ISocketMessage {
        
        private string _msg;
        public string Message => _msg;

        public string Type => "Test";

        public TestMessage()
        {
        }

        public TestMessage(string msg)
        {
            _msg = msg;
        }

        public void Deserialize(byte[] msg)
        {
            _msg = Encoding.UTF8.GetString(msg);
        }
    }

}
