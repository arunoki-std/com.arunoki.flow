using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Misc
{
  public abstract class ContextPart : IContextPart
  {
    public IContext Context { get; private set; }

    IContext IContextPart.Get () => Context;

    void IContextPart.Set (IContext context)
    {
      if (Utils.IsDebug ())
      {
        if (Context != null && context != null)
          throw new InvalidOperationException (
            $"Trying to rewrite existing {nameof(Context)} '{Context}' by '{context}'.");
      }

      Context = context;
    }
  }
}