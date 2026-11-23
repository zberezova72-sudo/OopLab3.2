using System.Globalization;
using System.Text;
using OopLab;
using OopLab.Models;
using OopLab.Storage;
using OopLab.UI;

// Українські символи в консолі та єдиний формат чисел (крапка як десятковий роздільник).
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

// Складання залежностей (composition root): меню отримує все готовим через конструктор.
// Кожна колекція містить власні, незалежні копії початкових 5 товарів.
IProductStorage[] storages =
{
    new ArrayStorage(SampleData.CreateProducts()),
    new ListStorage(SampleData.CreateProducts()),
    new ArrayListStorage(SampleData.CreateProducts()),
};

var menu = new ConsoleMenu(new ConsoleIO(), storages, SampleData.CreateTree());

try
{
    menu.Run();
}
catch (EndOfStreamException)
{
    // Вхідний потік закінчився (Ctrl+Z / Ctrl+D) — завершуємо роботу.
}

Console.WriteLine("Роботу завершено.");
