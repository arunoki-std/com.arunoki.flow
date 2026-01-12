namespace Arunoki.Flow.Sample.Handlers
{
  public abstract class BatteryHandler : IPipelineHandler, IContextPart
  {
    protected Battery Battery;

    IContext IContextPart.Get () => Battery;
    void IContextPart.Set (IContext context) => Battery = (Battery) context;
  }
}