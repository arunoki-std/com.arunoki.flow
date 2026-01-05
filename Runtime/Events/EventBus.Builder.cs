using System;

namespace Arunoki.Flow
{
  public partial class EventBus : IBuilder
  {
    void IBuilder.Produce (object element)
    {
      if (element is IContext context) AddEventSource (context);
      if (element is IHandler handler) Subscribe (handler);
      if (element is Type staticManager && IsConsumable (staticManager))
      {
        AddEventSource (staticManager);
        Subscribe (staticManager);
      }
    }

    public bool IsConsumable (object element)
      => element is IContext || element is IHandler || element is Type type && IsConsumable (type);

    public static bool IsConsumable (Type itemType)
      => itemType.IsAbstract && itemType.IsSealed;
  }
}