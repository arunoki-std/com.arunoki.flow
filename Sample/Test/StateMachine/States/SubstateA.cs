namespace Arunoki.Flow.Sample.States
{
  public class SubstateA : BaseStateTimeout, IStateA
  {
    public SubstateA () : base (true, typeof(IStateA))
    {
    }
  }
}