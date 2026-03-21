using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow
{
  public abstract class State<TContext> : IState<TContext> where TContext : class
  {
    protected readonly Type ParentState;
    private readonly bool isDefault;

    private bool isStarted;
    protected bool IsFirstUpdatePassed { get; private set; }

    private TContext context;

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

    public TContext Context
    {
      get => context;
      private set
      {
        Guard.ThrowIfRewrite (context, value);
        context = value;
      }
    }

    public virtual void OnEnter () { }

    public virtual void OnExit ()
    {
      IsFirstUpdatePassed = false;
      isStarted = false;
    }

    public virtual void OnUpdate ()
    {
      if (isStarted && !IsFirstUpdatePassed) IsFirstUpdatePassed = true;
      if (!isStarted)
      {
        isStarted = true;
        OnStart ();
      }
    }

    /// First update of the <see cref="OnUpdate"/> loop. 
    protected virtual void OnStart ()
    {
    }

    TContext IState<TContext>.Context { get => Context; set => Context = value; }
    public bool IsDefault () => isDefault;
    public bool IsSubstate () => ParentState != null;

    public bool IsSubstateOf (out Type parentType)
    {
      parentType = ParentState;
      return parentType != null;
    }

    public virtual bool IsReadyGoNext () => true;
  }
}