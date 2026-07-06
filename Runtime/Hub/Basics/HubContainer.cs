namespace Arunoki.Flow.Basics
{
    public abstract partial class HubContainer<TElement> : HubPart
        where TElement : class
    {
        protected HubContainer()
        {
            Set = new(new Container(this), IsConsumable);
            KeySet = new(new Container(this), new KeyContainer(this), IsConsumable);
        }
    }
}
