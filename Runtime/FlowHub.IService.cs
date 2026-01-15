namespace Arunoki.Flow
{
  public partial class FlowHub : IService
  {
    private bool isInitialized;
    public bool IsActive { get; private set; }

    public void TryInitialize ()
    {
      if (!isInitialized)
      {
        isInitialized = true;
        OnInitialized ();
        return;
      }
    }

    /// Invoked from constructor or before first activation.
    protected virtual void OnInitialized ()
    {
      Contexts.Initialize ();
    }

    /// To override.
    protected virtual void OnActivated ()
    {
      ForEachSet<IService> (service => service.Activate ());
    }

    /// To override.
    protected virtual void OnDeactivated ()
    {
      ForEachSet<IService> (service => service.Deactivate ());
    }

    public void Activate ()
    {
      TryInitialize ();

      if (!IsActive)
      {
        IsActive = true;
        OnActivated ();
      }
    }

    public void Deactivate ()
    {
      if (IsActive)
      {
        IsActive = false;

        OnDeactivated ();
      }
    }
  }
}