namespace Arunoki.Flow.StateMachine
{
  public partial class Hfsm
  {
    public bool IsActive { get; private set; }

    public void Activate ()
    {
      if (IsActive) return;

      IsActive = true;
      OnActivated ();
    }

    public void Deactivate ()
    {
      if (!IsActive) return;

      IsActive = false;
      OnDeactivated ();
    }

    protected virtual void OnActivated () { }
    protected virtual void OnDeactivated () { }
  }
}