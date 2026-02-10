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