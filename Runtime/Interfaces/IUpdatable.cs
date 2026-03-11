namespace Arunoki.Flow
{
  public interface IUpdatable
  {
    void Update ();
  }

  public interface IFixedUpdatable : IUpdatable
  {
  }

  public interface ILateUpdatable : IUpdatable
  {
  }
}