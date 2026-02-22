using System;

namespace Arunoki.Flow
{
  public class RewriteOperationException : InvalidOperationException
  {
    public RewriteOperationException (object current, object other)
      : base ($"Trying to rewrite existing '{current}' by '{other}'.")
    {
    }

    public RewriteOperationException (string message)
      : base (message)
    {
    }
  }
}