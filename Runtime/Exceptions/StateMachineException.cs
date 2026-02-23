using System;

namespace Arunoki.Flow
{
  public class StateMachineException : InvalidOperationException
  {
    public StateMachineException (string message) : base (message) { }

    internal static StateMachineException RootIsNotDefined (object stateMachine)
    {
      return new StateMachineException (
        $"State machine '{stateMachine.GetType ()}' is not ready to use. Root state must be defined.");
    }

    internal static StateMachineException StateIsNotDefined (object stateMachine, Type stateType)
    {
      return new StateMachineException (
        $"State '{stateType.Name}' is not defined at '{stateMachine.GetType ().Name}'.'");
    }

    internal static StateMachineException RouterRegistrationOrder (object stateMachine, object router)
    {
      return new StateMachineException (
        $"Router '{router.GetType ()}' should be added to state machine '{stateMachine.GetType ()}' before its initialization step.");
    }
  }
}