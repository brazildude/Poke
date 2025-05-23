using System.Collections;
using System.Reflection;
using System.Text;

namespace Poke.Server.Infrastructure.ObjectDiff;

public static class UltraFastObjectDiff
{
    private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = [];

    public static Dictionary<string, object?> GetChanges(object original, object modified, HashSet<string>? ignoreProperties = null)
    {
        var changes = new Dictionary<string, object?>();
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        var sb = new StringBuilder(128);
        Compare(original, modified, sb, changes, visited, ignoreProperties ?? []);
        return changes;
    }

    private static void Compare(object? orig, object? mod, StringBuilder path, Dictionary<string, object?> changes, HashSet<object> visited, HashSet<string> ignoreProperties)
    {
        if (orig == null && mod == null) return;

        // Cycle detection for original object
        if (orig != null && !IsSimpleType(orig.GetType()))
        {
            if (!visited.Add(orig)) return; // Already visited, skip
        }
        // Cycle detection for modified object
        if (mod != null && !IsSimpleType(mod.GetType()))
        {
            if (!visited.Add(mod)) return;
        }

        if (orig == null || mod == null || orig.GetType() != mod.GetType())
        {
            changes[ToCamelCase(path)] = mod;
            return;
        }

        var type = orig.GetType();

        if (IsSimpleType(type))
        {
            if (!Equals(orig, mod))
                changes[ToCamelCase(path)] = mod;
            return;
        }

        if (mod is IEnumerable modEnum && orig is IEnumerable origEnum && type != typeof(string))
        {
            CompareEnumerables(origEnum, modEnum, path, changes, visited, ignoreProperties);
            return;
        }

        foreach (var prop in GetProperties(type))
        {
            if (ignoreProperties.Contains(prop.Name))
                continue;

            var origVal = prop.GetValue(orig);
            var modVal = prop.GetValue(mod);

            int len = path.Length;
            if (len > 0) path.Append('.');
            path.Append(prop.Name);

            Compare(origVal, modVal, path, changes, visited, ignoreProperties);
            path.Length = len; // Reset path
        }
    }

    private static void CompareEnumerables(IEnumerable origEnum, IEnumerable modEnum, StringBuilder path, Dictionary<string, object?> changes, HashSet<object> visited, HashSet<string> ignoreProperties)
    {
        var origIter = origEnum.GetEnumerator();
        var modIter = modEnum.GetEnumerator();

        int i = 0;
        bool oHas, mHas;

        while ((oHas = origIter.MoveNext()) | (mHas = modIter.MoveNext()))
        {
            var oItem = oHas ? origIter.Current : null;
            var mItem = mHas ? modIter.Current : null;

            int len = path.Length;
            path.Append('[').Append(i).Append(']');
            Compare(oItem, mItem, path, changes, visited, ignoreProperties);
            path.Length = len;

            i++;
        }
    }

    private static PropertyInfo[] GetProperties(Type type)
    {
        if (!PropertyCache.TryGetValue(type, out var props))
        {
            props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyCache[type] = props;
        }
        return props;
    }

    private static bool IsSimpleType(Type type) =>
        type.IsPrimitive || type.IsEnum ||
        type == typeof(string) || type == typeof(decimal) ||
        type == typeof(DateTime) || type == typeof(Guid);

    private static string ToCamelCase(StringBuilder path)
    {
        string raw = path.ToString();
        Span<char> buffer = stackalloc char[raw.Length];
        raw.AsSpan().CopyTo(buffer);

        bool inIndexer = false;
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] == '[')
            {
                inIndexer = true;
                continue;
            }
            else if (buffer[i] == ']')
            {
                inIndexer = false;
                continue;
            }

            if (!inIndexer && (i == 0 || buffer[i - 1] == '.'))
            {
                buffer[i] = char.ToLowerInvariant(buffer[i]);
            }
        }

        return buffer.ToString();
    }
}

public sealed class ReferenceEqualityComparer : IEqualityComparer<object>
{
    public static ReferenceEqualityComparer Instance { get; } = new();

    private ReferenceEqualityComparer() { }

    public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

    public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
}
