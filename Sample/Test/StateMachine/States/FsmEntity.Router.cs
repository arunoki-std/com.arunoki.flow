using Arunoki.Flow.Sample.States;

namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity
  {
    public class Router : StateRouter<FsmEntity>, IStateInitializer<FsmEntity>
    {
      public void OnInit (IStateBuilder<FsmEntity> builder)
      {
        builder.AddState<StateA> ();
        builder.AddState<SubstateA> ();
        builder.AddState<SubstateA1> ();
        builder.InitRoot<StateA> ();
      }

      protected override void OnInitialize ()
      {
        base.OnInitialize ();

        UnityEngine.Debug.LogWarning ("Router initialized"); //TODO: Remove log
        // Machine.Change<SubstateA1> ();
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
    }
  }
}