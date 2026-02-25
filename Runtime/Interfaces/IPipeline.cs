namespace Arunoki.Flow
{
  /// This node tells <see cref="FlowHub"/> that its nested classes contains <see cref="IHandler"/> implementation. 
  public interface IPipeline
  {
  }

  public interface IActivePipeline : IPipeline
  {
    void OnActivated ();
    void OnDeactivated ();
  }

  public interface ILatePipeline : IPipeline
  {
    void OnLateActivate ();
  }

  public interface IResettablePipeline : IPipeline
  {
    void OnReset ();
  }
}