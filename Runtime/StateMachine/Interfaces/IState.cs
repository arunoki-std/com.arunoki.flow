using System;

namespace Arunoki.Flow
{
  public interface IState<TEntity> where TEntity : class
  {
    TEntity Entity { get; set; }

    void OnEnter ();
    void OnExit ();
    void OnUpdate ();

    bool IsDefault ();
    bool IsSubState ();
    Type GetParentType ();
  }
}