namespace Arunoki.Flow
{
  public interface IReadable<out TValue>
  {
    TValue Value { get; }

    TValue Previous { get; }
  }
}