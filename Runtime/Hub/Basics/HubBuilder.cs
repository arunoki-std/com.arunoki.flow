namespace Arunoki.Flow.Basics
{
  public abstract partial class HubBuilder<TElement> : BaseHubBuilder
    where TElement : class
  {
    protected HubBuilder ()
    {
      Set = new(new Container (this), IsConsumable);
      KeySet = new(new Container (this), new KeyContainer (this), IsConsumable);
    }
  }
}