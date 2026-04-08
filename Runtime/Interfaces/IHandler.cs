namespace Arunoki.Flow
{
  public interface IHandler
  {
  }

  public interface IGlobalEventsHandler : IHandler
  {
  }

  public interface IConditionHandler : IHandler
  {
    bool IsHandlingEvents { get; }
  }

  public interface IServiceHandler : IConditionHandler
  {
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