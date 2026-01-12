namespace Arunoki.Flow.Misc
{
  public partial class ContextSet
  {
    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      ForEach (context => Hub.Produce (context));
    }

    protected override void OnActivate ()
    {
      base.OnActivate ();

      // ForEach (context => Hub.Produce (context));
    }
  }
}