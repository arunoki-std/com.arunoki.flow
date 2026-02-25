namespace Arunoki.Flow
{
  public interface IHandler
  {
  }

  public interface IActiveHandler : IHandler
  {
    bool IsHandlingEvents { get; set; }

    void OnActivated ();

    void OnDeactivated ();
  }

  public interface ILateHandler : IHandler
  {
    void OnLateActivate ();
  }

  public interface IResettableHandler : IHandler
  {
    void OnReset ();
  }
}