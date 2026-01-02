using System;

namespace Arunoki.Flow
{
  public class StaticManagerException : ArgumentException
  {
    public StaticManagerException (Type wrongType)
      : base ($"Type {wrongType.Name} is not static manager.")
    {
    }
  }
}