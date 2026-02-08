using Arunoki.Collections;

using System;

namespace Arunoki.Flow.Collections
{
  public abstract class BaseBuilder<TEntity> : Container<TEntity>, IHubPart, IContextPart, IBuilder
    where TEntity : class
  {
    private bool isInitialized;

    protected BaseBuilder (IContainer<TEntity> rootContainer = null) : base (rootContainer)
    {
    }

    public FlowHub Hub { get; private set; }

    public IContext Context { get; private set; }

    /// To override.
    protected virtual void OnInitialized () { }

    protected internal void Initialize ()
    {
      if (!isInitialized)
      {
        isInitialized = true;
        OnInitialized ();
        return;
      }
    }

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

    public virtual void Produce (TEntity entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));
    }

    public virtual void Clear (TEntity entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));
    }

    public virtual bool IsConsumable (TEntity entity) => entity != null;

    void IBuilder.Clear (object entity) => Clear (entity as TEntity);
    void IBuilder.Produce (object entity) => Produce (entity as TEntity);
    bool IBuilder.IsConsumable (object entity) => IsConsumable (entity as TEntity);
  }
}