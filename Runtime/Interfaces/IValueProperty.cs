namespace Arunoki.Flow
{
    public interface IValueProperty<TValue> : IEventChannel
    {
        TValue Value { get; }
        TValue Set(TValue value);
        void Reset(TValue value);
    }

    public interface IValueProperty<TValue, TEvent> : IValueProperty<TValue>, IEventChannel<TEvent>
        where TEvent : IEvent { }
}
