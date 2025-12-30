namespace Arunoki.Collections
{
  public interface IGroupHandler<TElement>
  {
    IGroupHandler<TElement> TargetGroupHandler { get; set; }

    void OnAdded (TElement element);
    void OnRemoved (TElement element);
  }
}