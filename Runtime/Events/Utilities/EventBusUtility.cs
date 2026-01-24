using Arunoki.Flow.Core;

using System;
using System.Reflection;

namespace Arunoki.Flow.Utilities
{
  internal static partial class EventBusUtility
  {
    public static void GetReactiveProperties (this EventBus events, IContext context)
      => events.GetReactiveProperties (context, context);

    public static void GetReactiveProperties (this EventBus events, IContext context, object sourceObject)
    {
      const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

      GetReactiveProperties (events, context, context.GetType (), sourceObject, flags);
    }

    public static void GetReactiveProperties (this EventBus events, Type staticType)
    {
      if (Utils.IsDebug () && !(staticType.IsSealed && staticType.IsAbstract))
        throw new StaticManagerException (staticType);

      const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

      GetReactiveProperties (events, new StaticContextWrapper (staticType), staticType, null, flags);
    }

    /// properties from static class won't be cached.
    private static void GetReactiveProperties (EventBus events, IContext context, Type sourceType,
      object sourceObject, BindingFlags bindingFlags)
    {
      var saveEntry = sourceObject != null;
      if (saveEntry)
      {
        var entry = GetOrCreateEventChannelAccessors (sourceType, bindingFlags);

        if (entry.Getters != null)
        {
          FromGetters (entry, context, sourceObject, events);
        }
        else
        {
          FromProps (entry.Props, context, sourceObject, events);
        }
      }
      else
      {
        FromProps (GetEventChannelProperties (sourceType, bindingFlags), context, null, events);
      }
    }

    private static void FromProps (PropertyInfo [] props, IContext context, object sourceObject, EventBus events)
    {
      // fallback (универсально: static игнорирует object)
      for (int i = 0; i < props.Length; i++)
      {
        if (props [i].GetValue (sourceObject) is not Channel channel)
          continue;

        events.Add (channel);
        (channel as IContextPart).Set (context);
      }
    }

    private static void FromGetters (ChannelAccessors entry, IContext context, object sourceObject, EventBus events)
    {
      var getters = entry.Getters;
      for (int i = 0; i < getters.Length; i++)
      {
        var channel = getters [i] (sourceObject); // для static sourceObject игнорируется
        if (channel == null) continue;

        events.Add (channel);
        (channel as IContextPart).Set (context);
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