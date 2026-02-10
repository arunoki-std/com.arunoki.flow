namespace Arunoki.Flow.Basics
{
  public abstract class BaseService : IInitializable, IStartable, IService, IResettable
  {
    private readonly object targetService;

    private bool isStarted;
    private bool isActivated;
    private bool isInitialized;

    protected BaseService (object targetService = null)
    {
      this.targetService = targetService;
    }

    protected internal bool IsInitialized () => isInitialized;
    protected internal bool IsActivated () => isActivated;
    protected internal bool IsStarted () => isStarted;

    protected virtual void OnInitialized () => (targetService as IInitializable)?.Initialize ();
    protected virtual void OnStarted () => (targetService as IStartable)?.Start ();
    protected virtual void OnActivated () => (targetService as IService)?.Activate ();
    protected virtual void OnDeactivated () => (targetService as IService)?.Deactivate ();
    protected virtual void OnReset () => (targetService as IResettable)?.Reset ();

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
        Activate ();

        OnStarted ();
        isStarted = true;
      }
    }

    public void Activate ()
    {
      if (!isActivated)
      {
        Initialize ();

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

      OnReset ();
      isStarted = false;
    }

    public virtual bool AutoReset () => true;
  }
}