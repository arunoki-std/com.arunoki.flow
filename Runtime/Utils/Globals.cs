namespace Arunoki.Flow
{
  internal static partial class Globals
  {
    public static bool IsDebug ()
    {
#if DEBUG || UNITY_EDITOR
      return true;
#else
      return false;
#endif
    }

    public static bool IsWarningsEnabled ()
    {
#if ARUNOKI_WARNINGS
      return true;
#else
      return false;
#endif
    }
  }
}