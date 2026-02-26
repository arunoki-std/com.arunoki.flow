using Arunoki.Flow.Sample.States;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity
  {
    [Preserve]
    public class StateB : BaseState, IStateB
    {
      public StateB () : base (false, null)
      {
      }
    }

    [Preserve]
    public class SubstateB : BaseStateTimeout, IStateB
    {
      public SubstateB () : base (true, typeof(IStateB))
      {
      }
    }

    [Preserve]
    public class StateC : BaseState, IStateC
    {
      public StateC () : base (false, null)
      {
      }
    }

    [Preserve]
    public class SubstateC : BaseStateTimeout, IStateC
    {
      public SubstateC () : base (true, typeof(IStateC))
      {
      }
    }
  }
}