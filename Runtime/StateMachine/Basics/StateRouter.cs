using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public abstract class StateRouter<TEntity> : IStateRouter<TEntity> where TEntity : class
  {
    private bool isInitialized;
    private bool isStarted;
    private TEntity entity;
    private StateMachine<TEntity> machine;

    protected TEntity Entity
    {
      get => entity;
      private set
      {
        Guard.ThrowIfRewrite (entity, value);
        entity = value;
      }
    }

    protected StateMachine<TEntity> Machine
    {
      get => machine;
      private set
      {
        Guard.ThrowIfRewrite (machine, value);
        machine = value;
      }
    }

    TEntity IStateRouter<TEntity>.Entity { get => Entity; set => Entity = value; }
    StateMachine<TEntity> IStateRouter<TEntity>.Machine { get => Machine; set => Machine = value; }

    protected virtual void OnInitialize () { }
    protected virtual void OnStart () { }
    protected virtual void OnReset () { }

    void IInitializable.Initialize ()
    {
      if (!isInitialized)
      {
        OnInitialize ();
        isInitialized = true;
      }
    }

    void IStartable.Start ()
    {
      if (!isStarted)
      {
        OnStart ();
        isStarted = true;
      }
    }

    void IResettable.Reset ()
    {
      isStarted = false;
      OnReset ();
    }

    bool IStartable.IsStarted () => isStarted;

    bool IResettable.AutoReset () => true;

    bool IInitializable.IsInitialized () => isInitialized;
  }
}