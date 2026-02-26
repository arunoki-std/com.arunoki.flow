namespace Arunoki.Flow.Basics
{
  public abstract class BaseService : IInitializable, IStartable, IService, IResettable, ILateService
  {
    protected object Composition;

    private bool isStarted;
    private bool isActivated;
    private bool isInitialized;

    protected BaseService (object composition = null)
    {
      Composition = composition;
    }

    protected internal bool IsInitialized () => isInitialized;
    protected internal bool IsActivated () => isActivated;
    protected internal bool IsStarted () => isStarted;

    protected virtual void OnInitialized () => (Composition as IInitializable)?.Initialize ();
    protected virtual void OnStarted () => (Composition as IStartable)?.Start ();

    protected virtual void OnActivate ()
    {
      if (Composition is IService service && service is not IManualService)
        service.Activate ();
    }

    protected virtual void OnLateActivate ()
    {
      if (Composition is ILateService service && service is not IManualService)
        service.LateActivate ();
    }

    protected virtual void OnDeactivate ()
    {
      if (Composition is IService service && service is not IManualService)
        service.Deactivate ();
    }

    protected virtual void OnReset () => (Composition as IResettable)?.Reset ();

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

        OnActivate ();
        isActivated = true;
      }
    }

    public void Deactivate ()
    {
      if (isActivated)
      {
        OnDeactivate ();
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

    void ILateService.LateActivate ()
    {
      if (isActivated) OnLateActivate ();
    }
  }
}