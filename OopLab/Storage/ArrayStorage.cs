using OopLab.Models;

namespace OopLab.Storage;

/// <summary>Сховище на звичайному масиві Product[] — розмір фіксований, тому зміна розміру = новий масив.</summary>
public sealed class ArrayStorage : ProductStorageBase
{
    private Product[] _items;

    public ArrayStorage(IEnumerable<Product> initial) => _items = initial.ToArray();

    public override string Title => "Масив Product[]";

    public override string Description =>
        $"Product[]: Length = {_items.Length}, фіксований розмір = {_items.IsFixedSize}";

    public override int Count => _items.Length;

    protected override Product GetAt(int index) => _items[index];

    protected override void SetAt(int index, Product product) => _items[index] = product;

    // Додавання: масив не можна збільшити, тому Array.Resize створює новий масив і копіює дані.
    protected override void Append(Product product)
    {
        Array.Resize(ref _items, _items.Length + 1);
        _items[^1] = product;
    }

    // Видалення: зсуваємо хвіст ліворуч і зменшуємо масив (знову нова копія).
    protected override void RemoveAt(int index)
    {
        Array.Copy(_items, index + 1, _items, index, _items.Length - index - 1);
        Array.Resize(ref _items, _items.Length - 1);
    }

    public override string Sort(IComparer<Product>? comparer)
    {
        if (comparer is null)
        {
            Array.Sort(_items);
            return "Array.Sort(items) — IComparable<Product>";
        }
        Array.Sort(_items, comparer);
        return "Array.Sort(items, comparer) — IComparer<Product>";
    }

    public override IEnumerator<Product> GetEnumerator()
    {
        foreach (Product p in _items) yield return p;
    }
}
