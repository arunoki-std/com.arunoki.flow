using Arunoki.Collections;

using System;

namespace Arunoki.Flow.Basics
{
  public abstract partial class HubBuilder<TElement> : HubPart
    where TElement : class
  {
    protected HubBuilder ()
    {
      Set = new(new Container (this), IsConsumable);
      KeySet = new(new Container (this), new KeyContainer (this)); //TODO: add isConsumable to set collection

      composition = new(all);
    }
  }
}