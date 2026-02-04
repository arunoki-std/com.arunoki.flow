using Arunoki.Flow.Globals;

using System;

namespace Arunoki.Flow.Sample
{
  public class SampleHub : GlobalHub
  {
    private static SampleHub _instance;

    public static SampleHub Get => _instance;

    public SampleHub ()
    {
      if (_instance != null) throw new InvalidOperationException ($"'{nameof(SampleHub)}' already exists.");

      _instance = this;
    }
  }
}