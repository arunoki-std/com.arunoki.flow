namespace Arunoki.Flow.Basics
{
  public abstract class BaseService : IInitializable, IStartable, IService, IResettable
  {
    protected object Composition;

    private int initStep = -1;
    private int startStep = -1;
    private int activeStep = -1;

    protected BaseService (object composition = null)
    {
      Composition = composition;
    }

    private bool CanInit () => initStep < 0;
    private bool CanStart () => startStep < 0;
    private bool CanActivate () => activeStep < 0;
    private bool CanDeactivate () => activeStep > 0;

    protected internal bool IsInitialized () => initStep > 0;
    protected internal bool IsActivated () => activeStep > 0;
    protected internal bool IsStarted () => startStep > 0;
    protected internal virtual bool AutoReset () => true;

    bool IInitializable.IsInitialized () => IsInitialized ();
    bool IService.IsActivated () => IsActivated ();
    bool IStartable.IsStarted () => IsStarted ();
    bool IResettable.AutoReset () => AutoReset ();

    protected virtual void OnInit () => (Composition as IInitializable)?.Initialize ();
    protected virtual void OnStarted () => (Composition as IStartable)?.Start ();

    protected virtual void OnActivate ()
    {
      if (Composition is IService service && service is not IManualService)
        service.Activate ();
    }

    protected virtual void OnDeactivate ()
    {
      if (Composition is IService service && service is not IManualService)
        service.Deactivate ();
    }

    protected virtual void OnReset ()
    {
      if (CanDeactivate ()) Deactivate ();
      (Composition as IResettable)?.Reset ();
      startStep = -1;
    }


    public void Initialize ()
    {
      if (CanInit ())
      {
        initStep = 0;
        OnInit ();
        initStep = 1;
      }
    }

    public void Start ()
    {
      Activate ();
      if (CanStart ())
      {
        startStep = 0;
        OnStarted ();
        startStep = 1;
      }
    }

    public void Activate ()
    {
      Initialize ();
      if (CanActivate ())
      {
        activeStep = 0;
        OnActivate ();
        activeStep = 1;
      }
    }

    public void Deactivate ()
    {
      if (CanDeactivate ())
      {
        activeStep = 0;
        OnDeactivate ();
        activeStep = -1;
      }
    }

    public void Reset ()
    {
      OnReset ();
    }
  }
}