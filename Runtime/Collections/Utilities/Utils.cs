namespace Arunoki.Collections.Utilities
{
    internal static class Utils
    {
        public static bool IsEditor()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        public static bool IsDebug()
        {
#if DEVELOPMENT_BUILD || (UNITY_EDITOR && DEBUG)
            return true;
#else
            return false;
#endif
        }
    }
}
