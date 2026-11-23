using OopLab.Models;

namespace OopLab.Storage;

/// <summary>
/// Абстракція сховища товарів. Меню працює лише з цим інтерфейсом, тому масив,
/// List&lt;T&gt; і ArrayList взаємозамінні (слабка зв'язаність).
/// </summary>
public interface IProductStorage : IEnumerable<Product>
{
    /// <summary>Назва колекції для відображення.</summary>
    string Title { get; }

    /// <summary>Поточні характеристики колекції (тип, розмір, ємність).</summary>
    string Description { get; }

    int Count { get; }

    /// <summary>Додає товар. Кидає InvalidOperationException, якщо код уже існує.</summary>
    void Add(Product product);

    /// <summary>Видаляє товар за кодом. Повертає false, якщо товару немає.</summary>
    bool RemoveByCode(int code);

    /// <summary>Замінює товар з вказаним кодом на новий об'єкт. Повертає false, якщо товару немає.</summary>
    bool Update(int code, Product updated);

    Product? FindByCode(int code);

    IEnumerable<Product> FindByName(string text);

    /// <summary>
    /// Сортує колекцію власним методом сортування. comparer == null — природний порядок (IComparable).
    /// Повертає текстову назву викликаного методу .NET (для відображення).
    /// </summary>
    string Sort(IComparer<Product>? comparer);
}
