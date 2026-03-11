using Arunoki.Flow.Sample.States;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity
  {
    [Preserve]
    public class StateB : SampleState, IStateB
    {
      public StateB () : base (false, null)
      {
      }
    }

    [Preserve]
    public class SubstateB : SampleStateTimeout, IStateB
    {
      public SubstateB () : base (true, typeof(IStateB))
      {
      }
    }

    [Preserve]
    public class StateC : SampleState, IStateC
    {
      public StateC () : base (false, null)
      {
      }
    }

    [Preserve]
    public class SubstateC : SampleStateTimeout, IStateC
    {
      public SubstateC () : base (true, typeof(IStateC))
      {
      }
    }
  }
}