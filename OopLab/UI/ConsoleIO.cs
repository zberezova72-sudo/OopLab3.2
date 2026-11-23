namespace OopLab.UI;

/// <summary>Делегат безпечного розбору рядка: повертає true, якщо введення коректне.</summary>
public delegate bool TryParser<T>(string input, out T value);

/// <summary>Низькорівневий ввід/вивід на консоль. Уся робота з Console зосереджена тут.</summary>
public sealed class ConsoleIO
{
    public void Print(string text = "") => Console.WriteLine(text);

    /// <summary>Читає рядок; при кінці вхідного потоку кидає EndOfStreamException, щоб не зациклитись.</summary>
    public string Prompt(string text)
    {
        Console.Write($"{text}: ");
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }

    /// <summary>Запитує значення, доки введення не пройде перевірку.</summary>
    public T Read<T>(string prompt, TryParser<T> parser, string error)
    {
        while (true)
        {
            if (parser(Prompt(prompt).Trim(), out T value)) return value;
            Print($"  ! {error}");
        }
    }

    /// <summary>Те саме, але порожній рядок залишає поточне значення без змін.</summary>
    public T ReadOrKeep<T>(string prompt, TryParser<T> parser, string error, T current)
    {
        while (true)
        {
            string input = Prompt($"{prompt} [Enter — залишити «{current}»]").Trim();
            if (input.Length == 0) return current;
            if (parser(input, out T value)) return value;
            Print($"  ! {error}");
        }
    }

    /// <summary>Читає номер пункту меню від 0 до max.</summary>
    public int ReadChoice(int max) =>
        Read<int>("Ваш вибір",
            (string s, out int v) => int.TryParse(s, out v) && v >= 0 && v <= max,
            $"Введіть число від 0 до {max}.");
}
