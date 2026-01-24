using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow
{
  public partial class EventBus : IBuilder
  {
    void IBuilder.Produce (object element)
    {
      if (element is IHandler handler) Subscribe (handler);
      if (element is Type staticManager && staticManager.IsStatic ())
      {
        Subscribe (staticManager);
      }
    }

    /// Define whether is <param name="element"></param> can be subscribed.
    public bool IsConsumable (object element)
      => element is IHandler ||
         element is Type type && type.IsStatic ();
  }
}