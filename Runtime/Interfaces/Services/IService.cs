namespace Arunoki.Flow
{
  public interface IService
  {
    bool IsActivated ();

    void Activate ();

    void Deactivate ();
  }

  public interface ILateService
  {
    void LateActivate ();
  }

  public interface IManualService : IService
  {
  }
}