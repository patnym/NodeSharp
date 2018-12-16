namespace nodesharp.core
{
    public interface INodeSharpBinder {
        T Resolve<T>() where T : INodeSharp;
    }


}
