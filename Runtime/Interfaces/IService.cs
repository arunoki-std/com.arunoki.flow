namespace Arunoki.Flow
{
  public interface IService
  {
    bool IsActivated ();

    void Activate ();

    void Deactivate ();
  }

  public interface IManualService : IService
  {
  }
}