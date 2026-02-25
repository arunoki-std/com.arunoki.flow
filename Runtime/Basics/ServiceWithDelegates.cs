using System;

namespace Arunoki.Flow.Basics
{
  public abstract class ServiceWithDelegates : IInitializable, IStartable, IService, ILateService, IResettable
  {
    private bool isStarted;
    private bool isActivated;
    private bool isInitialized;

    protected Action OnActivate = delegate { };
    protected Action OnLateActivate = delegate { };
    protected Action OnDeactivate = delegate { };
    protected Action OnReset = delegate { };
    protected Action OnStart = delegate { };
    protected Action OnInit = delegate { };

    protected internal bool IsInitialized () => isInitialized;
    protected internal bool IsActivated () => isActivated;
    protected internal bool IsStarted () => isStarted;

    bool IInitializable.IsInitialized () => isInitialized;
    bool IService.IsActivated () => isActivated;
    bool IStartable.IsStarted () => isStarted;

    public void Initialize ()
    {
      if (!isInitialized)
      {
        OnInit?.Invoke ();
        isInitialized = true;
      }
    }

    public void Start ()
    {
      if (!isStarted)
      {
        Initialize ();

        OnStart?.Invoke ();
        isStarted = true;
      }
    }

    public void Activate ()
    {
      if (!isActivated)
      {
        Initialize ();
        Start ();

        OnActivate?.Invoke ();
        isActivated = true;
      }
    }

    public void Deactivate ()
    {
      if (isActivated)
      {
        OnDeactivate?.Invoke ();
        isActivated = false;
      }
    }

    public void LateActivate ()
    {
      if (!isActivated)
      {
        Activate ();
        OnLateActivate?.Invoke ();
      }
    }

    public void Reset ()
    {
      Deactivate ();

      isStarted = false;
      OnReset?.Invoke ();
    }

    public virtual bool AutoReset () => true;

    public virtual void Dispose ()
    {
      OnInit = null;
      OnActivate = null;
      OnDeactivate = null;
      OnLateActivate = null;
      OnReset = null;
      OnStart = null;
    }
  }
}