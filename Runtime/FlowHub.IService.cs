namespace Arunoki.Flow
{
  public partial class FlowHub : IService
  {
    public bool IsActive { get; private set; }

    /// To override.
    protected virtual void OnActivated ()
    {
      Contexts.Activate ();
      ForEachSet<IService> (service => service.Activate ());
    }

    /// To override.
    protected virtual void OnDeactivated ()
    {
      ForEachSet<IService> (service => service.Deactivate ());
      Contexts.Deactivate ();
    }

    public void Activate ()
    {
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