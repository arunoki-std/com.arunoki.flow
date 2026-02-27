using Arunoki.Flow.Utilities;

namespace Arunoki.Flow.Basics
{
  public abstract class HubPart : BaseService, IHubPart, IContextPart
  {
    public FlowHub Hub { get; private set; }

    public IContext Context { get; private set; }

    protected override void OnInit ()
    {
      if (Hub == null)
        throw new BuildOperationException ($"'{GetType ()}' is supposed to be a part of the '{nameof(FlowHub)}'.");

      if (Context == null)
        throw new BuildOperationException ($"'{GetType ()}' is supposed to be a part of the '{nameof(Context)}'.");

      base.OnInit ();
    }

    IContext IContextPart.Get () => Context;

    void IContextPart.Set (IContext value)
    {
      Guard.ThrowIfRewrite (Context, value);

      Context = value;
    }

    FlowHub IHubPart.Get () => Hub;

    void IHubPart.Set (FlowHub value)
    {
      Guard.ThrowIfRewrite (Hub, value);

      Hub = value;
    }
  }
}