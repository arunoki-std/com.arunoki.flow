using System.Reflection;

namespace Arunoki.Flow.Misc
{
  public class Callback
  {
    protected MethodInfo [] Methods;

    public Callback (object receiver, MethodInfo [] methods)
    {
      Receiver = receiver;
      Methods = methods;
    }

    public object Receiver { get; private set; }

    public void OnCallback (object message)
    {
      var target = GetTargetInstance ();

      if (target is IActiveEventReceiver er && !er.IsHandlingEvents)
        return;

      for (var i = 0; i < Methods.Length; i++)
        Methods [i].Invoke (target, new [] { message });
    }

    public void Dispose ()
    {
      Methods = null;
      Receiver = null;
    }

    public virtual bool IsReceiver (object target)
    {
      return target is IEventReceiver && Receiver == target;
    }

    public virtual object GetTargetInstance () => Receiver;
  }
}