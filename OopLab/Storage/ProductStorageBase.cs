using System.Collections;
using OopLab.Models;

namespace OopLab.Storage;

/// <summary>
/// Спільна логіка пошуку, додавання, видалення та оновлення (шаблонний метод).
/// Нащадки реалізують лише примітивні операції над своєю колекцією.
/// </summary>
public abstract class ProductStorageBase : IProductStorage
{
    public abstract string Title { get; }
    public abstract string Description { get; }
    public abstract int Count { get; }
    public abstract string Sort(IComparer<Product>? comparer);
    public abstract IEnumerator<Product> GetEnumerator();

    // Примітивні операції конкретної колекції.
    protected abstract Product GetAt(int index);
    protected abstract void SetAt(int index, Product product);
    protected abstract void Append(Product product);
    protected abstract void RemoveAt(int index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (IndexOfCode(product.Code) >= 0)
            throw new InvalidOperationException($"Товар з кодом {product.Code} вже існує.");
        Append(product);
    }

    public bool RemoveByCode(int code)
    {
        int index = IndexOfCode(code);
        if (index < 0) return false;
        RemoveAt(index);
        return true;
    }

    public bool Update(int code, Product updated)
    {
        ArgumentNullException.ThrowIfNull(updated);
        if (updated.Code != code)
            throw new ArgumentException("Код товару при оновленні змінювати не можна.");
        int index = IndexOfCode(code);
        if (index < 0) return false;
        SetAt(index, updated);
        return true;
    }

    public Product? FindByCode(int code)
    {
        int index = IndexOfCode(code);
        return index < 0 ? null : GetAt(index);
    }

    public IEnumerable<Product> FindByName(string text) =>
        this.Where(p => p.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase));

    // Лінійний пошук індексу за кодом (колекції не впорядковані за кодом).
    private int IndexOfCode(int code)
    {
        for (int i = 0; i < Count; i++)
            if (GetAt(i).Code == code) return i;
        return -1;
    }
}
