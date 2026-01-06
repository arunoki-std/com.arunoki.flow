namespace Arunoki.Flow
{
  public partial class FlowHub : IBuilder
  {
    public void Produce (object element)
    {
      ForEachSet<IBuilder> (builder =>
      {
        if (builder.IsConsumable (element)) builder.Produce (element);
      });
    }

    /// Rebuild all contexts.
    public void Build ()
    {
      AllContexts.ForEach (Produce);
    }

    public bool IsConsumable (object element)
    {
      return ForAnySet<IBuilder> (builder => builder.IsConsumable (element));
    }
  }
}