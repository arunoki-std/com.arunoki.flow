using System;
using System.Collections.Generic;
using System.Reflection;

namespace Arunoki.Flow.Utils
{
  public static class ReflectionUtils
  {
    private const BindingFlags PublicFlags =
      BindingFlags.Public | BindingFlags.Instance;

    private const BindingFlags ClassFlags =
      BindingFlags.Public | BindingFlags.NonPublic;

    public static List<T> GetAllPropertiesWithNested<T> (this object source, BindingFlags flags = PublicFlags)
    {
      var result = new List<T> ();
      GetNestedTypes (source, result, flags);
      return result;
    }

    private static void GetNestedTypes<T> (object source, List<T> list, BindingFlags flags)
    {
      foreach (var obj in source.GetAllProperties<T> (flags))
      {
        list.Add (obj);
        GetNestedTypes (obj, list, flags);
      }
    }

    public static IEnumerable<T> GetAllProperties<T> (this object source, BindingFlags flags = PublicFlags)
    {
      var lookingType = typeof(T);
      var isType = source is Type;
      var sourceType = isType ? source as Type : source.GetType ();
      var sourceObject = isType ? null : source;
      if (isType) flags |= BindingFlags.Static; // fix issue with static singleton field

      foreach (var property in sourceType.GetProperties (flags))
      {
        if (property.PropertyType == lookingType || lookingType.IsAssignableFrom (property.PropertyType))
        {
          var value = (T) property.GetValue (sourceObject);

          if (value != null)
            yield return value;
        }
      }
    }

    public static IEnumerable<Type> GetNestedTypes<T> (this Type sourceType, BindingFlags flags = ClassFlags)
    {
      var baseType = typeof(T);

      foreach (var type in sourceType.GetNestedTypes (flags))
      {
        if (!type.IsAbstract && (type == baseType || baseType.IsAssignableFrom (type)))
          yield return type;
      }
    }
  }
}