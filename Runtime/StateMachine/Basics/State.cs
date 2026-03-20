using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow
{
  public abstract class State<TEntity> : IState<TEntity> where TEntity : class
  {
    protected readonly Type ParentState;
    private readonly bool isDefault;

    private TEntity context;

    protected State () { }

    protected State (bool isDefault)
    {
      this.isDefault = isDefault;
    }

    protected State (Type parentState)
    {
      ParentState = parentState;
    }

    protected State (bool isDefault, Type parentState)
    {
      this.isDefault = isDefault;
      ParentState = parentState;
    }

    public TEntity Context
    {
      get => context;
      private set
      {
        Guard.ThrowIfRewrite (context, value);
        context = value;
      }
    }

    public abstract void OnEnter ();
    public abstract void OnExit ();
    public abstract void OnUpdate ();

    TEntity IState<TEntity>.Context { get => Context; set => Context = value; }
    public bool IsDefault () => isDefault;
    public bool IsSubstate () => ParentState != null;

    public bool IsSubstateOf (out Type parentType)
    {
      parentType = ParentState;
      return parentType != null;
    }

    /// <summary> Transition is locked.  </summary>
    public virtual bool IsProcessing () => false;
  }
}