using System;

namespace Arunoki.Flow.Sample.Managers
{
  public static partial class SampleManager
  {
    public static void Log<T> (IEvent e, string msg = "")
    {
      Log (typeof(T), e, msg);
    }

    public static void Log (Type receiver, IEvent e, string msg = "")
    {
      UnityEngine.Debug.Log ($"[Event - {e.GetType ().Name}] {receiver} {msg}");
    }
  }
}