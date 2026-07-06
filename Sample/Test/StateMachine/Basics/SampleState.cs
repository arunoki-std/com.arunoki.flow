using System;

namespace Arunoki.Flow.Sample.States
{
    public class SampleState : State<FsmEntity>
    {
        protected SampleState(bool isDefault, Type parentState)
            : base(isDefault, parentState) { }

        public override void OnEnter()
        {
            base.OnEnter();

            UnityEngine.Debug.Log($"state entered:\t {GetType().Name}");
        }

        public override void OnExit()
        {
            base.OnExit();

            UnityEngine.Debug.Log($"state exit:\t {GetType().Name}");
        }
    }
}
