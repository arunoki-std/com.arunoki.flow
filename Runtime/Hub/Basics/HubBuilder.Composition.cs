namespace Arunoki.Flow.Basics
{
  public abstract partial class HubBuilder<TElement>
  {
    private readonly ServiceContainer<TElement> composition;

    /// Init all elements at composition if <see cref="IsCompositionInitializable"/>.
    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      if (IsCompositionInitializable ()) composition.Initialize ();
    }

    /// Reset all elements at composition if <see cref="IsCompositionResettable"/>.
    protected override void OnReset ()
    {
      base.OnReset ();

      if (IsCompositionResettable ()) composition.Reset ();
    }

    /// Start all elements at composition if <see cref="IsCompositionStartable"/>.
    protected override void OnStarted ()
    {
      base.OnStarted ();

      if (IsCompositionStartable ()) composition.Start ();
    }

    /// Activate all elements at composition if <see cref="IsCompositionServiceAvailable"/>.
    protected override void OnActivated ()
    {
      base.OnActivated ();

      if (IsCompositionServiceAvailable ()) composition.Activate ();
    }

    /// Deactivate all elements at composition if <see cref="IsCompositionServiceAvailable"/>.
    protected override void OnDeactivated ()
    {
      base.OnDeactivated ();

      if (IsCompositionServiceAvailable ()) composition.Deactivate ();
    }

    protected virtual bool IsCompositionInitializable () => true;
    protected virtual bool IsCompositionResettable () => true;
    protected virtual bool IsCompositionStartable () => true;
    protected virtual bool IsCompositionServiceAvailable () => true;
  }
}