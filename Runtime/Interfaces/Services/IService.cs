namespace Arunoki.Flow
{
  public interface IService
  {
    bool IsActive ();

    void Activate ();

    void Deactivate ();
  }

  public interface IManualService : IService
  {
  }
}