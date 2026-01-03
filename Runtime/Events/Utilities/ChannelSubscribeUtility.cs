using Arunoki.Flow.Misc;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Arunoki.Flow.Utilities
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

      var groups = GetMethodsGroup (receiverType, bindingFlags);

      for (int i = 0; i < groups.Length; i++)
      {
        (Type eventType, MethodInfo [] methods) = groups [i];

        if (set.TryGet (eventType, out var channel))
        {
          channel.Subscribe (createCallback (receiver, methods));
        }
        else if (Utils.IsWarningsEnabled ())
        {
          UnityEngine.Debug.LogWarning (
            $"Event hub does not contain any channel capable of handling '{eventType}'.\n" +
            $"The methods of '{receiverType}' will not be invoked:\n{Utils.JoinAsList (methods)}."
          );
        }
      }
    }

    private static (Type Type, MethodInfo [] Methods) [] GetMethodsGroup (Type handlerType, BindingFlags bindingFlags)
    {
      var methods = handlerType.GetMethods (bindingFlags);

      // Делаем карту: тип события -> список методов
      var map = new Dictionary<Type, List<MethodInfo>> (methods.Length);

      foreach (var method in methods)
      {
        var parameters = method.GetParameters ();
        if (parameters.Length != 1)
          continue;

        var eventType = parameters [0].ParameterType;
        if (!BaseEventType.IsAssignableFrom (eventType))
          continue;

        if (!map.TryGetValue (eventType, out var list))
        {
          list = new List<MethodInfo> (4);
          map.Add (eventType, list);
        }

        list.Add (method);
      }

      // Упаковываем в массив пар
      var result = new (Type Type, MethodInfo [] Methods)[map.Count];
      var idx = 0;

      foreach (var kv in map)
        result [idx++] = (kv.Key, kv.Value.ToArray ());

      return result;
    }

    public static void Unsubscribe (this EventChannelSet eventSet, object receiver)
    {
      foreach (var eventChannel in eventSet.Set)
      foreach (var eventHandler in eventChannel.Handlers.Where (handler => handler.IsTarget (receiver)))
        eventChannel.Unsubscribe (eventHandler);
    }
  }
}