using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Arunoki.Flow.Globals
{
  public class RuntimeManagers
  {
    public readonly List<Type> StaticTypes;

    public RuntimeManagers ()
      : this (new List<Type> (32))
    {
    }

    public RuntimeManagers (List<Type> staticTypes)
    {
      StaticTypes = staticTypes;
    }

    public RuntimeManagers (Assembly assembly)
      : this (new List<Type> (32))
    {
      Init (assembly);
    }

    public RuntimeManagers (Assembly assembly, string nameSpace)
      : this (new List<Type> (32))
    {
      Init (assembly, new List<string> { nameSpace });
    }

    public RuntimeManagers (Assembly assembly, List<string> namespaces)
      : this (new List<Type> (32))
    {
      Init (assembly, namespaces);
    }

    public void Init (Assembly assembly, List<string> namespaces = null)
    {
      foreach (var type in assembly.GetTypes ())
      {
        if (!type.IsStatic ())
          continue;

        if (namespaces != null && namespaces.Count > 0 && !namespaces.Contains (type.Namespace))
          continue;

        StaticTypes.Add (type);
        RuntimeHelpers.RunClassConstructor (type.TypeHandle);
      }
    }

    public Enumerator GetEnumerator () => new(StaticTypes);

    public struct Enumerator
    {
      private List<Type> list;
      private int index;

      public Enumerator (List<Type> list)
      {
        this.list = list;
        index = -1;
      }

      public Type Current => list [index];
      public bool MoveNext () => ++index < list.Count;
      public void Reset () => index = -1;
      public void Dispose () => list = null;
    }
  }
}