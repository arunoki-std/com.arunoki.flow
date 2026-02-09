namespace Arunoki.Flow.Basics
{
  public abstract class BaseService : IInitializable, IStartable, IService, IResettable
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

    public void Initialize ()
    {
      if (isInitialized) return;

      OnInitialized ();
      isInitialized = true;
    }

    public void Start ()
    {
      if (isStarted) return;
      if (!isInitialized) Initialize ();

      OnStarted ();
      isStarted = true;
    }

    public void Activate ()
    {
      if (isActivated) return;
      if (!isStarted) Start ();

      OnActivated ();
      isActivated = true;
    }

    public void Deactivate ()
    {
      if (!isActivated) return;
      
      OnDeactivated ();
      isActivated = false;
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