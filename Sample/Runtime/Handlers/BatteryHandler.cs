namespace Arunoki.Flow.Sample.Controllers
{
  public abstract class BaseBatteryHandler<TContext> : IPipelineHandler, IContextPart where TContext : Battery
  {
    protected TContext Battery;

    IContext IContextPart.Get () => Battery;
    void IContextPart.Set (IContext context) => Battery = (TContext) context;
  }
}