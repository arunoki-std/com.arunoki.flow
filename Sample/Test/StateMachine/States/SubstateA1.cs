namespace Arunoki.Flow.Sample.States
{
  public class SubstateA1 : SampleStateTimeout, IStateA
  {
    public SubstateA1 () : base (false, typeof(IStateA))
    {
    }
  }
}