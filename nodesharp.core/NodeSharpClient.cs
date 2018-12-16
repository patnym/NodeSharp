using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace nodesharp.core
{
    public class NodeSharpClient : INodeSharpClient
    {
        private ISocketClient _socket;
        public ISocketClient Socket { get; private set; }
        private Process _nodeProcess;
        public Process NodeProcess { get; private set; }

        public NodeSharpClient()
        {

        }

        public List<string> GetMethodList()
        {
            

            return null;
        }

        public static INodeSharpClient Create(ISocketClient socket, string nodeExePath, string nodeJsPath) {
            var process = SpawnProcess(nodeExePath, nodeJsPath);
            socket.Connect();
            return new NodeSharpClient {
                Socket = socket,
                NodeProcess = process
            };
        }

        private static Process SpawnProcess(string  nodeExePath, string nodeJsPath) {
            Process nodeServer = new Process();
            nodeServer.StartInfo.FileName = nodeExePath;
            nodeServer.StartInfo.Arguments = nodeJsPath;
            nodeServer.Start();

            var job = new WinJob();
            job.AddProcess(nodeServer.Handle);

            return nodeServer;
        }

        public T Invoke<T>(string name, params object[] args)
        {
            var msg = new FunctionMessage<T>(name) {
                Arguements = new List<object>(args)
            };
            
            Socket.SendMessage(msg);
            return Socket.WaitForResponse<FunctionMessage<T>>().Result;
        }
    }
}
