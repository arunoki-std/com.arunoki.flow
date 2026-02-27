using System;

namespace Arunoki.Flow
{
  public class StateMachineException : InvalidOperationException
  {
    public StateMachineException (string message) : base (message) { }

    internal static StateMachineException RootIsNotDefined (object stateMachine, string message = "")
    {
      return new StateMachineException (
        $"Root state of state machine '{stateMachine.GetType ()}' is not defined. {message}");
    }

    internal static StateMachineException StateIsNotDefined (object stateMachine, Type stateType, string message = "")
    {
      return new StateMachineException (
        $"State '{stateType.Name}' is not defined at '{stateMachine.GetType ().Name}'. {message}'");
    }

    internal static StateMachineException RouterRegistrationOrder (object stateMachine, object router)
    {
      return new StateMachineException (
        $"Router '{router.GetType ()}' should be added to state machine '{stateMachine.GetType ()}' before its initialization step.");
    }
  }
}