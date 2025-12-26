using Arunoki.Flow.Misc;

using System;
using System.Linq;
using System.Reflection;

namespace Arunoki.Flow.Utils
{
  internal static class InstancePropertyUtility
  {
    private const BindingFlags PublicFlags =
      BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

    private static readonly Type ChannelType = typeof(IEventChannel);

    public static void RegisterEvents (this EventChannelCollection eventCollection, IEventsContext context)
    {
      RegisterEvents (eventCollection, context, context.GetType (), PublicFlags);
    }

    public static void RegisterEvents (this EventChannelCollection eventCollection, Type staticType)
    {
      if (Globals.IsDebug () && !(staticType.IsSealed && staticType.IsAbstract))
      {
        throw new ArgumentException (
          $"{staticType} is not static class. Reactive properties would not be initialized.");
      }

      RegisterEvents (eventCollection, new ProxyTypeContext (staticType), staticType, PublicFlags);
    }

    private static void RegisterEvents (EventChannelCollection eventCollection, IEventsContext context,
      Type contextType,
      BindingFlags bindingFlags)
    {
      var eventChannels = contextType
        .GetProperties (bindingFlags)
        .Where (p => ChannelType.IsAssignableFrom (p.PropertyType))
        .Select (p => p.GetValue (context))
        .Cast<EventChannel> ()
        .ToArray ();

      foreach (var channel in eventChannels)
      {
        eventCollection.Add (channel);
        channel.InitContext (context);
      }
    }
  }
}