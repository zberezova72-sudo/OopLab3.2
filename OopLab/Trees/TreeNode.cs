namespace OopLab.Trees;

/// <summary>
/// Вузол бінарного дерева. Це клас (а не структура); обмеження where T : class
/// гарантує, що елементами дерева не можуть бути структури.
/// </summary>
internal sealed class TreeNode<T> where T : class
{
    public TreeNode(T value) => Value = value;

    public T Value { get; set; }
    public TreeNode<T>? Left { get; set; }
    public TreeNode<T>? Right { get; set; }
}
