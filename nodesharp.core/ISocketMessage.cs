using Newtonsoft.Json;

namespace nodesharp.core
{
    public interface ISocketMessage {
        [JsonIgnore]
        string Type { get; }
        void Deserialize(byte[] msg);
    }


}
