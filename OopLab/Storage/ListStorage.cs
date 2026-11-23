using OopLab.Models;

namespace OopLab.Storage;

/// <summary>Сховище на узагальненій колекції List&lt;Product&gt; — динамічна, типобезпечна.</summary>
public sealed class ListStorage : ProductStorageBase
{
    private readonly List<Product> _list;

    public ListStorage(IEnumerable<Product> initial) => _list = new List<Product>(initial);

    public override string Title => "Узагальнена колекція List<Product>";

    public override string Description =>
        $"List<Product>: Count = {_list.Count}, Capacity = {_list.Capacity}";

    public override int Count => _list.Count;

    protected override Product GetAt(int index) => _list[index];

    protected override void SetAt(int index, Product product) => _list[index] = product;

    protected override void Append(Product product) => _list.Add(product);

    protected override void RemoveAt(int index) => _list.RemoveAt(index);

    public override string Sort(IComparer<Product>? comparer)
    {
        if (comparer is null)
        {
            _list.Sort();
            return "List<Product>.Sort() — IComparable<Product>";
        }
        _list.Sort(comparer);
        return "List<Product>.Sort(comparer) — IComparer<Product>";
    }

    public override IEnumerator<Product> GetEnumerator() => _list.GetEnumerator();
}
