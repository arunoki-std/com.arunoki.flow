using System;

namespace Arunoki.Flow
{
  public partial class EventBus : IBuilder
  {
    void IBuilder.Produce (object element)
    {
      if (element is IHandler handler) Subscribe (handler);
      if (element is Type staticManager && IsConsumable (staticManager))
      {
        Subscribe (staticManager);
      }
    }

    /// Define whether is <param name="element"></param> can be subscribed.
    public bool IsConsumable (object element)
      => element is IHandler ||
         element is Type type && IsConsumable (type);

    /// Is type static.
    public static bool IsConsumable (Type itemType)
      => itemType.IsAbstract && itemType.IsSealed;
  }
}