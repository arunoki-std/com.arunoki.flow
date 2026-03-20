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
    bool IsReadyGoNext ();
  }
}