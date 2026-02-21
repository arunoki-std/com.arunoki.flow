namespace Arunoki.Flow
{
  public interface IHubBuilder
  {
    bool Build (object entity);
    void Clear (object entity);
    void ClearAll ();

    bool IsConsumable (object entity);
  }
}