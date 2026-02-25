namespace Arunoki.Flow.Basics
{
  public abstract class BaseService : IInitializable, IStartable, IService, IResettable
  {
    protected object TargetService;

    private bool isStarted;
    private bool isActivated;
    private bool isInitialized;

    protected BaseService (object targetService = null)
    {
      TargetService = targetService;
    }

    protected internal bool IsInitialized () => isInitialized;
    protected internal bool IsActivated () => isActivated;
    protected internal bool IsStarted () => isStarted;

    protected virtual void OnInitialized () => (TargetService as IInitializable)?.Initialize ();
    protected virtual void OnStarted () => (TargetService as IStartable)?.Start ();

    protected virtual void OnActivated ()
    {
      if (TargetService is IService service && service is not IManualService)
        service.Activate ();
    }

    protected virtual void OnDeactivated ()
    {
      if (TargetService is IService service && service is not IManualService)
        service.Deactivate ();
    }

    protected virtual void OnReset () => (TargetService as IResettable)?.Reset ();

    bool IInitializable.IsInitialized () => isInitialized;
    bool IService.IsActivated () => isActivated;
    bool IStartable.IsStarted () => isStarted;

    public void Initialize ()
    {
      if (!isInitialized)
      {
        OnInitialized ();
        isInitialized = true;
      }
    }

    public void Start ()
    {
      if (!isStarted)
      {
        Initialize ();

        OnStarted ();
        isStarted = true;
      }
    }

    public void Activate ()
    {
      if (!isActivated)
      {
        Initialize ();
        Start ();

        OnActivated ();
        isActivated = true;
      }
    }

    public void Deactivate ()
    {
      if (isActivated)
      {
        OnDeactivated ();
        isActivated = false;
      }
    }

    public void Reset ()
    {
      Deactivate ();

      isStarted = false;
      OnReset ();
    }

    public virtual bool AutoReset () => true;
  }
}