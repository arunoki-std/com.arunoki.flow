using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
  public abstract partial class BaseHubBuilder<TElement> : IBuilder
    where TElement : class
  {
    private readonly List<Type> cachedTypes = new(16);

    void IBuilder.Produce (object entity) => Produce (entity as TElement);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <exception cref="ArgumentNullException"> <see cref="element"/> is null.</exception>
    /// /// <exception cref="BuildOperationException"><see cref="CanBuildAfterHubInit"/>, <see cref="CanBuildAfterHubStarted"/>, <see cref="CanBuildAfterHubActivation"/>, <see cref="IsMultiInstancesSupported"/></exception>
    public virtual void Produce (TElement element)
    {
      if (element == null) throw new ArgumentNullException (nameof(element));

      if (!CanBuildAfterHubInit () && Hub.IsInitialized ())
        throw BuildOperationException.AfterHubInit (element);

      if (!CanBuildAfterHubStarted () && Hub.IsStarted ())
        throw BuildOperationException.AfterHubStarted (element);

      if (!CanBuildAfterHubActivation () && Hub.IsActivated ())
        throw BuildOperationException.AfterHubActivated (element);

      if (Utils.IsDebug () && !IsMultiInstancesSupported () && cachedTypes.Contains (typeof(TElement)))
        throw BuildOperationException.MultiInstancesNotSupported (element, this);
    }

    void IBuilder.Clear (object entity) => Clear (entity as TElement);

    public virtual bool Clear (TElement element)
    {
      return element == null ? throw new ArgumentNullException (nameof(element)) : false;
    }

    protected override void OnElementAdded (TElement element)
    {
      if (Utils.IsDebug () && !IsMultiInstancesSupported ()) 
        cachedTypes.Add (element.GetType ());

      base.OnElementAdded (element);
    }

    protected override void OnElementRemoved (TElement element)
    {
      base.OnElementRemoved (element);

      if (Utils.IsDebug () && !IsMultiInstancesSupported ()) 
        cachedTypes.Remove (element.GetType ());
    }

    public abstract void ClearAll ();

    bool IBuilder.IsConsumable (object entity) => IsConsumable (entity as TElement);

    public virtual bool IsConsumable (TElement element) => element != null;

    /// Check whether is element type unique at elements collection. For debug build.
    protected virtual bool IsMultiInstancesSupported () => true;

    protected virtual bool CanBuildAfterHubInit () => true;
    protected virtual bool CanBuildAfterHubStarted () => true;
    protected virtual bool CanBuildAfterHubActivation () => true;
  }
}