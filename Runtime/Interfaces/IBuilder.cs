namespace Arunoki.Flow
{
  public interface IBuilder
  {
    bool Produce (object entity);
    void Clear (object entity);

    bool IsConsumable (object entity);
  }
}