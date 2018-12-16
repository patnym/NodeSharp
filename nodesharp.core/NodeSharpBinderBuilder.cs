using System;
using System.Collections.Generic;

namespace nodesharp.core
{
    public class NodeSharpBinderBuilder {

        private Dictionary<Type, string> _bindMap;

        public NodeSharpBinderBuilder()
        {
            _bindMap = new Dictionary<Type, string>();
        }

        public NodeSharpBinderBuilder Bind<T>(string fileName) where T : INodeSharp {
            _bindMap.Add(typeof(T), fileName);
            return this;
        }

        public INodeSharpBinder Build() {
            var nb = new NodeSharpBinder(_bindMap);
            nb.Bind();
            return nb;
        }
    }


}
