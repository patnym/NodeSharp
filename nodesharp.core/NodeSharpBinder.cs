using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace nodesharp.core
{
    internal class NodeSharpBinder : INodeSharpBinder {

        private readonly string NODE_EXE = Path.GetFullPath("E:/Programs/nvm/v11.0.0/node.exe");

        private Dictionary<Type, object> _container;
        private Dictionary<Type, IEnumerable<MethodDescriptor>> _methodDescriptions;
        private Dictionary<Type, string> _bindMap;
        //Spin up node process
        public NodeSharpBinder(Dictionary<Type, string> bindMap)
        {
            _bindMap = bindMap;
            _container = new Dictionary<Type, object>();
            _methodDescriptions = new Dictionary<Type, IEnumerable<MethodDescriptor>>();
        }

        public T Resolve<T>() where T : INodeSharp {
            if(_container.TryGetValue(typeof(T), out var idep)) {
                return (T)idep;
            } else {
                throw new Exception($"Could not resolve interface {typeof(T)}, are you missing a bind?");
            }
        }

        
        public void Bind() {

            //TEST
            var m = new List<MethodDescriptor>() {
                new MethodDescriptor(
                    "add",
                    new List<Arguement>() {
                        Arguement.Create("int"),
                        Arguement.Create("int")
                    },
                    Arguement.Create("int")
                ),
                new MethodDescriptor(
                    "subtract",
                    new List<Arguement>() {
                        Arguement.Create("int"),
                        Arguement.Create("int")
                    },
                    Arguement.Create("int")
                )
            };
            _methodDescriptions.Add(_bindMap.Keys.First(), m);

            foreach(var iClass in _bindMap) {
                var proxyBuilder = new ProxyBuilder()
                    .AddNodeJsExe(NODE_EXE)
                    .AddNodeJsEntry(iClass.Value);

                if(_methodDescriptions.TryGetValue(iClass.Key, out var methods)) {
                    proxyBuilder.AddMethods(methods);
                } else {
                    throw new Exception($"Could not find a valid method descriptor for type of {iClass.Key}");
                }
                _container.Add(iClass.Key, proxyBuilder.Build(iClass.Key));
            }
        }

    }
    
    class MethodDescriptor {
        public string Name { get; private set; }
        public List<Arguement> Arguements { get; private set; }
        public Arguement Returns { get; private set; }

        public MethodDescriptor(string name, List<Arguement> arguements, 
            Arguement returns)
        {
            Name = name;
            Arguements = arguements;
            Returns = returns;
        }
    }

    class Arguement {
        public Type Type { get; private set; }

        private Arguement()
        {
            
        }

        public static Arguement Create(string type) {
            Type t;
            switch(type) {
                case "int":
                    t = typeof(int);
                    break;
                default:
                    throw new Exception($"Invalid type {type}, only supports types: int");
            }
            return new Arguement {
                Type = t
            };
        }
    }
}
