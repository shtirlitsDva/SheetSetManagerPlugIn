using SheetSetManager.SheetManager.Rename.Engine;

using System.Text.RegularExpressions;

namespace SheetSetManager.SheetManager.Rename.Profiles.Vf
{
    /// <summary>
    /// Pure classifier. Given a sheet snapshot, returns the <see cref="VfSheetState"/>
    /// that uniquely matches its current Number / Title / Emnelinje shape.
    /// </summary>
    /// <remarks>
    /// The classifier is the single source of truth for "what state is this sheet in".
    /// Strategies trust the result and do not re-detect.
    ///
    /// Discrimination rules:
    ///  - <see cref="VfSheetState.CivilNascent"/>: Title still carries the Civil regex shape.
    ///  - <see cref="VfSheetState.VfAllUnderscores"/> / <see cref="VfSheetState.VfDashesExceptLast2"/>:
    ///    Title is the literal "LEDNINGSPLAN" AND Number matches the corresponding shape.
    ///    Only one of the two number shapes can match because the prefix character classes
    ///    forbid the other separator.
    ///  - <see cref="VfSheetState.Unknown"/>: anything else.
    /// </remarks>
    internal sealed class VfStateClassifier
    {
        internal const string LiteralVfTitle = "LEDNINGSPLAN";

        // Civil sheet-creation tool title, e.g. "5 ST -0+000 - 1+500".
        internal static readonly Regex CivilTitleRgx = new(
            @"(?<NR>\d+)\sST\s-?(?<FST>\d\+\d{3})(\.\d+)?\s-\s(?<SST>\d\+\d{3})\.*\d*",
            RegexOptions.Compiled);

        // VF number shapes. Prefix segments forbid both separator characters so a number
        // is unambiguously one shape or the other.
        internal static readonly Regex VfAllUnderscoresNumberRgx = new(
            @"^(?<P1>[^-_]+)_(?<P2>[^-_]+)_(?<P3>[^-_]+)_(?<NR>\d{3})_(?<Seq>\d{3})$",
            RegexOptions.Compiled);

        internal static readonly Regex VfDashesExceptLast2NumberRgx = new(
            @"^(?<P1>[^-_]+)-(?<P2>[^-_]+)-(?<P3>[^-_]+)_(?<NR>\d{3})_(?<Seq>\d{3})$",
            RegexOptions.Compiled);

        public VfSheetState Classify(RenameSheetData sheet)
        {
            if (CivilTitleRgx.IsMatch(sheet.Title))
                return VfSheetState.CivilNascent;

            if (sheet.Title == LiteralVfTitle)
            {
                if (VfAllUnderscoresNumberRgx.IsMatch(sheet.Number))
                    return VfSheetState.VfAllUnderscores;
                if (VfDashesExceptLast2NumberRgx.IsMatch(sheet.Number))
                    return VfSheetState.VfDashesExceptLast2;
            }

            return VfSheetState.Unknown;
        }
    }
}
