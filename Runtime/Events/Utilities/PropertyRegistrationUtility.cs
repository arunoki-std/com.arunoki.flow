using Arunoki.Flow.Misc;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Arunoki.Flow.Utilities
{
  internal static class InstancePropertyUtility
  {
    private const BindingFlags PublicFlags =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

    private static readonly Dictionary<(Type, BindingFlags), Func<IContext, EventChannel> []> GettersCache = new();

    public static void RegisterEvents (this EventChannelSet eventSet, IContext context)
    {
      RegisterEvents (eventSet, context, context.GetType (), PublicFlags);
    }

    public static void RegisterEvents (this EventChannelSet eventSet, Type staticType)
    {
      if (Utils.IsDebug () && !(staticType.IsSealed && staticType.IsAbstract))
      {
        throw new StaticManagerException (staticType);
      }

      RegisterEvents (eventSet, new ProxyTypeContext (staticType), staticType, PublicFlags);
    }

    private static void RegisterEvents (EventChannelSet eventSet, IContext context, Type contextType,
      BindingFlags bindingFlags)
    {
      var getters = GetChannelGetters (contextType, bindingFlags);

      for (int i = 0; i < getters.Length; i++)
      {
        var channel = getters [i] (context);
        if (channel == null) continue;

        eventSet.Add (channel);
        channel.InitContext (context);
      }
    }

    private static Func<IContext, EventChannel> [] GetChannelGetters (Type contextType, BindingFlags flags)
    {
      var key = (contextType, flags);
      if (GettersCache.TryGetValue (key, out var cached))
        return cached;

      var props = contextType.GetProperties (flags);
      var list = new List<Func<IContext, EventChannel>> ();

      for (int i = 0; i < props.Length; i++)
      {
        var p = props [i];

        if (!p.CanRead || p.GetIndexParameters ().Length != 0)
          continue;

        if (!typeof(EventChannel).IsAssignableFrom (p.PropertyType))
          continue;

        var getter = p.GetMethod;
        if (getter == null)
          continue;

        // getter объявлен на конкретном типе контекста, поэтому делаем делегат через reflection invoke-safe обёртку
        list.Add (ctx => (EventChannel) getter.Invoke (ctx, null)!);
      }

      var result = list.ToArray ();
      GettersCache [key] = result;
      return result;
    }
  }
}