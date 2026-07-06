using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Arunoki.Collections.Utilities
{
    public static partial class ReflectionUtils
    {
        private static readonly ConcurrentDictionary<PropsCacheKey, PropertyInfo[]> PropsCache =
            new();

        private static PropertyInfo[] GetCachedProperties(
            Type sourceType,
            Type lookingType,
            BindingFlags flags
        )
        {
            var key = new PropsCacheKey(sourceType, lookingType, flags);

            return PropsCache.GetOrAdd(
                key,
                static k =>
                {
                    var props = k.SourceType.GetProperties(k.Flags);

                    // 1 проход: посчитать, сколько подходит (чтобы не аллоцировать List)
                    int count = 0;
                    for (int i = 0; i < props.Length; i++)
                    {
                        var p = props[i];
                        if (IsMatchingProperty(p, k.TargetType))
                            count++;
                    }

                    if (count == 0)
                        return Array.Empty<PropertyInfo>();

                    // 2 проход: заполнить массив
                    var result = new PropertyInfo[count];
                    int w = 0;

                    for (int i = 0; i < props.Length; i++)
                    {
                        var p = props[i];
                        if (!IsMatchingProperty(p, k.TargetType))
                            continue;
                        result[w++] = p;
                    }

                    return result;
                }
            );
        }

        private static bool IsMatchingProperty(PropertyInfo property, Type lookingType)
        {
            if (property.GetIndexParameters().Length != 0)
                return false;
            if (property.GetGetMethod(true) == null)
                return false;

            var pt = property.PropertyType;
            return pt == lookingType || lookingType.IsAssignableFrom(pt);
        }

        private readonly struct PropsCacheKey : IEquatable<PropsCacheKey>
        {
            public readonly Type SourceType;
            public readonly Type TargetType;
            public readonly BindingFlags Flags;

            public PropsCacheKey(Type sourceType, Type targetType, BindingFlags flags)
            {
                SourceType = sourceType;
                TargetType = targetType;
                Flags = flags;
            }

            public override bool Equals(object obj) => obj is PropsCacheKey other && Equals(other);

            public bool Equals(PropsCacheKey other) =>
                SourceType == other.SourceType
                && TargetType == other.TargetType
                && Flags == other.Flags;

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + (SourceType != null ? SourceType.GetHashCode() : 0);
                    hash = hash * 31 + (TargetType != null ? TargetType.GetHashCode() : 0);
                    hash = hash * 31 + (int)Flags;
                    return hash;
                }
            }
        }

        private sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
            where T : class
        {
            public static readonly ReferenceEqualityComparer<T> Instance = new();

            public bool Equals(T x, T y) => ReferenceEquals(x, y);

            public int GetHashCode(T obj) =>
                System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
