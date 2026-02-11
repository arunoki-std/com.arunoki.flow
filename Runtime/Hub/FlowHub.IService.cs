namespace Arunoki.Flow
{
  public partial class FlowHub
  {
    protected override void OnReset ()
    {
      Events.Reset ();

      base.OnReset ();
    }
  }
}