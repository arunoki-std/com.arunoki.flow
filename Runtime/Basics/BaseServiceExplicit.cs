namespace Arunoki.Flow.Basics
{
  public abstract class BaseServiceExplicit : IInitializable, IStartable, IService, IResettable
  {
    protected object TargetService;

    private bool isStarted;
    private bool isActivated;
    private bool isInitialized;

    protected BaseServiceExplicit (object targetService = null)
    {
      TargetService = targetService;
    }

    protected internal bool IsInitialized () => isInitialized;
    protected internal bool IsActivated () => isActivated;
    protected internal bool IsStarted () => isStarted;
    protected virtual bool AutoReset () => true;

    protected virtual void OnInitialized () => (TargetService as IInitializable)?.Initialize ();
    protected virtual void OnStarted () => (TargetService as IStartable)?.Start ();

    protected virtual void OnActivate ()
    {
      if (TargetService is IService service && service is not IManualService)
        service.Activate ();
    }

    protected virtual void OnDeactivate ()
    {
      if (TargetService is IService service && service is not IManualService)
        service.Deactivate ();
    }

    protected virtual void OnReset () => (TargetService as IResettable)?.Reset ();

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

        OnStarted ();
        isStarted = true;
      }
    }

    void IService.Activate ()
    {
      if (!isActivated)
      {
        (this as IInitializable).Initialize ();
        (this as IStartable).Start ();

        OnActivate ();
        isActivated = true;
      }
    }

    void IService.Deactivate ()
    {
      if (isActivated)
      {
        OnDeactivate ();
        isActivated = false;
      }
    }

    void IResettable.Reset ()
    {
      (this as IService).Deactivate ();

      isStarted = false;
      OnReset ();
    }

    bool IResettable.AutoReset () => AutoReset ();
  }
}