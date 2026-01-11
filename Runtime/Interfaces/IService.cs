namespace Arunoki.Flow
{
  public interface IService
  {
    bool IsActive { get; }

    void Activate ();
    void Deactivate ();
  }
}