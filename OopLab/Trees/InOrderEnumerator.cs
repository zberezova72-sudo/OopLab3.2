using System.Collections;

namespace OopLab.Trees;

/// <summary>
/// Власний ітератор: центрований (inorder) обхід — ліве піддерево, вузол, праве піддерево.
/// Реалізований без рекурсії через явний стек; для впорядкованого дерева дає елементи за зростанням.
/// </summary>
public sealed class InOrderEnumerator<T> : IEnumerator<T> where T : class
{
    private readonly TreeNode<T>? _root;
    private readonly Stack<TreeNode<T>> _stack = new();
    private T? _current;

    internal InOrderEnumerator(TreeNode<T>? root)
    {
        _root = root;
        Reset();
    }

    public T Current => _current ?? throw new InvalidOperationException("Перебір не розпочато або вже завершено.");

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (_stack.Count == 0)
        {
            _current = null;
            return false;
        }

        TreeNode<T> node = _stack.Pop();
        _current = node.Value;
        PushLeftChain(node.Right); // після вузла треба обійти його праве піддерево
        return true;
    }

    public void Reset()
    {
        _stack.Clear();
        _current = null;
        PushLeftChain(_root);
    }

    public void Dispose() => _stack.Clear();

    // Кладе у стек вузол і весь ланцюжок його лівих нащадків.
    private void PushLeftChain(TreeNode<T>? node)
    {
        while (node is not null)
        {
            _stack.Push(node);
            node = node.Left;
        }
    }
}
