using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace Arunoki.Flow.Basics
{
  public sealed class ActiveOperations
  {
    private const uint FirstID = 1111;

    private readonly Action<string, string, float> onUntrack;
    private readonly string category;
    private readonly string name;
    private readonly Dictionary<uint, OperationInfo> operations;

    private uint nextId = FirstID;

    public ActiveOperations (string category, Action<string, string, float> onUntrack, int capacity = 128)
    {
      this.onUntrack = onUntrack;
      this.category = category;
      this.name = $"{nameof(ActiveOperations)}({category})";
      operations = new Dictionary<uint, OperationInfo> (capacity);
    }

    public ActiveOperations (string name, int capacity = 128)
    {
      this.category = $"{nameof(ActiveOperations)}({name})";
      operations = new Dictionary<uint, OperationInfo> (capacity);
    }

    public bool IsEmpty => operations.Count == 0;
    public int Count => operations.Count;

    public uint Track (string operationName)
    {
      var id = operations.Count == 0 ? (nextId = FirstID) : ++nextId;
      operations [id] = new OperationInfo (operationName, Time.realtimeSinceStartup);
      return id;
    }

    public bool Untrack (uint id)
    {
      if (onUntrack != null)
      {
        if (operations.TryGetValue (id, out var operation))
        {
          float duration = Time.realtimeSinceStartup - operation.StartedAt;
          onUntrack (category, operation.Name, duration);
          operations.Remove (id);
          return true;
        }

        return false;
      }

      return operations.Remove (id);
    }

    public bool TryUntrack (uint id, out float duration)
    {
      if (operations.TryGetValue (id, out var operation))
      {
        duration = Time.realtimeSinceStartup - operation.StartedAt;
        onUntrack?.Invoke (category, operation.Name, duration);
        operations.Remove (id);
        return true;
      }

      duration = 0f;
      return false;
    }

    public bool TryGetDuration (uint id, out float duration)
    {
      if (operations.TryGetValue (id, out var operation))
      {
        duration = Time.realtimeSinceStartup - operation.StartedAt;
        return true;
      }

      duration = 0f;
      return false;
    }

    public bool HasLongerThan (float seconds)
    {
      float now = Time.realtimeSinceStartup;

      foreach (var pair in operations)
      {
        if (now - pair.Value.StartedAt >= seconds)
          return true;
      }

      return false;
    }

    public void Clear ()
    {
      operations.Clear ();
      nextId = 0;
    }

    public override string ToString ()
    {
      if (operations.Count == 0)
        return $"{name} []";

      float now = Time.realtimeSinceStartup;
      var builder = new StringBuilder (128);

      builder.Append (category);
      builder.Append (" [");

      bool isFirst = true;

      foreach (var pair in operations)
      {
        if (!isFirst)
          builder.Append (", ");

        isFirst = false;

        float duration = now - pair.Value.StartedAt;
        builder.Append (pair.Value.Name);
        builder.Append (" (");
        builder.Append (duration.ToString ("0.000"));
        builder.Append ("s)");
      }

      builder.Append (']');
      return builder.ToString ();
    }

    private readonly struct OperationInfo
    {
      public readonly string Name;
      public readonly float StartedAt;

      public OperationInfo (string name, float startedAt)
      {
        Name = name;
        StartedAt = startedAt;
      }
    }
  }
}