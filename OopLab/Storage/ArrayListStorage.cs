using System.Collections;
using OopLab.Comparison;
using OopLab.Models;

namespace OopLab.Storage;

/// <summary>Сховище на неузагальненій колекції ArrayList — зберігає object, тому потрібні приведення типів.</summary>
public sealed class ArrayListStorage : ProductStorageBase
{
    private readonly ArrayList _list;

    public ArrayListStorage(IEnumerable<Product> initial)
    {
        _list = new ArrayList();
        foreach (Product p in initial) _list.Add(p);
    }

    public override string Title => "Неузагальнена колекція ArrayList";

    public override string Description =>
        $"ArrayList: Count = {_list.Count}, Capacity = {_list.Capacity}, елементи типу object";

    public override int Count => _list.Count;

    // Явне приведення (Product): компілятор не гарантує тип, помилка виникне лише під час виконання.
    protected override Product GetAt(int index) => (Product)_list[index]!;

    protected override void SetAt(int index, Product product) => _list[index] = product;

    // Додавання значення до ArrayList виконує перетворення до object (для структур — boxing).
    protected override void Append(Product product) => _list.Add(product);

    protected override void RemoveAt(int index) => _list.RemoveAt(index);

    public override string Sort(IComparer<Product>? comparer)
    {
        if (comparer is null)
        {
            _list.Sort(); // використовує неузагальнений IComparable, реалізований у Product
            return "ArrayList.Sort() — IComparable";
        }
        _list.Sort(new NonGenericComparer<Product>(comparer));
        return "ArrayList.Sort(comparer) — IComparer (через адаптер)";
    }

    public override IEnumerator<Product> GetEnumerator()
    {
        foreach (Product p in _list) yield return p; // кожен елемент приводиться object -> Product
    }
}
