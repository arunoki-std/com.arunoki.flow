using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Events
{
  //TODO: Remove partial part
  public partial class EventBus //: IBuilder
  {
    // void IBuilder.Produce (object entity)
    // {
    //   if (entity is IHandler handler) Subscribe (handler);
    //   if (entity is Type staticManager && staticManager.IsStatic ())
    //   {
    //     Subscribe (staticManager);
    //   }
    // }
    //
    // void IBuilder.Clear (object entity)
    // {
    //   if (IsConsumable (entity)) Unsubscribe (entity);
    // }
    //
    // /// Define whether is <param name="entity"></param> can be subscribed.
    // public bool IsConsumable (object entity)
    //   => entity is IHandler ||
    //      entity is Type type && type.IsStatic ();
  }
}