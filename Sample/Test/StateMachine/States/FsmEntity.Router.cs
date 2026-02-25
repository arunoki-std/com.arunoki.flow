using Arunoki.Flow.Sample.States;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity
  {
    [Preserve]
    public class Router : IContextPart, IHandler, IInitializable
    {
      private bool isInitialized;
      public FsmEntity Entity { get; private set; }
      public StateMachine<FsmEntity> Machine => Entity.StateMachine;

      private void OnInitialize ()
      {
        UnityEngine.Debug.LogWarning ("Router initialized"); //TODO: Remove log
        Machine.AddState<StateA> ();
        Machine.AddState<SubstateA> ();
        Machine.AddState<SubstateA1> ();
        Machine.SetRoot<StateA> ();
      }

      public void OnTimeout (ref TimeoutEvent evt)
      {
        UnityEngine.Debug.LogWarning ("TIME OUT"); //TODO: Remove log

        if (Machine.IsActive<IStateA> ())
        {
          if (Machine.IsActive<SubstateA> ())
            Machine.Change<SubstateA1> ();

          else Machine.Change<IStateB> ();
        }

        else if (Machine.IsActive<IStateB> ())
          Machine.Change<IStateC> ();

        else if (Machine.IsActive<IStateC> ())
        {
          Machine.Change<IStateA> ();
        }
      }

      void IInitializable.Initialize ()
      {
        if (!isInitialized)
        {
          OnInitialize ();
          isInitialized = true;
        }
      }

      IContext IContextPart.Get () => Entity;
      void IContextPart.Set (IContext context) => Entity = (FsmEntity) context;


      bool IInitializable.IsInitialized () => isInitialized;
    }
  }
}