using System.Collections;
using System.Text;

namespace OopLab.Trees;

/// <summary>
/// Узагальнене бінарне дерево пошуку. Порівняння елементів виконує IComparer&lt;T&gt;
/// (за замовчуванням — IComparable&lt;T&gt; самого елемента). IEnumerable&lt;T&gt; повертає власний
/// ітератор з центрованим (inorder) обходом.
/// Інваріант: у лівому піддереві — елементи строго менші за вузол, у правому — більші або рівні.
/// </summary>
public class BinaryTree<T> : IEnumerable<T> where T : class
{
    private TreeNode<T>? _root;

    public BinaryTree(IComparer<T>? comparer = null) => Comparer = comparer ?? Comparer<T>.Default;

    /// <summary>Компаратор, що визначає порядок елементів у дереві.</summary>
    public IComparer<T> Comparer { get; }

    public int Count { get; private set; }

    /// <summary>Висота дерева (порожнє — 0, один вузол — 1).</summary>
    public int Height => HeightOf(_root);

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var node = new TreeNode<T>(item);
        Count++;

        if (_root is null)
        {
            _root = node;
            return;
        }

        TreeNode<T> current = _root;
        while (true)
        {
            if (Comparer.Compare(item, current.Value) < 0)
            {
                if (current.Left is null) { current.Left = node; return; }
                current = current.Left;
            }
            else
            {
                if (current.Right is null) { current.Right = node; return; }
                current = current.Right;
            }
        }
    }

    /// <summary>Видаляє саме цей елемент (за Equals). Повертає false, якщо його немає.</summary>
    public bool Remove(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        bool removed = false;
        _root = Remove(_root, item, ref removed);
        if (removed) Count--;
        return removed;
    }

    /// <summary>Повертає перший елемент (в порядку inorder), що задовольняє умову, або null.</summary>
    public T? Find(Predicate<T> match)
    {
        foreach (T item in this)
            if (match(item)) return item;
        return null;
    }

    public void Clear()
    {
        _root = null;
        Count = 0;
    }

    /// <summary>
    /// Створює нове дерево з іншим компаратором. Елементи сортуються методом Array.Sort
    /// (з IComparer) і вставляються «медіаною вперед», тому нове дерево отримується збалансованим.
    /// </summary>
    public BinaryTree<T> Rebuild(IComparer<T>? comparer)
    {
        var tree = new BinaryTree<T>(comparer);
        T[] items = this.ToArray();
        Array.Sort(items, tree.Comparer);
        tree.AddRange(items, 0, items.Length - 1);
        return tree;
    }

    /// <summary>Текстове подання структури: праве піддерево зверху, ліве знизу, відступ = глибина.</summary>
    public string ToStructureString(Func<T, string> format)
    {
        var sb = new StringBuilder();
        AppendNode(_root, 0, sb, format);
        return sb.ToString();
    }

    public IEnumerator<T> GetEnumerator() => new InOrderEnumerator<T>(_root);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ---- допоміжні методи ----

    private TreeNode<T>? Remove(TreeNode<T>? node, T item, ref bool removed)
    {
        if (node is null) return null;

        int cmp = Comparer.Compare(item, node.Value);
        if (cmp == 0 && Equals(item, node.Value))
        {
            removed = true;
            if (node.Left is null) return node.Right;
            if (node.Right is null) return node.Left;

            // Два нащадки: беремо найменший елемент правого піддерева (наступник в inorder).
            TreeNode<T> successor = node.Right;
            while (successor.Left is not null) successor = successor.Left;
            node.Value = successor.Value;
            node.Right = RemoveMin(node.Right);
            return node;
        }

        // Рівні за ключем, але не той самий елемент — шукаємо праворуч (так вставляються дублікати).
        if (cmp < 0) node.Left = Remove(node.Left, item, ref removed);
        else node.Right = Remove(node.Right, item, ref removed);
        return node;
    }

    private static TreeNode<T>? RemoveMin(TreeNode<T> node)
    {
        if (node.Left is null) return node.Right;
        node.Left = RemoveMin(node.Left);
        return node;
    }

    private void AddRange(T[] sorted, int low, int high)
    {
        if (low > high) return;
        int mid = (low + high) / 2;
        Add(sorted[mid]);
        AddRange(sorted, low, mid - 1);
        AddRange(sorted, mid + 1, high);
    }

    private static int HeightOf(TreeNode<T>? node) =>
        node is null ? 0 : 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));

    private static void AppendNode(TreeNode<T>? node, int depth, StringBuilder sb, Func<T, string> format)
    {
        if (node is null) return;
        AppendNode(node.Right, depth + 1, sb, format);
        sb.Append(' ', depth * 6).AppendLine(format(node.Value));
        AppendNode(node.Left, depth + 1, sb, format);
    }
}
