namespace Arunoki.Flow
{
  public interface IContextPart
  {
    IContext Get ();

    void Set (IContext context);
  }
}