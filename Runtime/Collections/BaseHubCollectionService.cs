using Arunoki.Collections;

namespace Arunoki.Flow.Collections
{
  public abstract class BaseHubCollectionService<TElement> : BaseHubCollection<TElement>, IService
  {
    protected BaseHubCollectionService (IContainer<TElement> targetContainer = null) : base (targetContainer)
    {
    }

    /// To override.
    protected virtual void OnActivate () { }

    /// To override.
    protected virtual void OnDeactivate () { }

    public bool IsActive { get; private set; }

    public void Activate ()
    {
      Initialize ();

      if (!IsActive)
      {
        IsActive = true;
        OnActivate ();
      }
    }

    public void Deactivate ()
    {
      if (IsActive)
      {
        IsActive = false;
        OnDeactivate ();
      }
    }
  }
}