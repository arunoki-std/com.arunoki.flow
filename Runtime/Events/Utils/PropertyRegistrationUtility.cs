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

    public static void RegisterEvents (this EventChannelSet eventSet, IContext context)
    {
      RegisterEvents (eventSet, context, context.GetType (), PublicFlags);
    }

    public static void RegisterEvents (this EventChannelSet eventSet, Type staticType)
    {
      if (Globals.IsDebug () && !(staticType.IsSealed && staticType.IsAbstract))
      {
        throw new StaticManagerException (staticType);
      }

      RegisterEvents (eventSet, new ProxyTypeContext (staticType), staticType, PublicFlags);
    }

    private static void RegisterEvents (EventChannelSet eventSet, IContext context,
      Type contextType,
      BindingFlags bindingFlags)
    {
      //TODO: LINQ remove
      var eventChannels = contextType
        .GetProperties (bindingFlags)
        .Where (p => ChannelType.IsAssignableFrom (p.PropertyType))
        .Select (p => p.GetValue (context))
        .Cast<EventChannel> ()
        .ToArray ();

      foreach (var channel in eventChannels)
      {
        eventSet.Add (channel);
        channel.InitContext (context);
      }
    }
  }
}