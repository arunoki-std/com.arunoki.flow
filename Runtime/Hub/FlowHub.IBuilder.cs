using System;

namespace Arunoki.Flow
{
  public partial class FlowHub
  {
    protected override void OnReset ()
    {
      Events.Reset ();

      base.OnReset ();
    }

    public bool Register (object entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));
      var result = false;

      for (var i = 0; i < Elements.Count; i++)
      {
        var container = Elements [i];
        result = (container.IsConsumable (entity) && container.Register (entity)) || result;
      }

      return result;
    }

    public virtual void Remove (object entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));

      for (var i = 0; i < Elements.Count; i++)
      {
        var container = Elements [i];
        if (container.IsConsumable (entity)) container.Remove (entity);
      }
    }

    public virtual void RemoveAll ()
    {
      Events.ClearAll ();

      for (var i = 0; i < Elements.Count; i++)
        Elements [i].RemoveAll ();
    }

    public bool IsConsumable (object entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));

      for (var i = 0; i < Elements.Count; i++)
        if (Elements [i].IsConsumable (entity))
          return true;

      return false;
    }
  }
}