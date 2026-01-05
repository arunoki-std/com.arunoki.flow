namespace Arunoki.Flow
{
  /// One type of event per data
  public class EventChannel<TEvent, TData> : EventChannel<TEvent>
    where TEvent : struct, IDataEvent<TData>
    where TData : struct
  {
    public void Publish (ref TData data)
    {
      var evt = GetEventInstance ();
      evt.Data = data;

      Publish (ref evt);
    }
  }
}