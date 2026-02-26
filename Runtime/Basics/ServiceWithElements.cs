using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
  public class ServiceWithElements<TElement> : BaseService where TElement : class
  {
    protected internal readonly List<TElement> Elements;

    public ServiceWithElements () : this (new(8)) { }

    public ServiceWithElements (List<TElement> elements)
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

    protected override void OnActivate ()
    {
      base.OnActivate ();

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is IService service && !service.IsActivated () && service is not IManualService)
          service.Activate ();
    }

    protected override void OnLateActivate ()
    {
      base.OnLateActivate ();

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is ILateService service && service is not IManualService)
          service.LateActivate ();
    }

    protected override void OnDeactivate ()
    {
      base.OnDeactivate ();

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is IService service && service.IsActivated () && service is not IManualService)
          service.Deactivate ();
    }
  }
}