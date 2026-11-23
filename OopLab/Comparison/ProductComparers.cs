using OopLab.Models;

namespace OopLab.Comparison;

/// <summary>Набір компараторів товарів (реалізації IComparer&lt;Product&gt;).</summary>
public static class ProductComparers
{
    public static readonly IComparer<Product> ByName =
        new KeyComparer<Product, string>(p => p.Name, StringComparer.CurrentCultureIgnoreCase);

    public static readonly IComparer<Product> ByManufacturer =
        new KeyComparer<Product, string>(p => p.Manufacturer, StringComparer.CurrentCultureIgnoreCase);

    public static readonly IComparer<Product> ByPrice =
        new KeyComparer<Product, decimal>(p => p.Price);

    public static readonly IComparer<Product> ByTotalCost =
        new KeyComparer<Product, decimal>(p => p.GetTotalCost());

    /// <summary>
    /// Критерії для меню. Для природного порядку компаратор = null:
    /// тоді використовується IComparable&lt;Product&gt; (порівняння за кодом).
    /// </summary>
    public static readonly (string Title, IComparer<Product>? Comparer)[] All =
    {
        ("За кодом — IComparable<Product> (природний порядок)", null),
        ("За назвою — IComparer<Product>", ByName),
        ("За виробником — IComparer<Product>", ByManufacturer),
        ("За ціною — IComparer<Product>", ByPrice),
        ("За вартістю партії — IComparer<Product>", ByTotalCost),
    };
}
