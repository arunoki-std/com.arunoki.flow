namespace Arunoki.Flow
{
  public interface IActiveEventReceiver
  {
    bool IsHandlingEvents { get; set; }

    void OnHandlerActivated ();

    void OnHandlerDeactivated ();
  }
}