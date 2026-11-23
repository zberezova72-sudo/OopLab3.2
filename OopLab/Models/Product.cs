namespace OopLab.Models;

/// <summary>
/// Товар (варіант №3): код, назва, виробник, вартість, кількість у партії.
/// Клас містить лише логіку предметної області — жодного вводу/виводу на консоль.
/// Реалізує IComparable&lt;Product&gt; (природний порядок — за кодом) та IEquatable&lt;Product&gt; (рівність за кодом).
/// </summary>
public class Product : IComparable<Product>, IComparable, IEquatable<Product>
{
    private int _code;
    private string _name = string.Empty;
    private string _manufacturer = string.Empty;
    private decimal _price;
    private int _quantity;

    public Product(int code, string name, string manufacturer, decimal price, int quantity)
    {
        Code = code;
        Name = name;
        Manufacturer = manufacturer;
        Price = price;
        Quantity = quantity;
    }

    /// <summary>Унікальний код товару. Після створення не змінюється (є ключем).</summary>
    public int Code
    {
        get => _code;
        private set => _code = value > 0
            ? value
            : throw new ArgumentException("Код товару має бути додатним числом.");
    }

    /// <summary>Назва товару (не може бути порожньою).</summary>
    public string Name
    {
        get => _name;
        set => _name = RequireText(value, "Назва");
    }

    /// <summary>Виробник (не може бути порожнім).</summary>
    public string Manufacturer
    {
        get => _manufacturer;
        set => _manufacturer = RequireText(value, "Виробник");
    }

    /// <summary>Вартість одиниці товару (не від'ємна, округлюється до копійок).</summary>
    public decimal Price
    {
        get => _price;
        set => _price = value >= 0
            ? Math.Round(value, 2, MidpointRounding.AwayFromZero)
            : throw new ArgumentException("Вартість не може бути від'ємною.");
    }

    /// <summary>Кількість одиниць у партії (не від'ємна).</summary>
    public int Quantity
    {
        get => _quantity;
        set => _quantity = value >= 0
            ? value
            : throw new ArgumentException("Кількість не може бути від'ємною.");
    }

    /// <summary>Загальна вартість партії = вартість одиниці × кількість.</summary>
    public decimal GetTotalCost() => Price * Quantity;

    /// <summary>Збільшує вартість товару на вказаний відсоток (змінює стан об'єкта).</summary>
    public void IncreasePrice(decimal percent)
    {
        if (percent < 0)
            throw new ArgumentException("Відсоток збільшення не може бути від'ємним.");
        Price += Price * percent / 100m;
    }

    /// <summary>Повертає текст з інформацією про товар (виведення на екран — справа інтерфейсу).</summary>
    public string GetInfo() =>
        $"Код: {Code} | Назва: {Name} | Виробник: {Manufacturer} | " +
        $"Ціна: {Price:F2} | Кількість: {Quantity} | Вартість партії: {GetTotalCost():F2}";

    public override string ToString() => GetInfo();

    // ---- Порівняння та рівність: природний порядок і рівність визначає код товару ----

    public int CompareTo(Product? other) => other is null ? 1 : Code.CompareTo(other.Code);

    // Явна реалізація неузагальненого IComparable (потрібна, зокрема, для ArrayList.Sort()).
    int IComparable.CompareTo(object? obj) => obj switch
    {
        null => 1,
        Product p => CompareTo(p),
        _ => throw new ArgumentException("Об'єкт має бути типу Product.")
    };

    public bool Equals(Product? other) => other is not null && Code == other.Code;
    public override bool Equals(object? obj) => Equals(obj as Product);
    public override int GetHashCode() => Code;

    private static string RequireText(string? value, string field) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"Поле «{field}» не може бути порожнім.")
            : value.Trim();
}
