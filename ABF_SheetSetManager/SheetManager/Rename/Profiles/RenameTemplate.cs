using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// Tiny token-substitution template. Tokens look like <c>{Name}</c> or <c>{Name:format}</c>.
    /// Supported formats:
    ///   <c>D&lt;n&gt;</c> — left-pad with zeros to <c>n</c> digits, e.g. <c>{Seq:D3}</c>.
    ///   (none)            — string substitution.
    /// Unknown tokens throw at <see cref="Render"/> time so missing-value bugs surface
    /// loudly instead of silently producing "{Foo}" output.
    /// </summary>
    public sealed class RenameTemplate
    {
        // Single-line, non-greedy. Allows letters, digits, underscore in the name.
        private static readonly Regex _tokenRgx = new(
            @"\{(?<name>[A-Za-z_][A-Za-z0-9_]*)(?::(?<fmt>[^}]+))?\}",
            RegexOptions.Compiled);

        private readonly string _template;

        public RenameTemplate(string template)
        {
            _template = template ?? throw new ArgumentNullException(nameof(template));
        }

        public string Render(IReadOnlyDictionary<string, object?> tokens)
        {
            var sb = new StringBuilder(_template.Length);
            int last = 0;
            foreach (Match m in _tokenRgx.Matches(_template))
            {
                if (m.Index > last) sb.Append(_template, last, m.Index - last);

                var name = m.Groups["name"].Value;
                var fmt = m.Groups["fmt"].Success ? m.Groups["fmt"].Value : null;

                if (!tokens.TryGetValue(name, out var value))
                    throw new KeyNotFoundException(
                        $"RenameTemplate: token '{{{name}}}' not provided. Template: '{_template}'.");

                sb.Append(Format(value, fmt));
                last = m.Index + m.Length;
            }

            if (last < _template.Length) sb.Append(_template, last, _template.Length - last);
            return sb.ToString();
        }

        private static string Format(object? value, string? fmt)
        {
            if (value is null) return string.Empty;
            if (fmt is null) return value.ToString() ?? string.Empty;

            // D<n> → zero-pad. Works for int directly, or for a string parsed as int.
            if (fmt.Length >= 2 && (fmt[0] == 'D' || fmt[0] == 'd')
                && int.TryParse(fmt.AsSpan(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out var width))
            {
                int n = value switch
                {
                    int i => i,
                    long l => (int)l,
                    string s when int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var p) => p,
                    _ => throw new FormatException(
                        $"RenameTemplate: cannot apply format '{fmt}' to value '{value}' of type {value.GetType().Name}.")
                };
                return n.ToString("D" + width, CultureInfo.InvariantCulture);
            }

            // Fallback: IFormattable.ToString(fmt, invariant) when supported.
            if (value is IFormattable f)
                return f.ToString(fmt, CultureInfo.InvariantCulture);

            throw new FormatException(
                $"RenameTemplate: format '{fmt}' is not supported for value '{value}' of type {value.GetType().Name}.");
        }
    }
}
