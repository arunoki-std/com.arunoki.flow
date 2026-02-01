using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public class ProxyValue<TEvent, TData> : Channel<TEvent, TData>, IResettable
    where TEvent : struct, IDataEvent<TData>
  {
    private readonly bool autoReset;
    private readonly bool hasInitialData;
    private readonly TData initialData;

    public ProxyValue (bool autoReset = true)
    {
      initialData = default;
      hasInitialData = false;
      this.autoReset = autoReset;
    }

    public ProxyValue (TData data, bool autoReset = true)
    {
      initialData = data;
      hasInitialData = true;
      this.autoReset = autoReset;

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

    public bool AutoReset () => autoReset;

    protected virtual void SetDataValue (ref TData data)
    {
      Data = data;
      IsNotEmpty = true;
    }
  }
}