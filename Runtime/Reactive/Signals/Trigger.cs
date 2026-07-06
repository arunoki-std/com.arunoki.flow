using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
    public class Trigger<TEvent> : Channel<TEvent>, IResettable
        where TEvent : struct, IDomainEvent
    {
        private readonly bool autoReset;

        public Trigger(bool autoReset = false)
        {
            this.autoReset = autoReset;
        }

        public bool IsTriggered { get; private set; }

        public void Fire()
        {
            if (!IsTriggered)
            {
                IsTriggered = true;

                Publish();
            }
        }

        void IResettable.Reset() => Reload();

        public void Reload()
        {
            IsTriggered = false;
        }

        public bool AutoReset() => autoReset;

        public static implicit operator bool(Trigger<TEvent> a) => a.IsTriggered;
    }
}
