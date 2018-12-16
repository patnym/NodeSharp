using Newtonsoft.Json;

namespace nodesharp.core
{
    class Parcel {

        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("data")]
        public ISocketMessage Data { get; private set; }

        public Parcel(ISocketMessage message)
        {
            Data = message;
            Type = message.Type.ToLower();
        }

        public Parcel()
        {
        }
    }


}
