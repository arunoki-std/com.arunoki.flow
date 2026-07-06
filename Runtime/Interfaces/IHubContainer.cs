namespace Arunoki.Flow
{
    public interface IHubContainer
    {
        bool Register(object element);
        void Remove(object element);
        void RemoveAll();

        bool IsConsumable(object element);

        int GetBuildOrder();
    }
}
