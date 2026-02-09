using System;

namespace Arunoki.Flow
{
  public class BuildOperationException : InvalidOperationException
  {
    public BuildOperationException (string message) : base (message) { }

    public static BuildOperationException AfterHubInit (object entity)
    {
      return new BuildOperationException (
        $"This type of entity '{entity.GetType ()}' must be produced before {nameof(FlowHub)} has been initialized.");
    }

    public static BuildOperationException AfterHubStarted (object entity)
    {
      return new BuildOperationException (
        $"This type of entity '{entity.GetType ()}' must be produced before {nameof(FlowHub)} has been started.");
    }

    public static BuildOperationException AfterHubActivated (object entity)
    {
      return new BuildOperationException (
        $"This type of entity '{entity.GetType ()}' must be produced before {nameof(FlowHub)} has been activated.");
    }

    public static BuildOperationException MultiInstancesNotSupported (object entity, IBuilder builder)
    {
      return new BuildOperationException (
        $"Builder '{builder.GetType ()}' can produce only one instance per type '{entity.GetType ()}'.");
    }
  }
}