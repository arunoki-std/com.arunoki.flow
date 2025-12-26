using UnityEngine;

namespace Arunoki.Flow
{
  internal static partial class Globals
  {
    public static int ToInt (float value) => Mathf.FloorToInt (value);

    public static uint ToUint (float value) => (uint) Mathf.FloorToInt (value);
  }
}