namespace Arunoki.Flow
{
  public interface IProperty<TValue> : IReadable<TValue>
  {
    bool Set (TValue value);
  }
}