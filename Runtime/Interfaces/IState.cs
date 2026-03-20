using System;

namespace Arunoki.Flow
{
  public interface IState<TContext> where TContext : class
  {
    TContext Context { get; set; }

    void OnEnter ();
    void OnExit ();
    void OnUpdate ();

    bool IsDefault ();
    bool IsSubstate ();
    bool IsSubstateOf (out Type parentType);

    /// <summary>  Define whether is transition locked. </summary>
    bool IsProcessing ();
  }
}