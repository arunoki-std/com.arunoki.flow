using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Globals;

using System;

namespace Arunoki.Flow
{
  public partial class FlowHub : IBuilder
  {
    internal enum BuildOrder
    {
      Any = 0,
      Managers = short.MinValue,
      Contexts = short.MinValue + 1,
      Services = short.MinValue + 2,
      Pipelines = short.MinValue + 3,
      Handlers = short.MinValue + 4
    }

    protected virtual void OnCreateBuilders (IContext context)
    {
      Elements.AddRange (this.FindProperties<IBuilder> ());

      if (context is not DummyContext)
        Elements.AddRange (context.FindProperties<IBuilder> ());

      Elements.Sort ((a, b)
        => Order (a).CompareTo (Order (b)));
    }

    protected virtual void OnInitBuilders ()
    {
      for (var i = 0; i < Elements.Count; i++)
        TryInjectDependencies (Elements [i]);
    }

    private static int Order (IBuilder x) =>
      x is BaseHubBuilder bb ? bb.GetBuildOrder () : (int) FlowHub.BuildOrder.Any;

    public bool Produce (object entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));
      var result = false;

      for (var i = 0; i < Elements.Count; i++)
      {
        var builder = Elements [i];
        result = (builder.IsConsumable (entity) && builder.Produce (entity)) || result;
      }

      return result;
    }

    public virtual void Clear (object entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));

      for (var i = 0; i < Elements.Count; i++)
      {
        var builder = Elements [i];
        if (builder.IsConsumable (entity)) builder.Produce (entity);
      }
    }

    public virtual void ClearAll ()
    {
      Events.ClearAll ();

      for (var i = 0; i < Elements.Count; i++)
        Elements [i].ClearAll ();
    }

    protected internal virtual void TryInjectDependencies (object entity)
    {
      if (entity is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (this);
      if (entity is IContextPart ctxPart && ctxPart.Get () == null) ctxPart.Set (Contexts.Root);
    }

    public bool IsConsumable (object entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));

      for (var i = 0; i < Elements.Count; i++)
        if (Elements [i].IsConsumable (entity))
          return true;

      return false;
    }
  }
}