namespace Arunoki.Flow.Basics
{
  public abstract class BaseServiceExplicit : IInitializable, IStartable, IService, IResettable
  {
    private bool isStarted;
    private bool isActivated;
    private bool isInitialized;

    protected internal bool IsInitialized () => isInitialized;
    protected internal bool IsActivated () => isActivated;
    protected internal bool IsStarted () => isStarted;
    protected virtual bool AutoReset () => true;

    /// To override.
    protected virtual void OnInitialized () { }

    /// To override.
    protected virtual void OnStarted () { }

    /// To override.
    protected virtual void OnActivated () { }

    /// To override.
    protected virtual void OnDeactivated () { }

    /// To override.
    protected virtual void OnReset () { }

    bool IInitializable.IsInitialized () => isInitialized;
    bool IService.IsActivated () => isActivated;
    bool IStartable.IsStarted () => isStarted;

    void IInitializable.Initialize ()
    {
      if (!isInitialized)
      {
        OnInitialized ();
        isInitialized = true;
      }
    }

    void IStartable.Start ()
    {
      if (!isStarted)
      {
        (this as IInitializable).Initialize ();
        (this as IService).Activate ();

        OnStarted ();
        isStarted = true;
      }
    }

    void IService.Activate ()
    {
      if (!isActivated)
      {
        (this as IInitializable).Initialize ();

        OnActivated ();
        isActivated = true;
      }
    }

    void IService.Deactivate ()
    {
      if (isActivated)
      {
        OnDeactivated ();
        isActivated = false;
      }
    }

    void IResettable.Reset ()
    {
      (this as IService).Deactivate ();

      OnReset ();
      isStarted = false;
    }

    bool IResettable.AutoReset () => AutoReset ();
  }
}