using Arunoki.Flow.Events;
using Arunoki.Flow.Events.Core;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Arunoki.Flow.Utilities
{
  internal static partial class EventBusUtility
  {
    public static List<Callback> Subscribe (EventBus events, object receiver)
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
      var callbacks = new List<Callback> (groups.Length * 8);

      for (int i = 0; i < groups.Length; i++)
      {
        (Type eventType, MethodInfo [] methods) = groups [i];

        if (events.TryGet (eventType, out var channel))
        {
          callbacks.Add (channel.Subscribe (receiver, methods));
        }
        else if (Utils.IsWarningsEnabled ())
        {
          UnityEngine.Debug.LogWarning (
            $"Event hub does not contain any channel capable of handling '{eventType}'.\n" +
            $"The methods of '{receiverType}' will not be invoked:\n{Utils.JoinAsList (methods)}."
          );
        }
      }

      return callbacks;
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
        if (!eventType.IsByRef)
          continue;

        eventType = eventType.GetElementType ();
        if (!typeof(IEvent).IsAssignableFrom (eventType))
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
  }
}