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

    public bool IsConsumable (object element)
      => ForAnySet<IBuilder> (builder => builder.IsConsumable (element));
  }
}