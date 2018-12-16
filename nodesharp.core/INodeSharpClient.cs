using System.Collections.Generic;

namespace nodesharp.core
{
    public interface INodeSharpClient {
        List<string> GetMethodList();
        T Invoke<T>(string name, params object[] args);
    }


}
