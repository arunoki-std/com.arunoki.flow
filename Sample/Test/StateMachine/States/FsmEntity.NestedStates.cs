using Arunoki.Flow.Sample.States;

namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity
  {
    public class StateB : BaseState, IStateB
    {
      public StateB () : base (false, null)
      {
      }
    }

    public class SubstateB : BaseStateTimeout, IStateB
    {
      public SubstateB () : base (true, typeof(IStateB))
      {
      }
    }

    public class StateC : BaseState, IStateC
    {
      public StateC () : base (false, null)
      {
      }
    }

    public class SubstateC : BaseStateTimeout, IStateC
    {
      public SubstateC () : base (true, typeof(IStateC))
      {
      }
    }
  }
}