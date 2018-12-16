using System;
using System.Reflection;
using System.Threading;
using System.Reflection.Emit;
using nodesharp.core;
using System.IO;

namespace nodesharp.application
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create builder
            var nodeSharpBinder = new NodeSharpBinderBuilder()
                .Bind<MyJavascriptFunctions>(Path.GetFullPath("E:/Development/test_node/test.js"))
                .Build();
            var mf = nodeSharpBinder.Resolve<MyJavascriptFunctions>();
            
            var result = mf.subtract(10, 4);
            var result2 = mf.add(126, 5);
            Console.WriteLine($"Executed Javascript function substract 4 from 10, got: {result}");
            Console.WriteLine($"Executed Javascript function add 5 to 126, got: {result2}");
        }
    }

    public interface MyJavascriptFunctions : INodeSharp {
        int add(int x, int y);
        int subtract(int x, int y);
    }
}
