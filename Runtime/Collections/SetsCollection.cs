using System.Collections.Generic;

namespace Arunoki.Collections
{
    // TODO [RF-008] Unused outside flow as of 2026-07-06 — candidate for removal.
    // TODO [RF-008] NOT thread-safe: Sets list mutated without synchronization; also populates
    //   itself via reflection (AddSetsFrom), which is not safe to run concurrently.
    public partial class SetsCollection<TElement> : Container<TElement>
    {
        protected readonly List<ISet<TElement>> Sets = new();

        public SetsCollection(object setsSource = null)
            : this(null, setsSource) { }

        public SetsCollection(IContainer<TElement> rootContainer, object setsSource = null)
            : base(rootContainer)
        {
            if (setsSource != null)
            {
                AddSetsFrom(setsSource);
            }
        }
    }
}
