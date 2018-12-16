using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace nodesharp.core
{
    internal class FunctionMessage<T> : ISocketMessage
    {
        public string Type => "Function";
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("arguements")]
        public List<object> Arguements { get; set; }
        private T _result;
        [JsonIgnore]
        public T Result => _result; 

        public FunctionMessage(string name)
        {
            Arguements = new List<object>();
            Name = name;
        }

        public FunctionMessage()
        {

        }

        public void Deserialize(byte[] msg)
        {
            _result = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(msg));
        }
    }

}
