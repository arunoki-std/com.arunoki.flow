namespace Arunoki.Flow
{
  public interface IContextPart
  {
    IFlowContext Get ();

    void Set (IFlowContext context);
  }
}