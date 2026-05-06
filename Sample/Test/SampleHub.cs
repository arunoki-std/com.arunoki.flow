using System;

namespace Arunoki.Flow.Sample
{
  public class SampleHub : GlobalHub
  {
    public static SampleHub Get { get; private set; }

    public SampleHub ()
    {
      if (Get != null) throw new InvalidOperationException ($"'{nameof(SampleHub)}' already exists.");

      Get = this;
    }
  }
}