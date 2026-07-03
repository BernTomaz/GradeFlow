using System.Globalization;

namespace GradeFlow.Application.Corrections;

internal static class NumberParser
{
    public static bool TryParse(string value, out decimal result)
        => decimal.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out result);
}
