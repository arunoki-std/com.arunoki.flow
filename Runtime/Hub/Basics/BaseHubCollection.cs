using Arunoki.Collections;
using Arunoki.Collections.Enumerators;

namespace Arunoki.Flow.Basics
{
  public class BaseHubCollection<TElement> : BaseHubBuilder<TElement> where TElement : class
  {
    public BaseHubCollection (IContainer<TElement> rootContainer = null) : base (rootContainer)
    {
      Elements = new(this, IsConsumable);
    }

    public Set<TElement> Elements { get; }

    public sealed override void Produce (TElement element)
    {
      base.Produce (element);

      Elements.TryAdd (element);
    }

    public override bool Clear (TElement element)
    {
      base.Clear (element);

      return Elements.Remove (element);
    }

    public override void ClearAll ()
    {
      Elements.Clear ();
    }

    public override MutableEnumerator<TElement> GetEnumerator () => Elements.GetEnumerator ();

    public override MutableCastEnumerable<TElement, T> Cast<T> () => Elements.Cast<T> ();

    /// var (index, value)
    public MutableIndexedEnumerable<TElement> WithIndex () => Elements.WithIndex ();
  }
}