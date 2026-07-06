using System;
using UnityEngine;

namespace Arunoki.Flow.Sample.States
{
    public abstract class SampleStateTimeout : SampleState
    {
        protected float TimeMin = 2;
        protected float TimeMax = 4;

        private float timeElapsed;

        protected SampleStateTimeout(bool isDefault, Type parentState)
            : base(isDefault, parentState) { }

        public override void OnEnter()
        {
            base.OnEnter();

            timeElapsed = UnityEngine.Random.Range(TimeMin, TimeMax);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            timeElapsed -= Time.deltaTime;

            if (timeElapsed <= TimeMin)
                Context.Timeout.Emit();
        }
    }
}
