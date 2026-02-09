using Arunoki.Collections;

namespace Arunoki.Flow.Basics
{
  public abstract partial class BaseHubBuilder<TElement> : BaseHubPart<TElement>
    where TElement : class
  {
    protected BaseHubBuilder (IContainer<TElement> rootContainer = null) : base (rootContainer)
    {
    }
  }
}