using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace nodesharp.core
{
    internal class ProxyBuilder {
        private INodeSharpClient _client;

        private string _nodeExePath;
        private string _nodeJsEntryPath;
        private IEnumerable<MethodDescriptor> _methods;

        public ProxyBuilder AddNodeJsExe(string path) {
            _nodeExePath = path;
            return this;
        }

        public ProxyBuilder AddNodeJsEntry(string path) {
            _nodeJsEntryPath = path;
            return this;
        }

        public ProxyBuilder AddMethods(IEnumerable<MethodDescriptor> methods) {
            _methods = methods;
            return this;
        }

        public object Build(Type type) {
            CreateClient();
            return CreateImplementation(type);
        }

        private void CreateClient() {
            _client = NodeSharpClient.Create(new WinSocketClient(), 
                _nodeExePath, _nodeJsEntryPath);
        }

        private ModuleBuilder _moduleBuilder;

        private object CreateImplementation(Type type) {
            CreateModule();

            var tb = _moduleBuilder.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Public);
            tb.AddInterfaceImplementation(type);

            var proxyField = CreateConstructor(tb);

            foreach(var methodDesc in _methods) {
                var mb = CreateMethod(tb, proxyField, methodDesc);
                tb.DefineMethodOverride(mb, type.GetMethod(methodDesc.Name));
            }

            return Activator.CreateInstance(tb.CreateType(), args: _client);
        }

        private void CreateModule() {
            var appDomain = AppDomain.CurrentDomain;
            var asmName = new AssemblyName(Guid.NewGuid().ToString());

            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(
                asmName,
                AssemblyBuilderAccess.RunAndCollect
            );
            _moduleBuilder = asmBuilder.DefineDynamicModule($"{asmName.Name}.dll");
        }

        private FieldBuilder CreateConstructor(TypeBuilder tb) {
            var paramTypes = new Type[] { typeof(INodeSharpClient) };
            
            var cb = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                paramTypes
            );

            var proxyField = tb.DefineField(
                "proxy",
                typeof(INodeSharpClient),
                FieldAttributes.Private
            );

            var il = cb.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, 
                typeof(object).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, proxyField);
            il.Emit(OpCodes.Ret);

            return proxyField;
        }

        private MethodBuilder CreateMethod(TypeBuilder tb, FieldBuilder proxyField,
            MethodDescriptor methodDesc) {
            var paramCount = methodDesc.Arguements.Count;
            var parametersTypes = new Type[paramCount];

            for(var i = 0; i < paramCount; i++) {
                parametersTypes[i] = methodDesc.Arguements[i].Type;
            }

            var methodBuilder = tb.DefineMethod(
                methodDesc.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                CallingConventions.HasThis,
                methodDesc.Returns.Type,
                parametersTypes
            );

            var il = methodBuilder.GetILGenerator();
            var invokeMethod = proxyField.FieldType.GetMethod("Invoke")
                .MakeGenericMethod(methodDesc.Returns.Type);

            var ilParams = il.DeclareLocal(typeof(object[]));
            il.Emit(OpCodes.Ldarg_0);
            
            //Create args array
            il.Emit(OpCodes.Ldc_I4, 2);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc, ilParams);

            for(var i = 0; i < paramCount; i++) {
                il.Emit(OpCodes.Ldloc, ilParams);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i+1);
                il.Emit(OpCodes.Box, typeof(int));
                il.Emit(OpCodes.Stelem_Ref);
            }


            il.Emit(OpCodes.Ldfld, proxyField);
            il.Emit(OpCodes.Ldstr, methodDesc.Name);
            il.Emit(OpCodes.Ldloc, ilParams);
            il.Emit(OpCodes.Callvirt, invokeMethod);
            il.Emit(OpCodes.Ret);

            return methodBuilder;
        }
    }
}
