namespace Arunoki.Flow
{
  public interface IBuilder
  {
    bool Produce (object entity);
    void Clear (object entity);
    void ClearAll ();

    bool IsConsumable (object entity);
  }
}