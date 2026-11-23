using OopLab.Comparison;
using OopLab.Models;
using OopLab.Storage;
using OopLab.Trees;

namespace OopLab.UI;

/// <summary>
/// Інтерактивне консольне меню. Уся взаємодія з користувачем — тут; класи предметної області
/// (Product, колекції, дерево) консоллю не користуються. Залежності передаються через конструктор.
/// </summary>
public sealed class ConsoleMenu
{
    private readonly ConsoleIO _io;
    private readonly IProductStorage[] _storages;
    private BinaryTree<Product> _tree;
    private string _treeCriterion = ProductComparers.All[0].Title;

    public ConsoleMenu(ConsoleIO io, IProductStorage[] storages, BinaryTree<Product> tree)
    {
        _io = io;
        _storages = storages;
        _tree = tree;
    }

    public void Run()
    {
        while (true)
        {
            switch (ShowMenu("ГОЛОВНЕ МЕНЮ",
                        "Колекції: масив, List<T>, ArrayList (додати, видалити, оновити, знайти, обійти)",
                        "Бінарне дерево товарів (центрований обхід inorder)",
                        "Сортування: IComparable<T> та IComparer<T>",
                        "Порівняння масиву та колекцій (відмінності)"))
            {
                case 0: return;
                case 1: CollectionsMenu(); break;
                case 2: TreeMenu(); break;
                case 3: SortingMenu(); break;
                case 4: ShowComparison(); break;
            }
        }
    }

    // ===================== Колекції =====================

    private void CollectionsMenu()
    {
        while (true)
        {
            int choice = ShowMenu("ВИБІР КОЛЕКЦІЇ", _storages.Select(s => s.Title).ToArray());
            if (choice == 0) return;
            StorageMenu(_storages[choice - 1]);
        }
    }

    private void StorageMenu(IProductStorage storage)
    {
        while (true)
        {
            int choice = ShowMenu(storage.Title,
                "Показати всі товари (прохід foreach)",
                "Додати товар",
                "Видалити товар за кодом",
                "Оновити товар за кодом",
                "Знайти товар за кодом (інформація про товар)",
                "Знайти товари за назвою",
                "Збільшити ціну товару на відсоток",
                "Загальна вартість партії товару",
                "Загальна вартість усіх партій");
            if (choice == 0) return;

            Execute(() =>
            {
                switch (choice)
                {
                    case 1:
                        PrintProducts(storage);
                        break;
                    case 2:
                        storage.Add(ReadNewProduct(code => storage.FindByCode(code) is not null));
                        _io.Print("Товар додано.");
                        break;
                    case 3:
                        _io.Print(storage.RemoveByCode(ReadCode()) ? "Товар видалено." : NotFound);
                        break;
                    case 4:
                        UpdateInStorage(storage);
                        break;
                    case 5:
                        PrintInfo(storage.FindByCode(ReadCode()));
                        break;
                    case 6:
                        PrintProducts(storage.FindByName(ReadName()));
                        break;
                    case 7:
                        IncreasePrice(storage.FindByCode(ReadCode()), p => p.IncreasePrice(ReadPercent()));
                        break;
                    case 8:
                        PrintTotal(storage.FindByCode(ReadCode()));
                        break;
                    case 9:
                        _io.Print($"Загальна вартість усіх партій: {storage.Sum(p => p.GetTotalCost()):F2}");
                        break;
                }
            });
        }
    }

    private void UpdateInStorage(IProductStorage storage)
    {
        int code = ReadCode();
        Product? old = storage.FindByCode(code);
        if (old is null) { _io.Print(NotFound); return; }

        // Створюємо новий об'єкт з тим самим кодом і замінюємо ним старий елемент колекції.
        Product updated = ReadUpdatedProduct(old);
        storage.Update(code, updated);
        _io.Print("Товар оновлено.");
    }

    // ===================== Бінарне дерево =====================

