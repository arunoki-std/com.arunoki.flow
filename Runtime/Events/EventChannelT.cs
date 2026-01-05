using Arunoki.Flow.Misc;

using System.Reflection;

namespace Arunoki.Flow
{
  public class EventChannel<TEvent> : EventChannel
    where TEvent : struct, IEvent
  {
    public event RefAction<TEvent> OnEvent;

    public EventChannel () : base (typeof(TEvent))
    {
    }

    protected internal override void Subscribe (object target, MethodInfo [] methods)
    {
      var e = $"{target}: ";
      foreach (var m in methods) e += (m.Name + ",");
      UnityEngine.Debug.Log ($"{GetType ().Name} ({typeof(TEvent)}) => {e}");

      Add (new Callback<TEvent> (target, methods));
    }

    /// Remove all subscribers.
    public override void Clear ()
    {
      base.Clear ();

      OnEvent = null;
    }

    public virtual void Publish ()
    {
      TEvent evt = GetEventInstance ();

      Publish (ref evt);
    }

    protected virtual void Publish (ref TEvent evt)
    {
      for (var index = Elements.Count - 1; index >= 0; index--)
        (Elements [index] as Callback<TEvent>).Publish (ref evt);

      if (OnEvent != null)
      {
        OnEvent (ref evt);
      }
    }

    protected virtual TEvent GetEventInstance ()
    {
      return new TEvent { Context = this.Context };
    }
  }
}