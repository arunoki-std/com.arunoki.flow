namespace Arunoki.Flow
{
  public interface IBuilder
  {
    void Build (object element);

    bool IsConsumable (object element);
  }
}