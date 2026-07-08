using System;
using System.Collections.Generic;
using System.Reflection;

namespace Arunoki.Flow.Collections.Utilities
{
    // Thread-safety: PropsCache uses ConcurrentDictionary and the public methods are stateless,
    // so lookups are safe; results (List<T>) are per-call and not shared.
    public static partial class ReflectionUtils
    {
        private const BindingFlags PublicFlags = BindingFlags.Public | BindingFlags.Instance;
        private const BindingFlags ClassFlags = BindingFlags.Public | BindingFlags.NonPublic;

        public static List<Type> GetNestedTypes<T>(
            this Type sourceType,
            BindingFlags flags = ClassFlags
        )
            where T : class
        {
            var targetType = typeof(T);
            var nestedTypes = sourceType.GetNestedTypes(flags);
            var result = new List<Type>(nestedTypes.Length);

            for (int index = 0; index < nestedTypes.Length; index++)
            {
                var type = nestedTypes[index];

                if (
                    !type.IsAbstract
                    && (
                        type == targetType
                        || targetType.IsAssignableFrom(type)
                        || type.IsSubclassOf(targetType)
                    )
                )
                {
                    result.Add(type);
                }
            }

            return result;
        }

        public static List<T> FindPropertiesWithNested<T>(
            this object source,
            Func<T, bool> predicate = null,
            BindingFlags flags = PublicFlags
        )
            where T : class
        {
            var visited = new HashSet<T>(ReferenceEqualityComparer<T>.Instance);
            var result = new List<T>();
            var stack = new Stack<object>();
            stack.Push(source);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var obj in current.FindProperties(predicate, flags))
                {
                    if (visited.Add(obj))
                    {
                        result.Add(obj);
                        stack.Push(obj);
                    }
                }
            }

            return result;
        }

        public static List<T> FindProperties<T>(this object source)
            where T : class => FindProperties<T>(source, null, PublicFlags);

        public static List<T> FindProperties<T>(this object source, BindingFlags flags)
            where T : class => FindProperties<T>(source, null, flags);

        public static List<T> FindProperties<T>(this object source, Func<T, bool> predicate)
            where T : class => FindProperties(source, predicate, PublicFlags);

        public static List<T> FindProperties<T>(
            this object source,
            Func<T, bool> predicate,
            BindingFlags flags
        )
            where T : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            var lookingType = typeof(T);
            var isType = source is Type;
            var sourceType = isType ? source as Type : source.GetType();
            var sourceObject = isType ? null : source;

            if (isType)
                flags |= BindingFlags.Static; // fix issue with static singleton field

            var cachedProps = GetCachedProperties(sourceType, lookingType, flags);
            var values = new List<T>(cachedProps.Length);

            for (int i = 0; i < cachedProps.Length; i++)
            {
                var property = cachedProps[i];
                var value = (T)property.GetValue(sourceObject);

                if (value != null && (predicate == null || predicate(value)))
                    values.Add(value);
            }

            return values;
        }
    }
}
