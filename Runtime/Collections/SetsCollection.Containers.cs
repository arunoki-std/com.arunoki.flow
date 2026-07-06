using Arunoki.Collections.Utilities;

namespace Arunoki.Collections
{
    public partial class SetsCollection<TElement>
    {
        protected void AddSetsFrom(object source)
        {
            source.FindProperties<ISet<TElement>>(TryAddSet);
        }

        protected virtual bool TryAddSet(ISet<TElement> set)
        {
            if (set is null)
                return false;
            if (set == this)
                return false;
            if (Sets.Contains(set))
                return false;

            OnSetAdded(set);
            return true;
        }

        protected virtual void OnSetAdded(ISet<TElement> set)
        {
            Sets.Add(set);
        }
    }
}
