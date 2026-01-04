using Arunoki.Flow.Misc;

using System;
using System.Reflection;

namespace Arunoki.Flow.Utilities
{
  internal static partial class EventsReflectionUtility
  {
    public static void RegisterEvents (this EventChannelSet eventSet, IContext context)
      => eventSet.RegisterEvents (context, context);

    public static void RegisterEvents (this EventChannelSet eventSet, IContext context, object sourceObject)
    {
      const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

      RegisterEvents (eventSet, context, context.GetType (), sourceObject, flags);
    }

    public static void RegisterEvents (this EventChannelSet eventSet, Type staticType)
    {
      if (Utils.IsDebug () && !(staticType.IsSealed && staticType.IsAbstract))
        throw new StaticManagerException (staticType);

      const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

      RegisterEvents (eventSet, new ProxyTypeContext (staticType), staticType, null, flags);
    }

    private static void RegisterEvents (EventChannelSet eventSet, IContext context, Type sourceType,
      object sourceObject, BindingFlags bindingFlags)
    {
      var entry = GetOrCreateEventChannelAccessors (sourceType, bindingFlags);

      var getters = entry.Getters;
      if (getters != null)
      {
        for (int i = 0; i < getters.Length; i++)
        {
          var channel = getters [i] (sourceObject); // для static sourceObject игнорируется
          if (channel == null) continue;

          eventSet.Add (channel);
          channel.InitContext (context);
        }

        return;
      }

      // fallback (универсально: static игнорирует object)
      var props = entry.Props;
      for (int i = 0; i < props.Length; i++)
      {
        if (props [i].GetValue (sourceObject) is not EventChannel channel)
          continue;

        eventSet.Add (channel);
        channel.InitContext (context);
      }
    }

    #region Obsolete LINQ

    // private static void RegisterEvents (EventChannelSet eventSet, IContext context, Type contextType, BindingFlags bindingFlags)
    // {
    //   foreach (var channel in GetEventChannels (contextType, bindingFlags, context))
    //   {
    //     eventSet.Add (channel);
    //     channel.InitContext (context);
    //   }
    // }
    //
    // private static EventChannel [] GetEventChannels (Type sourceType, BindingFlags bindingFlags, object sourceObject = null)
    // {
    //   return sourceType
    //     .GetProperties (bindingFlags)
    //     .Where (p => typeof(EventChannel).IsAssignableFrom (p.PropertyType))
    //     .Select (p => p.GetValue (sourceObject))
    //     .Cast<EventChannel> ()
    //     .ToArray ();
    // }

    #endregion
  }
}