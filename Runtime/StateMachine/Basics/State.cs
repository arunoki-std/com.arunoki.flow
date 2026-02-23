using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow
{
  public abstract class State<TEntity> : IState<TEntity> where TEntity : class
  {
    protected readonly Type ParentType;
    protected readonly bool IsDefault;
    private TEntity entity;

    protected State (bool isDefault, Type parentType)
    {
      IsDefault = isDefault;
      ParentType = parentType;
    }

    protected TEntity Entity
    {
      get => entity;
      private set
      {
        Guard.ThrowIfRewrite (entity, value);
        entity = value;
      }
    }

    public abstract void OnEnter ();
    public abstract void OnExit ();
    public abstract void OnUpdate ();

    TEntity IState<TEntity>.Entity { get => Entity; set => Entity = value; }
    bool IState<TEntity>.IsDefault () => IsDefault;
    bool IState<TEntity>.IsSubState () => ParentType != null;
    Type IState<TEntity>.GetParentType () => ParentType;
  }
}