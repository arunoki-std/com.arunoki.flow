using Arunoki.Collections;

using System;

namespace Arunoki.Flow.Basics
{
  public abstract partial class HubBuilder<TElement> : HubPart
    where TElement : class
  {
    protected HubBuilder (IContainer<TElement> rootContainer = null, IContainer<Type> rootKeyBuilder = null)
    {
      (this as IContainer<TElement>).RootContainer = rootContainer;
      (this as IContainer<Type>).RootContainer = rootKeyBuilder;

      Set = new(this, IsConsumable);
      KeySet = new(this, this); //TODO: add isConsumable to set collection

      composition = new(all);
    }
  }
}