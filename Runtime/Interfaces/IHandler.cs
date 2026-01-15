namespace Arunoki.Flow
{
  public interface IHandler
  {
  }

  public interface IActiveHandler : IHandler
  {
    bool IsHandlingEvents { get; set; }

    void OnHandlerActivated ();

    void OnHandlerDeactivated ();
  }
}