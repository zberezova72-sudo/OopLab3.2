using OopLab.Models;
using OopLab.Trees;

namespace OopLab;

/// <summary>Початковий набір із 5 товарів. Кожен виклик створює нові незалежні об'єкти.</summary>
public static class SampleData
{
    public static Product[] CreateProducts() => new[]
    {
        new Product(1003, "Хліб пшеничний", "Київхліб", 28.90m, 80),
        new Product(1001, "Молоко 2.5%", "Галичина", 42.50m, 120),
        new Product(1004, "Масло вершкове", "Тульчинка", 135.00m, 40),
        new Product(1002, "Сир кисломолочний", "Яготин", 64.75m, 55),
        new Product(1005, "Кефір 1%", "Волошкове поле", 38.20m, 90),
    };

    /// <summary>Дерево з природним порядком (IComparable&lt;Product&gt;, за кодом).</summary>
    public static BinaryTree<Product> CreateTree()
    {
        var tree = new BinaryTree<Product>();
        foreach (Product p in CreateProducts()) tree.Add(p);
        return tree;
    }
}
