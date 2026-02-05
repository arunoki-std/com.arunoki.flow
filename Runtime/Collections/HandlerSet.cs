using Arunoki.Collections;

namespace Arunoki.Flow.Collections
{
  public class HandlerSet : BaseHandlerSet
  {
    protected Set<IHandler> Handlers = new();

    protected override ISet<IHandler> GetSet () => Handlers;

    protected sealed override void Produce (object element)
      => Produce ((IHandler) element);

    public virtual void Produce (IHandler handler)
      => Handlers.Add (handler);

    public override bool IsConsumable (object element)
      => element is IHandler;
  }
}