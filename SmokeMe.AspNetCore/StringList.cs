using System.Collections;
using System.Collections.ObjectModel;

namespace SmokeMe.AspNetCore;

public sealed class StringList : IReadOnlyCollection<string>
{
    private readonly ReadOnlyCollection<string> _collection;
    
    private StringList(IList<string> values)
    {
        this._collection = new(values);
    }

    public IEnumerator<string> GetEnumerator()
    {
        return this._collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Count => this._collection.Count;

    public static StringList Default => new(new List<string>());

    public static bool TryParse(string value, out StringList list)
    {
        return TryParse(value, default, out list);
    }

    public static bool TryParse(string value, IFormatProvider? provider, out StringList list)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
        {
            list = Default;

            return false;
        }

        list = new(value.Split(','));

        return true;
    }

    public static implicit operator string[](StringList? list)
    {
        return list?._collection.ToArray() ?? Array.Empty<string>();
    }
}