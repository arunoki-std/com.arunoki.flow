namespace Arunoki.Flow.Utilities
{
  public static partial class Utils
  {
    public static bool IsEditor ()
    {
#if UNITY_EDITOR
      return true;
#else
      return false;
#endif
    }

    public static bool IsEditorOrStandalone ()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
      return true;
#else
      return false;
#endif
    }

    public static bool IsStandalone ()
    {
#if UNITY_STANDALONE
      return true;
#else
      return false;
#endif
    }

    public static bool IsDebugBuild ()
    {
#if DEVELOPMENT_BUILD && !UNITY_EDITOR
      return true;
#else
      return false;
#endif
    }

    public static bool IsDebug ()
    {
#if DEVELOPMENT_BUILD || (UNITY_EDITOR && DEBUG)
      return true;
#else
      return false;
#endif
    }

    public static bool IsTraceable ()
    {
#if ARUNOKI_TRACE
      return true;
#else
      return false;
#endif
    }
  }
}