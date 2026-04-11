namespace Arunoki.Flow
{
  public interface IFlowHandler
  {
  }

  public interface IFlowGlobalHandler : IFlowHandler
  {
  }

  public interface IFlowConditionHandler : IFlowHandler
  {
    bool IsHandlingEvents { get; }
  }

  public interface IFlowServiceHandler : IFlowConditionHandler
  {
    void OnActivated ();

    void OnDeactivated ();
  }

  public interface IFlowResettableHandler : IFlowHandler
  {
    void OnReset ();
  }
}