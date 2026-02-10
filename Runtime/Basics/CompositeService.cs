using Arunoki.Collections.Enumerators;

using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
  internal class CompositeService<TElement> : BaseService where TElement : class
  {
    protected readonly List<TElement> Elements;

    public CompositeService () : this (new()) { }

    public CompositeService (List<TElement> elements)
    {
      Elements = elements;
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      foreach (IInitializable initializer in Cast<IInitializable> ())
        if (!initializer.IsInitialized ())
          initializer.Initialize ();
    }

    protected override void OnReset ()
    {
      base.OnReset ();

      foreach (IResettable resettable in Cast<IResettable> ())
        if (resettable.AutoReset ())
          resettable.Reset ();
    }

    protected override void OnStarted ()
    {
      base.OnStarted ();

      foreach (IStartable starter in Cast<IStartable> ())
        if (!starter.IsStarted ())
          starter.Start ();
    }

    protected override void OnActivated ()
    {
      base.OnActivated ();

      foreach (IService service in Cast<IService> ())
        if (!service.IsActivated () && service is not IManualService)
          service.Activate ();
    }

    protected override void OnDeactivated ()
    {
      base.OnDeactivated ();

      foreach (IService service in Cast<IService> ())
        if (service.IsActivated () && service is not IManualService)
          service.Deactivate ();
    }

    public MutableEnumerator<TElement> GetEnumerator () => new(Elements);
    public MutableCastEnumerable<TElement, T> Cast<T> () => new(Elements);
  }
}