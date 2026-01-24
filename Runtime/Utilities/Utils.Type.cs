using System;

namespace Arunoki.Flow.Utilities
{
  public static partial class Utils
  {
    private static readonly Type UnityComponent = typeof(UnityEngine.Component);

    public static bool IsUnityComponent (this Type type) => type.IsSubclassOf (UnityComponent);

    public static bool IsStatic (this Type type) => type.IsAbstract && type.IsSealed;

    public static bool IsAbstract (this Type type) => type.IsAbstract && !type.IsSealed;

    public static bool IsConcrete (this Type type) =>
      !type.IsAbstract && !type.IsInterface && !type.ContainsGenericParameters;

    public static string [] NamespaceToArray (this Type type)
    {
      var parts = type.Namespace.Split (new [] { '.' });
      var array = new string[parts.Length];
      var prev = "";

      for (var i = 0; i < parts.Length; i++)
      {
        var curr = parts [i];
        prev = string.IsNullOrEmpty (prev) ? curr : $"{prev}.{curr}";
        array [i] = prev;
      }

      return array;
    }
  }
}