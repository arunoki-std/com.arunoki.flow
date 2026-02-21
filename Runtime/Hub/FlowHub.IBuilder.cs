using System;

namespace Arunoki.Flow
{
  public partial class FlowHub : IHubBuilder
  {
    protected override void OnReset ()
    {
      Events.Reset ();

      base.OnReset ();
    }

    public bool Build (object entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));
      var result = false;

      for (var i = 0; i < Elements.Count; i++)
      {
        var builder = Elements [i];
        result = (builder.IsConsumable (entity) && builder.Build (entity)) || result;
      }

      return result;
    }

    public virtual void Clear (object entity)
    {
      if (entity == null) throw new ArgumentNullException (nameof(entity));

      for (var i = 0; i < Elements.Count; i++)
      {
        var builder = Elements [i];
        if (builder.IsConsumable (entity)) builder.Clear (entity);
      }
    }

    public virtual void ClearAll ()
    {
      Events.ClearAll ();

      for (var i = 0; i < Elements.Count; i++)
        Elements [i].ClearAll ();
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