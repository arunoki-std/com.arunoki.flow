namespace Arunoki.Flow
{
  public interface IService
  {
    bool IsActivated ();

    void Activate ();

    void Deactivate ();
  }

  public interface ILateService : IService
  {
    void OnLateActivate ();
  }

  public interface IManualService : IService
  {
  }
}