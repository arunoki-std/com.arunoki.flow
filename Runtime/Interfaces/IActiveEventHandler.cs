namespace Arunoki.Flow
{
  public interface IActiveEventHandler
  {
    bool IsHandlingEvents { get; set; }

    void OnHandlerActivated ();

    void OnHandlerDeactivated ();
  }
}