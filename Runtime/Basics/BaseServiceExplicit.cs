namespace Arunoki.Flow.Basics
{
  public abstract class BaseServiceExplicit : IInitializable, IStartable, IService, IResettable
  {
    private readonly object targetService;

    private bool isStarted;
    private bool isActivated;
    private bool isInitialized;

    protected BaseServiceExplicit (object targetService = null)
    {
      this.targetService = targetService;
    }

    protected internal bool IsInitialized () => isInitialized;
    protected internal bool IsActivated () => isActivated;
    protected internal bool IsStarted () => isStarted;
    protected virtual bool AutoReset () => true;

    protected virtual void OnInitialized () => (targetService as IInitializable)?.Initialize ();
    protected virtual void OnStarted () => (targetService as IStartable)?.Start ();
    protected virtual void OnActivated () => (targetService as IService)?.Activate ();
    protected virtual void OnDeactivated () => (targetService as IService)?.Deactivate ();
    protected virtual void OnReset () => (targetService as IResettable)?.Reset ();

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