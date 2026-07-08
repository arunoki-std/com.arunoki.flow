using System;
using System.Collections.Generic;
using System.Reflection;
using Arunoki.Flow.Collections.Enumerators;
using Arunoki.Flow.Events.Core;
using Arunoki.Flow.Utilities;

namespace Arunoki.Flow.Events
{
    public abstract class Channel : IEventChannel, IContextPart
    {
        private readonly Type eventType;

        protected internal List<Callback> Callbacks { get; } = new(16);

        public IFlowContext Context { get; private set; }

        protected Channel(Type eventType)
        {
            this.eventType = eventType;
        }

        protected internal virtual void Add(Callback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            Callbacks.Insert(0, callback);
        }

        protected internal virtual void Remove(Callback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            RemoveAt(Callbacks.IndexOf(callback));
        }

        protected internal virtual void RemoveAt(int index)
        {
            if (index < 0 || index >= Callbacks.Count)
                return;

            Callbacks.RemoveAt(index);
        }

        public virtual void Clear()
        {
            for (var i = Callbacks.Count - 1; i >= 0; i--)
                Callbacks.RemoveAt(i);
        }

        protected internal abstract Callback Subscribe(object target, MethodInfo[] methods);

        public Type GetEventType() => eventType;

        IFlowContext IContextPart.Get() => Context;

        void IContextPart.Set(IFlowContext context)
        {
            if (Utils.IsDebug())
            {
                if (Context != null && context != null)
                    throw new InvalidOperationException(
                        $"Trying to rewrite existing {nameof(Context)} '{Context}' by other '{context}'."
                    );
            }

            Context = context;
        }

        public MutableEnumerator<Callback> GetEnumerator() => new(Callbacks);
    }
}
