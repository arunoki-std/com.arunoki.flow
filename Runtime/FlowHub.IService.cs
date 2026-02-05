namespace Arunoki.Flow
{
  public partial class FlowHub : IService
  {
    private bool isInitialized;
    public bool IsActive { get; private set; }

    public void Initialize ()
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

      foreach (var service in Services)
        service.Activate ();
    }

    /// To override.
    protected virtual void OnDeactivated ()
    {
      ForEachSet<IService> (service => service.Deactivate ());

      foreach (var service in Services)
        service.Deactivate ();
    }

    public void Activate ()
    {
      Initialize ();

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