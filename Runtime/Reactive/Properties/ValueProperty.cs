using System;
using System.Collections.Generic;
using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
    public class ValueProperty<TEvent, TValue>
        : Channel<TEvent>,
            IValueProperty<TValue, TEvent>,
            IObservableEventChannel<TValue>,
            IResettable
        where TEvent : struct, IValueEvent<TValue>
    {
        private readonly TValue defaultValue;
        private readonly bool autoReset;

        private Action<Channel, TValue> onUpdatedCallback;

        event Action<Channel, TValue> IObservableEventChannel<TValue>.OnUpdated
        {
            add => onUpdatedCallback += value;
            remove => onUpdatedCallback -= value;
        }

        public ValueProperty(bool autoReset)
            : this(default, autoReset) { }

        public ValueProperty()
            : this(default, false) { }

        public ValueProperty(TValue defaultValue, bool autoReset = false)
        {
            this.defaultValue = defaultValue;
            this.autoReset = autoReset;

            OnEvent += (ref TEvent evt) =>
            {
                onUpdatedCallback?.Invoke(this, evt.Current);
            };
        }

        public TValue Value { get; private set; }

        public TValue Previous { get; private set; }

        public bool AutoReset() => autoReset;

        protected virtual bool TryChange(ref TValue value)
        {
            var current = Value;
            if (!Equals(ref value, ref current))
            {
                Previous = current;
                Value = value;

                return true;
            }

            return false;
        }

        public virtual TValue Set(TValue value)
        {
            if (TryChange(ref value))
                Publish();
            return value;
        }

        /// Update values if needed and publish event anyway
        public virtual TValue Force(TValue value)
        {
            TryChange(ref value);
            Publish();
            return value;
        }

        void IValueProperty<TValue>.Reset(TValue value) => TryChange(ref value);

        /// Set values to default.
        public virtual void Reset()
        {
            Value = defaultValue;
            Previous = defaultValue;
        }

        /// Remove all subscribers.
        public override void Clear()
        {
            base.Clear();

            Reset();

            onUpdatedCallback = null;
        }

        protected virtual bool Equals(ref TValue a, ref TValue b) =>
            EqualityComparer<TValue>.Default.Equals(a, b);

        protected override TEvent GetEventInstance()
        {
            return new TEvent
            {
                Context = this.Context,
                Current = this.Value,
                Previous = this.Previous,
            };
        }
    }
}
