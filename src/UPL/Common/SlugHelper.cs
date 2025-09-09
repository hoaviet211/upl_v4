using System.Globalization;
using System.Text;

namespace UPL.Common;

public static class SlugHelper
{
    public static string ToSlug(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var normalized = input.Trim().ToLowerInvariant();

        normalized = normalized.Replace("đ", "d");
        normalized = normalized.Replace("Đ", "d");

        var sb = new StringBuilder();
        foreach (var c in normalized.Normalize(NormalizationForm.FormD))
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        var cleaned = sb.ToString().Normalize(NormalizationForm.FormC);

        var result = new StringBuilder();
        foreach (var c in cleaned)
        {
            if (char.IsLetterOrDigit(c)) result.Append(c);
            else if (char.IsWhiteSpace(c) || c == '-' || c == '_') result.Append('-');
        }

        var slug = result.ToString();
        while (slug.Contains("--")) slug = slug.Replace("--", "-");
        return slug.Trim('-');
    }
}

