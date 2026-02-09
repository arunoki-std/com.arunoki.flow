using System;

namespace Arunoki.Flow
{
  public partial class FlowHub : IBuilder
  {
    public void Produce (object entity)
    {
      // TryInjectDependencies (entity);
      // ForEachSet<IBuilder> (builder =>
      // {
      //   if (builder.IsConsumable (entity)) builder.Produce (entity);
      // });
    }

    public virtual void Clear (object entity)
    {
      // ForEachSet<IBuilder> (builder =>
      // {
      //   if (builder.IsConsumable (entity)) builder.Clear (entity);
      // });
    }

    public virtual void ClearAll ()
    {
      
    }

    protected virtual void TryInjectDependencies (object entity)
    {
      if (entity == null) throw new NullReferenceException (nameof(entity));
      if (entity is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (this);
      if (entity is IContextPart ctxPart && ctxPart.Get () == null) ctxPart.Set (EntityContext);
    }

    public bool IsConsumable (object entity) => false;
      // => ForAnySet<IBuilder> (builder => builder.IsConsumable (entity));
  }
}