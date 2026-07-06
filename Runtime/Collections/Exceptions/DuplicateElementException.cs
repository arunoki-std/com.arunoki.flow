using System;

namespace Arunoki.Collections
{
    public class DuplicateElementException : InvalidOperationException
    {
        public DuplicateElementException(string message)
            : base(message) { }

        public DuplicateElementException(object element)
            : base($"Element '{element}' already exists in the collection.") { }

        public DuplicateElementException(object element, object collection)
            : base($"Element '{element}' already exists in the collection '{collection}'.") { }
    }
}
