using Arunoki.Collections;

namespace Arunoki.Flow.Collections
{
  public abstract class BaseBuilderService<TElement> : BaseBuilder<TElement>, IService where TElement : class
  {
    protected BaseBuilderService(IContainer<TElement> rootContainer) : base(rootContainer)
    {
    }

    /// To override.
    protected virtual void OnActivate () { }

    /// To override.
    protected virtual void OnDeactivate () { }

    bool IService.IsActive => IsActive;
    protected internal bool IsActive { get; private set; }

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