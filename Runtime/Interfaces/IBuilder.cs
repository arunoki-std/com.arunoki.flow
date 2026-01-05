namespace Arunoki.Flow
{
  public interface IBuilder
  {
    void Produce (object element);

    bool IsConsumable (object element);
  }
}