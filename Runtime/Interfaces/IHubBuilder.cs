namespace Arunoki.Flow
{
  public interface IHubBuilder
  {
    bool Produce (object entity);
    void Clear (object entity);
    void ClearAll ();

    bool IsConsumable (object entity);
  }
}