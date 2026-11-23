using System.Collections;

namespace OopLab.Comparison;

/// <summary>
/// Універсальний IComparer&lt;T&gt;, що порівнює об'єкти за ключем, який повертає селектор.
/// Дозволяє не писати окремий клас-компаратор для кожного поля.
/// </summary>
public sealed class KeyComparer<T, TKey> : IComparer<T>
{
    private readonly Func<T, TKey> _selector;
    private readonly IComparer<TKey> _keyComparer;

    public KeyComparer(Func<T, TKey> selector, IComparer<TKey>? keyComparer = null)
    {
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        _keyComparer = keyComparer ?? Comparer<TKey>.Default;
    }

    public int Compare(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        return _keyComparer.Compare(_selector(x), _selector(y));
    }
}

/// <summary>
/// Адаптер: перетворює IComparer&lt;T&gt; на неузагальнений IComparer
/// (потрібен для ArrayList.Sort, який приймає лише System.Collections.IComparer).
/// </summary>
public sealed class NonGenericComparer<T> : IComparer
{
    private readonly IComparer<T> _inner;

    public NonGenericComparer(IComparer<T> inner) => _inner = inner;

    public int Compare(object? x, object? y) => _inner.Compare((T?)x, (T?)y);
}
