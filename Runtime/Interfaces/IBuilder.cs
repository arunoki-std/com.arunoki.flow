namespace Arunoki.Flow
{
  public interface IBuilder
  {
    void Build (object item);

    bool IsConsumable (object item);
  }
}