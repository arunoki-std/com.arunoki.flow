using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
  internal class ServiceContainer<TElement> : BaseService where TElement : class
  {
    protected readonly List<TElement> Elements;

    public ServiceContainer () : this (new()) { }

    public ServiceContainer (List<TElement> elements)
    {
      Elements = elements;
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is IInitializable initializer && !initializer.IsInitialized ())
          initializer.Initialize ();
    }

    protected override void OnReset ()
    {
      base.OnReset ();

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is IResettable resettable && resettable.AutoReset ())
          resettable.Reset ();
    }

    protected override void OnStarted ()
    {
      base.OnStarted ();

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is IStartable starter && !starter.IsStarted ())
          starter.Start ();
    }

    protected override void OnActivated ()
    {
      base.OnActivated ();

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is IService service && !service.IsActivated () && service is not IManualService)
          service.Activate ();
    }

    protected override void OnDeactivated ()
    {
      base.OnDeactivated ();

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is IService service && service.IsActivated () && service is not IManualService)
          service.Deactivate ();
    }
  }
}