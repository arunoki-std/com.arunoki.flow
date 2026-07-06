namespace Arunoki.Flow.Sample.States
{
    public class SubstateA : SampleStateTimeout, IStateA
    {
        public SubstateA()
            : base(true, typeof(IStateA)) { }
    }
}
