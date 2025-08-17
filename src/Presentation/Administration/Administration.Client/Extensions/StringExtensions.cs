using System.Globalization;

namespace Administration.Client.Extensions;

public static class StringExtensions
{
    public static string CapitalizeFirstLetterOfFirstWord(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public static string CapitalizeFirstLetterOfEachWord(this string input)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLower());
    }
}
