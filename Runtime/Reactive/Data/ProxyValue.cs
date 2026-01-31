using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public class ProxyValue<TEvent, TData> : Channel<TEvent, TData>, IResettable
    where TEvent : struct, IDataEvent<TData>
  {
    private readonly bool hasInitialData;
    private readonly TData initialData;

    public ProxyValue ()
    {
      initialData = default;
      hasInitialData = false;
    }

    public ProxyValue (TData data)
    {
      initialData = data;
      hasInitialData = true;

      SetDataValue (ref data);
    }

    public TData Data { get; private set; }

    public bool IsNotEmpty { get; private set; }

    public override void Publish (ref TData data)
    {
      SetDataValue (ref data);
      base.Publish (ref data);
    }

    public virtual void Reset ()
    {
      if (hasInitialData)
      {
        Data = initialData;
        IsNotEmpty = true;
      }
      else
      {
        Data = default;
        IsNotEmpty = false;
      }
    }

    protected virtual void SetDataValue (ref TData data)
    {
      Data = data;
      IsNotEmpty = true;
    }
  }
}