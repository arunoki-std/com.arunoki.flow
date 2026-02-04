using UnityEngine;

namespace Arunoki.Flow
{
  public class ProgressProperty<TEvent> : FloatProperty<TEvent> where TEvent : struct, IValueEvent<float>
  {
    public ProgressProperty (bool autoReset = true) : base (autoReset)
    {
    }

    protected override bool TryChange (ref float value)
    {
      value = UnityEngine.Mathf.Clamp01 (value);
      return base.TryChange (ref value);
    }

    public bool IsReady () => Mathf.Approximately (Value, 1.0f);
  }
}