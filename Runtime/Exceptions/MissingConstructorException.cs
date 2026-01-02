using System;

namespace Arunoki.Flow
{
  public class MissingConstructorException : ArgumentException
  {
    public MissingConstructorException (string typeName)
      : base ($"Required constructor of {typeName} was not found.")
    {
    }
  }
}