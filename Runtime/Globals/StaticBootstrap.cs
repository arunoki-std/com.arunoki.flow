using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Arunoki.Flow.Globals
{
  public class StaticBootstrap
  {
    private readonly List<Type> staticTypes;

    public StaticBootstrap ()
      : this (new List<Type> (32))
    {
    }

    public StaticBootstrap (List<Type> staticTypes)
    {
      this.staticTypes = staticTypes;
    }

    public StaticBootstrap (Assembly assembly)
      : this (new List<Type> (32))
    {
      Init (assembly);
    }

    public StaticBootstrap (Assembly assembly, string nameSpace)
      : this (new List<Type> (32))
    {
      Init (assembly, new List<string> { nameSpace });
    }

    public StaticBootstrap (Assembly assembly, List<string> namespaces)
      : this (new List<Type> (32))
    {
      Init (assembly, namespaces);
    }

    /// Find all static classes at declared <see cref="namespaces"/> and run their class constructors.
    public void Init (Assembly assembly, List<string> namespaces = null)
    {
      foreach (var type in assembly.GetTypes ())
      {
        if (!type.IsStatic ())
          continue;

        if (namespaces != null && namespaces.Count > 0 && !namespaces.Contains (type.Namespace))
          continue;

        staticTypes.Add (type);

        RuntimeHelpers.RunClassConstructor (type.TypeHandle);
      }
    }

    public IReadOnlyList<Type> GetTypes () => staticTypes;

    public Enumerator GetEnumerator () => new(staticTypes);

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