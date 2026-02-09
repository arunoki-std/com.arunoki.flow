namespace Arunoki.Flow
{
  public partial class FlowHub
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
      (Contexts as IInitializable).Initialize ();
    }

    /// To override.
    protected virtual void OnActivated ()
    {
    }

    /// To override.
    protected virtual void OnDeactivated ()
    {
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