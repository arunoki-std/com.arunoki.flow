using UnityEngine;

namespace Arunoki.Flow.Utilities
{
  public static partial class Utils
  {
    public static int ToInt (float value) => Mathf.FloorToInt (value);

    public static uint ToUint (float value) => (uint) Mathf.FloorToInt (value);
  }
}