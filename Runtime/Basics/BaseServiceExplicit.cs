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
    protected internal bool IsActivated () => activeStep > 0;
    protected internal bool IsStarted () => startStep > 0;
    protected internal virtual bool AutoReset () => true;

    bool IInitializable.IsInitialized () => IsInitialized ();
    bool IService.IsActivated () => IsActivated ();
    bool IStartable.IsStarted () => IsStarted ();
    bool IResettable.AutoReset () => AutoReset ();

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

    protected virtual void OnReset ()
    {
      (this as IService).Deactivate ();
      (TargetService as IResettable)?.Reset ();
      startStep = -1;
    }

    void IInitializable.Initialize ()
    {
      if (CanInit ())
      {
        initStep++;
        OnInitialized ();
        initStep++;
      }
    }

    void IStartable.Start ()
    {
      (this as IInitializable).Initialize ();
      (this as IService).Activate ();

      if (CanStart ())
      {
        startStep++;
        OnStarted ();
        startStep++;
      }
    }

    void IService.Activate ()
    {
      (this as IInitializable).Initialize ();

      if (CanActivate ())
      {
        activeStep++;
        OnActivate ();
        activeStep++;
      }
    }

    void IService.Deactivate ()
    {
      if (CanDeactivate ())
      {
        activeStep--;
        OnDeactivate ();
        activeStep = -1;
      }
    }

    void IResettable.Reset () => OnReset ();
  }
}