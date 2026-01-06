#nullable enable

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Arunoki.Flow.Utilities
{
  internal static partial class EventBusUtility
  {
    private static readonly Dictionary<(Type, BindingFlags), ChannelAccessors> Cache = new();

    public static ChannelAccessors GetOrCreateEventChannelAccessors (Type sourceType, BindingFlags bindingFlags)
    {
      var key = (sourceType, bindingFlags);
      if (Cache.TryGetValue (key, out var entry))
        return entry;

      entry = new ChannelAccessors { Props = GetEventChannelProperties (sourceType, bindingFlags), Getters = null };
      if (entry.Props.Length != 0 && IsExpressionSupported ())
      {
        try
        {
          entry.Getters = BuildGetters (sourceType, entry.Props);
        }
        catch (PlatformNotSupportedException) { entry.Getters = null; }
        catch (NotSupportedException) { entry.Getters = null; }
        catch (InvalidOperationException) { entry.Getters = null; }
      }

      Cache [key] = entry;
      return entry;
    }

    private static PropertyInfo [] GetEventChannelProperties (Type sourceType, BindingFlags bindingFlags)
    {
      var props = sourceType.GetProperties (bindingFlags);
      var list = new List<PropertyInfo> (4);

      for (int i = 0; i < props.Length; i++)
      {
        var p = props [i];
        if (!p.CanRead) continue;
        if (p.GetIndexParameters ().Length != 0) continue;
        if (!typeof(EventChannel).IsAssignableFrom (p.PropertyType)) continue;
        list.Add (p);
      }

      return list.ToArray ();
    }

    private static Func<object, EventChannel> [] BuildGetters (Type sourceType, PropertyInfo [] props)
    {
      var getters = new Func<object, EventChannel>[props.Length];

      var srcParam = Expression.Parameter (typeof(object), "src");

      for (int i = 0; i < props.Length; i++)
      {
        var p = props [i];
        var get = p.GetMethod!;
        Expression access;

        if (get.IsStatic)
        {
          // static: SourceType.Channel
          access = Expression.Property (null, p);
        }
        else
        {
          // instance: ((DeclaringType)src).Channel
          var declaring = p.DeclaringType ?? sourceType;
          var cast = Expression.Convert (srcParam, declaring);
          access = Expression.Property (cast, p);
        }

        var toBase = Expression.Convert (access, typeof(EventChannel));
        getters [i] = Expression.Lambda<Func<object, EventChannel>> (toBase, srcParam).Compile ();
      }

      return getters;
    }

    internal sealed class ChannelAccessors
    {
      /// Fast way
      public Func<object, EventChannel> []? Getters;

      /// Fallback
      public PropertyInfo [] Props = Array.Empty<PropertyInfo> ();
    }
  }
}