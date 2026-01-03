namespace Arunoki.Flow.Sample.Events
{
  public struct OverloadEvent : IDomainEvent
  {
    public OverloadEvent (IContext context)
    {
      Context = context;
    }

    public IContext Context { get; }

    public bool GetMessage (out string message)
    {
      message = null;
      return false;
    }
  }
}