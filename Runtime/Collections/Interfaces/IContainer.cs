namespace Arunoki.Collections
{
    public interface IContainer<in T>
    {
        void OnAdded(T element);

        void OnRemoved(T element);
    }
}
