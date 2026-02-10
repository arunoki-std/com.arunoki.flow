using System;

namespace Arunoki.Flow.Basics
{
  public abstract class HubPart : BaseServiceExplicit, IHubPart, IContextPart
  {
    public FlowHub Hub { get; private set; }

    public IContext Context { get; private set; }

    IContext IContextPart.Get () => Context;

    void IContextPart.Set (IContext value)
    {
      if (Context != null && value != null && Context != value)
        throw new InvalidOperationException (
          $"Trying to rewrite existing {nameof(Context)} '{Context}' by '{value}' at {this}.");

      Context = value;
    }

    FlowHub IHubPart.Get () => Hub;

    void IHubPart.Set (FlowHub value)
    {
      if (Hub != null && value != null && Hub != value)
        throw new InvalidOperationException (
          $"Trying to rewrite existing {nameof(Hub)} '{Hub}' by '{value}' at {this}.");

      Hub = value;
    }
  }
}