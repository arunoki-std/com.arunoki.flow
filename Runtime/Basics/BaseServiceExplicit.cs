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

    bool IService.IsActivated () => isActivated;

    void IInitializable.Initialize ()
    {
      if (isInitialized) return;

      OnInitialized ();
      isInitialized = true;
    }

    void IStartable.Start ()
    {
      if (isStarted) return;

      if (!isInitialized)
        (this as IInitializable).Initialize ();

      OnStarted ();
      isStarted = true;
    }

    void IService.Activate ()
    {
      if (isActivated) return;
      if (!isStarted) (this as IStartable).Start ();

      OnActivated ();
      isActivated = true;
    }

    void IService.Deactivate ()
    {
      if (!isActivated) return;

      OnDeactivated ();
      isActivated = false;
    }

    void IResettable.Reset ()
    {
      (this as IService).Deactivate ();
      isStarted = false;

      OnReset ();
    }

    bool IResettable.AutoReset () => true;
  }
}