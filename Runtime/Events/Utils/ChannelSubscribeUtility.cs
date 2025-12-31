using Arunoki.Flow.Misc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arunoki.Flow.Utils
{
  internal static class ChannelSubscribeUtility
  {
    private static readonly Type BaseEventType = typeof(IEvent);

    public static void Subscribe (this EventChannelSet set, object receiver,
      Func<object, MethodInfo [], EventsHandler> createCallback)
    {
      Type receiverType;
      BindingFlags bindingFlags;
      if (receiver is Type type)
      {
        receiverType = type;
        bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;
      }
      else
      {
        receiverType = receiver.GetType ();
        bindingFlags = BindingFlags.Instance | BindingFlags.Public;
      }

      foreach (var kvp in GetMethodsGroup (receiverType, bindingFlags))
      {
        if (set.TryGet (kvp.Item1, out var channel))
        {
          channel.Subscribe (createCallback (receiver, kvp.Item2));
        }
        else if (Globals.IsWarningsEnabled ())
        {
          UnityEngine.Debug.LogWarning (
            $"Event hub does not contain any channel capable of handling '{kvp.Item1}'.\n" +
            $"The methods of '{receiver.GetType ()}' will not be invoked: \n{Globals.JoinAsList (kvp.Item2)}. "
          );
        }
      }
    }

    public static void Unsubscribe (this EventChannelSet set, object receiver)
    {
      set.Set.ForEach (channel =>
        channel.Handlers.Where (callback =>
          callback.IsTarget (receiver), channel.Unsubscribe)
      );
    }

    private static IEnumerable<(Type, MethodInfo [])> GetMethodsGroup (Type receiverType, BindingFlags bindingFlags)
    {
      return receiverType.GetMethods (bindingFlags)
        .Where (info =>
        {
          var parameters = info.GetParameters ();
          return parameters.Length == 1 && BaseEventType.IsAssignableFrom (parameters [0].ParameterType);
        })
        .GroupBy (info => info.GetParameters () [0].ParameterType)
        .Select (grouping => (Type: grouping.Key, Methods: grouping.ToArray ()))
        .ToArray ();
    }
  }
}