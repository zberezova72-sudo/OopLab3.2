using System.Globalization;
using System.Text.RegularExpressions;

namespace OopLab.UI;

/// <summary>Перевірка введених рядків (регулярні вирази та діапазони). Сигнатури відповідають TryParser&lt;T&gt;.</summary>
public static class Validators
{
    // Назва/виробник: починається з літери або цифри; далі літери, цифри, пробіл та .,'&"-%/()
    private static readonly Regex TextRegex = new(@"^[\p{L}\d][\p{L}\d .,'&""\-%/()]{0,49}$");
    private static readonly Regex CodeRegex = new(@"^\d{1,9}$");
    private static readonly Regex QuantityRegex = new(@"^\d{1,7}$");
    private static readonly Regex DecimalRegex = new(@"^\d{1,10}([.,]\d{1,2})?$");

    public static bool TryText(string s, out string value)
    {
        value = s;
        return TextRegex.IsMatch(s);
    }

    public static bool TryCode(string s, out int value)
    {
        value = 0;
        return CodeRegex.IsMatch(s) && int.TryParse(s, out value) && value > 0;
    }

    public static bool TryQuantity(string s, out int value)
    {
        value = 0;
        return QuantityRegex.IsMatch(s) && int.TryParse(s, out value);
    }

    public static bool TryPrice(string s, out decimal value) => TryDecimal(s, 1_000_000_000m, out value);

    public static bool TryPercent(string s, out decimal value) => TryDecimal(s, 1000m, out value);

    private static bool TryDecimal(string s, decimal max, out decimal value)
    {
        value = 0;
        return DecimalRegex.IsMatch(s)
               && decimal.TryParse(s.Replace(',', '.'), NumberStyles.AllowDecimalPoint,
                   CultureInfo.InvariantCulture, out value)
               && value <= max;
    }
}
