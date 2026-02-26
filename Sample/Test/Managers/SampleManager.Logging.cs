using System;

namespace Arunoki.Flow.Sample.Managers
{
  public static partial class SampleManager
  {
    public static void Log<TReceiver> (IEvent e, string msg = "")
    {
      Log (typeof(TReceiver), e, msg);
    }

    public static void Log (Type receiver, IEvent e, string msg = "")
    {
      UnityEngine.Debug.Log ($"[{e.GetType ().Name}]\t\t {receiver} {msg}");
    }

    public static void Log (IEvent e, string msg = "")
    {
      UnityEngine.Debug.Log ($"[{e.GetType ().Name}] {msg}");
    }
  }
}