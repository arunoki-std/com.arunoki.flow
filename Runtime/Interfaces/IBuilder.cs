namespace Arunoki.Flow
{
  public interface IBuilder
  {
    void Produce (object entity);
    void Clear (object entity);

    bool IsConsumable (object entity);
  }
}