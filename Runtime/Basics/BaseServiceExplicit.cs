namespace Arunoki.Flow.Basics
{
  public abstract class BaseServiceExplicit : IInitializable, IStartable, IService, IResettable
  {
    private int initStep = -1;
    private int startStep = -1;
    private int activeStep = -1;

    protected object TargetService;

    protected BaseServiceExplicit (object targetService = null)
    {
      TargetService = targetService;
    }

    private bool CanInit () => initStep < 0;
    private bool CanStart () => startStep < 0;
    private bool CanActivate () => activeStep < 0;
    private bool CanDeactivate () => activeStep > 0;

    protected internal bool IsInitialized () => initStep > 0;
    protected internal virtual bool AutoReset () => true;

    public bool IsActive () => activeStep > 0;
    bool IStartable.IsStarted () => startStep > 0;
    bool IResettable.AutoReset () => AutoReset ();
    bool IInitializable.IsInitialized () => IsInitialized ();

    protected virtual void OnInit () => (TargetService as IInitializable)?.Initialize ();
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

    public virtual void Reset ()
    {
      (TargetService as IResettable)?.Reset ();
      ResetStartStep ();
    }

    protected virtual void ResetStartStep () => startStep = -1;

    void IInitializable.Initialize ()
    {
      if (CanInit ())
      {
        initStep = 0;
        OnInit ();
        initStep = 1;
      }
    }

    void IStartable.Start ()
    {
      (this as IInitializable).Initialize ();
      (this as IService).Activate ();

      if (CanStart ())
      {
        startStep = 0;
        OnStarted ();
        startStep = 1;
      }
    }

    void IService.Activate ()
    {
      (this as IInitializable).Initialize ();

      if (CanActivate ())
      {
        activeStep = 0;
        OnActivate ();
        activeStep = 1;
      }
    }

    void IService.Deactivate ()
    {
      if (CanDeactivate ())
      {
        activeStep = 0;
        OnDeactivate ();
        activeStep = -1;
      }
    }
  }
}