    private void TreeMenu()
    {
        while (true)
        {
            int choice = ShowMenu($"БІНАРНЕ ДЕРЕВО (порядок: {_treeCriterion})",
                "Показати всі товари (inorder, foreach через власний ітератор)",
                "Показати структуру дерева",
                "Додати товар",
                "Видалити товар за кодом",
                "Знайти товар за кодом (інформація про товар)",
                "Знайти товари за назвою",
                "Збільшити ціну товару на відсоток",
                "Змінити критерій порівняння (перебудувати дерево)",
                "Інформація про дерево");
            if (choice == 0) return;

            Execute(() =>
            {
                switch (choice)
                {
                    case 1:
                        PrintProducts(_tree);
                        break;
                    case 2:
                        _io.Print(_tree.Count == 0
                            ? "Дерево порожнє."
                            : "(праве піддерево зверху, ліве знизу; відступ = глибина)\n" +
                              _tree.ToStructureString(p => $"{p.Code} {p.Name}"));
                        break;
                    case 3:
                        _tree.Add(ReadNewProduct(code => FindInTree(code) is not null));
                        _io.Print("Товар додано.");
                        break;
                    case 4:
                        Product? toRemove = FindInTree(ReadCode());
                        _io.Print(toRemove is not null && _tree.Remove(toRemove) ? "Товар видалено." : NotFound);
                        break;
                    case 5:
                        PrintInfo(FindInTree(ReadCode()));
                        break;
                    case 6:
                        string text = ReadName();
                        PrintProducts(_tree.Where(p => p.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase)));
                        break;
                    case 7:
                        IncreasePriceInTree();
                        break;
                    case 8:
                        int index = ChooseCriterion();
                        if (index >= 0) ApplyCriterionToTree(index);
                        break;
                    case 9:
                        _io.Print($"Елементів: {_tree.Count}, висота: {_tree.Height}, критерій: {_treeCriterion}");
                        break;
                }
            });
        }
    }

    // Ціна може бути ключем порівняння, тому змінюємо товар «поза» деревом: видалити → змінити → додати.
    private void IncreasePriceInTree()
    {
        Product? product = FindInTree(ReadCode());
        if (product is null) { _io.Print(NotFound); return; }

        decimal percent = ReadPercent();
        _tree.Remove(product);
        try { product.IncreasePrice(percent); }
        finally { _tree.Add(product); }
        _io.Print($"Ціну збільшено: {product.GetInfo()}");
    }

    private Product? FindInTree(int code) => _tree.Find(p => p.Code == code);

    private void ApplyCriterionToTree(int index)
    {
        (string title, IComparer<Product>? comparer) = ProductComparers.All[index];
        _tree = _tree.Rebuild(comparer);
        _treeCriterion = title;
    }

    // ===================== Сортування =====================

    private void SortingMenu()
    {
        int index = ChooseCriterion();
        if (index < 0) return;

        (string title, IComparer<Product>? comparer) = ProductComparers.All[index];
        _io.Print($"Критерій сортування: {title}");

        foreach (IProductStorage storage in _storages)
        {
            string method = storage.Sort(comparer);
            _io.Print($"\n--- {storage.Title}: {method} ---");
            PrintProducts(storage);
        }

        ApplyCriterionToTree(index);
        _io.Print($"\n--- Бінарне дерево: Array.Sort + перебудова з IComparer<Product>, обхід inorder ---");
        PrintProducts(_tree);
    }

    private int ChooseCriterion() =>
        ShowMenu("КРИТЕРІЙ ПОРІВНЯННЯ", ProductComparers.All.Select(c => c.Title).ToArray()) - 1;

    // ===================== Порівняння колекцій =====================

    private void ShowComparison()
    {
        _io.Print("\nПОТОЧНИЙ СТАН КОЛЕКЦІЙ:");
        foreach (IProductStorage s in _storages) _io.Print($"  • {s.Title}: {s.Description}");

        _io.Print(@"
ВІДМІННОСТІ:
  Масив Product[]
    - розмір фіксований: додавання/видалення = створення нового масиву (Array.Resize) і копіювання, O(n);
    - доступ за індексом O(1), найменші витрати пам'яті; типобезпечний.
  List<Product> (узагальнена колекція)
    - динамічний розмір: внутрішній масив з запасом (Capacity), додавання в кінець амортизовано O(1);
    - видалення/вставка в середині зсуває елементи, O(n); типобезпечний, без boxing; багатий API.
  ArrayList (неузагальнена колекція)
    - зберігає елементи як object: потрібні явні приведення типів, тип перевіряється лише під час виконання
      (у список можна додати будь-який об'єкт); для структур виникає boxing/unboxing;
    - застаріла, у новому коді замість неї використовують List<T>.
  Спільне: пошук за кодом лінійний O(n), обхід — foreach (IEnumerable). Бінарне дерево пошуку
  дає пошук/вставку за O(h), де h — висота дерева (до цього потрібен впорядкований ключ).");
    }

    // ===================== Допоміжне введення/виведення =====================

    private const string NotFound = "Товар з таким кодом не знайдено.";

    // Показує меню і повертає номер пункту (0 — назад/вихід).
    private int ShowMenu(string title, params string[] items)
    {
        _io.Print($"\n=== {title} ===");
        for (int i = 0; i < items.Length; i++) _io.Print($" {i + 1}. {items[i]}");
        _io.Print(" 0. Назад / вихід");
        return _io.ReadChoice(items.Length);
    }

    // Помилки предметної області (некоректні дані) не завершують програму.
    private void Execute(Action action)
    {
        try { action(); }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            _io.Print($"Помилка: {e.Message}");
        }
    }

    private void PrintProducts(IEnumerable<Product> products)
    {
        int n = 0;
        foreach (Product p in products) _io.Print($"{++n,3}. {p.GetInfo()}");
        if (n == 0) _io.Print("  (немає товарів)");
    }

    private void PrintInfo(Product? product) => _io.Print(product?.GetInfo() ?? NotFound);

    private void PrintTotal(Product? product) =>
        _io.Print(product is null ? NotFound : $"Загальна вартість партії «{product.Name}»: {product.GetTotalCost():F2}");

    private void IncreasePrice(Product? product, Action<Product> increase)
    {
        if (product is null) { _io.Print(NotFound); return; }
        increase(product);
        _io.Print($"Ціну збільшено: {product.GetInfo()}");
    }

    private int ReadCode() =>
        _io.Read<int>("Код товару (додатне ціле)", Validators.TryCode, "Код — додатне ціле число (до 9 цифр).");

    private string ReadName() =>
        _io.Read<string>("Назва", Validators.TryText, TextError);

    private decimal ReadPercent() =>
        _io.Read<decimal>("Відсоток збільшення (0–1000)", Validators.TryPercent, "Введіть число від 0 до 1000 (до 2 знаків після коми).");

    private const string TextError = "Допустимі літери, цифри, пробіл та .,'&\"-%/() (1–50 символів, перший — літера чи цифра).";

    private Product ReadNewProduct(Func<int, bool> codeExists)
    {
        int code;
        while (codeExists(code = ReadCode())) _io.Print("  ! Товар з таким кодом вже існує.");

        string name = _io.Read<string>("Назва", Validators.TryText, TextError);
        string manufacturer = _io.Read<string>("Виробник", Validators.TryText, TextError);
        decimal price = _io.Read<decimal>("Ціна за одиницю", Validators.TryPrice, "Введіть невід'ємне число (до 2 знаків після коми).");
        int quantity = _io.Read<int>("Кількість у партії", Validators.TryQuantity, "Введіть невід'ємне ціле число (до 7 цифр).");
        return new Product(code, name, manufacturer, price, quantity);
    }

    private Product ReadUpdatedProduct(Product old)
    {
        string name = _io.ReadOrKeep("Назва", Validators.TryText, TextError, old.Name);
        string manufacturer = _io.ReadOrKeep("Виробник", Validators.TryText, TextError, old.Manufacturer);
        decimal price = _io.ReadOrKeep("Ціна за одиницю", Validators.TryPrice, "Введіть невід'ємне число (до 2 знаків після коми).", old.Price);
        int quantity = _io.ReadOrKeep("Кількість у партії", Validators.TryQuantity, "Введіть невід'ємне ціле число (до 7 цифр).", old.Quantity);
        return new Product(old.Code, name, manufacturer, price, quantity);
    }
}
