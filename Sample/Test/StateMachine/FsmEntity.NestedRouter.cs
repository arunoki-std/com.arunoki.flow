using Arunoki.Flow.Sample.Managers;
using Arunoki.Flow.Sample.States;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity
  {
    [Preserve]
    public class Router : IContextPart, IFlowHandler, IInitializable
    {
      private bool isInitialized;
      public FsmEntity Entity { get; private set; }
      public StateMachine<FsmEntity> States => Entity.States;

      private void OnInitialize ()
      {
        States.AddState<StateA> ();
        States.AddState<SubstateA> ();
        States.AddState<SubstateA1> ();

        States.GoTo<StateB> ();
      }

      public void OnTimeout (ref TimeoutEvent evt)
      {
        SampleManager.Log<Router> (evt);

        if (States.IsActive<IStateA> ())
        {
          if (States.IsActive<SubstateA> ())
            States.GoTo<SubstateA1> ();

          else States.GoTo<IStateB> ();
        }

        else if (States.IsActive<IStateB> ())
          States.GoTo<IStateC> ();

        else if (States.IsActive<IStateC> ())
          States.GoTo<IStateA> ();
      }

      void IInitializable.Initialize ()
      {
        if (!isInitialized)
        {
          OnInitialize ();
          isInitialized = true;
        }
      }

      IFlowContext IContextPart.Get () => Entity;
      void IContextPart.Set (IFlowContext context) => Entity = (FsmEntity) context;


      bool IInitializable.IsInitialized () => isInitialized;
    }
  }
}