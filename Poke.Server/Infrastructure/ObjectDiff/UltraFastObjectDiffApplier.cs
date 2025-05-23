using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Poke.Server.Infrastructure.ObjectDiff;

public static class UltraFastObjectDiffApplier
{
    private const int MaxPathDepth = 32;

    private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyCache = [];
    private static readonly Dictionary<Type, Func<object>> _constructorCache = [];

    public static void ApplyChanges(object target, Dictionary<string, object?> changes)
    {
        foreach (var kvp in changes)
        {
            ApplyChange(target, kvp.Key, kvp.Value);
        }
    }

    private static void ApplyChange(object target, string path, object? value)
    {
        Span<PathSegment> segments = stackalloc PathSegment[MaxPathDepth];
        ReadOnlySpan<char> pathSpan = path.AsSpan();
        int count = ParsePath(pathSpan, segments);

        object? current = target;

        for (int i = 0; i < count; i++)
        {
            if (current == null) return;

            var seg = segments[i];
            bool isLast = i == count - 1;

            if (seg.IsIndex)
            {
                if (current is IList list)
                {
                    if (seg.Index >= list.Count) return;

                    if (isLast)
                    {
                        var elem = list[seg.Index];
                        list[seg.Index] = ConvertValue(value, elem?.GetType());
                    }
                    else
                    {
                        current = list[seg.Index];
                    }
                }
                else return;
            }
            else
            {
                string propName = seg.GetSpan(pathSpan).ToString();
                var prop = GetProperty(current.GetType(), propName);
                if (prop == null || !prop.CanRead || !prop.CanWrite) return;

                if (isLast)
                {
                    prop.SetValue(current, ConvertValue(value, prop.PropertyType));
                }
                else
                {
                    var next = prop.GetValue(current);
                    if (next == null)
                    {
                        next = CreateInstance(prop.PropertyType);
                        prop.SetValue(current, next);
                    }
                    current = next;
                }
            }
        }
    }

    private static int ParsePath(ReadOnlySpan<char> path, Span<PathSegment> segments)
    {
        int count = 0;
        int i = 0;

        while (i < path.Length && count < segments.Length)
        {
            if (char.IsLetter(path[i]) || path[i] == '_')
            {
                int start = i;
                while (i < path.Length && (char.IsLetterOrDigit(path[i]) || path[i] == '_'))
                    i++;
                segments[count++] = new PathSegment(start, i - start);
            }
            else if (path[i] == '[')
            {
                i++; // skip '['
                int start = i;
                while (i < path.Length && char.IsDigit(path[i])) i++;
                int index = int.Parse(path.Slice(start, i - start), CultureInfo.InvariantCulture);
                if (i < path.Length && path[i] == ']') i++; // skip ']'
                segments[count++] = new PathSegment(index);
            }
            else
            {
                i++; // skip '.', etc.
            }
        }

        return count;
    }

    private static object? ConvertValue(object? value, Type? targetType)
    {
        if (value == null || targetType == null || targetType.IsAssignableFrom(value.GetType()))
            return value;

        if (targetType.IsEnum)
            return Enum.Parse(targetType, value.ToString()!);

        return Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType) ?? targetType, CultureInfo.InvariantCulture);
    }

    private static PropertyInfo? GetProperty(Type type, string name)
    {
        if (!_propertyCache.TryGetValue(type, out var props))
        {
            props = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                props[prop.Name] = prop;
            }
            _propertyCache[type] = props;
        }

        return props.TryGetValue(name, out var info) ? info : null;
    }

    private static object CreateInstance(Type type)
    {
        if (_constructorCache.TryGetValue(type, out var ctor))
            return ctor();

        var constructorInfo = type.GetConstructor(Type.EmptyTypes);
        if (constructorInfo == null)
            throw new InvalidOperationException($"Type {type.FullName} does not have a parameterless constructor.");

        var lambda = Expression.Lambda<Func<object>>(Expression.New(constructorInfo)).Compile();
        _constructorCache[type] = lambda;
        return lambda();
    }

    private readonly struct PathSegment
    {
        public int Start { get; }
        public int Length { get; }
        public int Index { get; }
        public bool IsIndex => Index >= 0;

        public PathSegment(int start, int length)
        {
            Start = start;
            Length = length;
            Index = -1;
        }

        public PathSegment(int index)
        {
            Start = -1;
            Length = 0;
            Index = index;
        }

        public ReadOnlySpan<char> GetSpan(ReadOnlySpan<char> fullPath)
            => IsIndex ? default : fullPath.Slice(Start, Length);
    }
}
