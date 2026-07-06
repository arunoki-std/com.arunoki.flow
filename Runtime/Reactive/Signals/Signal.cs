using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
    public class Signal<TEvent> : Channel<TEvent>
        where TEvent : struct, IDomainEvent
    {
        /// Call event.
        /// Methods from <see cref="IFlowHandler"/>'s will be invoked first and after them event delegates.
        public void Emit()
        {
            Publish();
        }
    }
}
