using System;

namespace Arunoki.Flow
{
  public class RewriteOperationException : InvalidOperationException
  {
    public RewriteOperationException (object current, object other)
      : base ($"Trying to rewrite existing '{current}' by '{other}'.")
    {
    }

    public RewriteOperationException (string propertyName)
      : base ($"Trying to rewrite existing '{propertyName}' value.")
    {
    }
  }
}