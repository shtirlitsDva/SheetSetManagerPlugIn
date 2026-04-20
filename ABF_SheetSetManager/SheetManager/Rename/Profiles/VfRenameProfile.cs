using SheetSetManager.SheetManager.Rename.Engine;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// Replicates the legacy RSSVF command, plus the new SeparatorStyle requirement.
    /// Title-source regex extracts NR / FST / SST. Sets number, title="LEDNINGSPLAN",
    /// and Emnelinje 1/2 custom properties.
    /// </summary>
    internal sealed class VfRenameProfile : IRenameProfile
    {
        public const string ProfileId = "VF";

        // Legacy regex preserved verbatim.
        private static readonly Regex _titleRgx = new(
            @"(?<NR>\d+)\sST\s-?(?<FST>\d\+\d{3})(\.\d+)?\s-\s(?<SST>\d\+\d{3})\.*\d*",
            RegexOptions.Compiled);

        private const string LiteralTitle = "LEDNINGSPLAN";

        // Two number templates, one per separator style. Last two segments are always
        // underscore-joined per the new requirement.
        private readonly RenameTemplate _numberAllUnderscores =
            new("{Program}_{Komm}_{Energi}_{NR:D3}_{Seq:D3}");
        private readonly RenameTemplate _numberDashesExceptLastTwo =
            new("{Program}-{Komm}-{Energi}_{NR:D3}_{Seq:D3}");

        public string Id => ProfileId;
        public string DisplayName => "Vestforbrænding (RSSVF)";
        public string Description =>
            "Program / Kommunekode / Energidistrikt — number, title=LEDNINGSPLAN, Emnelinje 1+2.";

        private const string SepAllUnderscores = "AllUnderscores";
        private const string SepDashesExceptLastTwo = "DashesExceptLastTwo";

        public IReadOnlyList<RenameInputField> InputFields { get; } = new RenameInputField[]
        {
            new TextRenameInputField("Program", "Program"),
            new TextRenameInputField("Komm",    "VF kommunekode"),
            new TextRenameInputField("Energi",  "Energidistrikt"),
            new ChoiceRenameInputField(
                name: "SeparatorStyle",
                label: "Separator style",
                choices: new[]
                {
                    new RenameChoice(SepAllUnderscores,        "All underscores  (A_B_C_NN_SS)"),
                    new RenameChoice(SepDashesExceptLastTwo,   "Dashes, last two underscores  (A-B-C_NN_SS)"),
                },
                defaultValue: SepAllUnderscores),
        };

        public RenamePreviewRow BuildPreview(
            RenameSheetData sheet,
            IReadOnlyDictionary<string, string> inputs,
            RenameSequenceState seqState)
        {
            var titleMatch = _titleRgx.Match(sheet.Title);
            if (!titleMatch.Success)
            {
                return new RenamePreviewRow(
                    sheet.OriginalIndex, sheet.SubsetName,
                    sheet.Number, null,
                    sheet.Title, null,
                    Array.Empty<RenamePropertyChange>(),
                    RenameStatus.Error,
                    $"Title '{sheet.Title}' did not match VF regex.");
            }

            var nr = int.Parse(titleMatch.Groups["NR"].Value, CultureInfo.InvariantCulture);
            var fst = titleMatch.Groups["FST"].Value.Replace("+", "");
            var sst = titleMatch.Groups["SST"].Value.Replace("+", "");
            var seq = seqState.Next();

            var template = inputs["SeparatorStyle"] switch
            {
                SepDashesExceptLastTwo => _numberDashesExceptLastTwo,
                _                      => _numberAllUnderscores,
            };

            var newNumber = template.Render(new Dictionary<string, object?>
            {
                ["Program"] = inputs["Program"],
                ["Komm"]    = inputs["Komm"],
                ["Energi"]  = inputs["Energi"],
                ["NR"]      = nr,
                ["Seq"]     = seq,
            });

            var nrPadded = nr.ToString("D3", CultureInfo.InvariantCulture);
            var newEmne1 = $"STRÆKNING {nrPadded}";
            var newEmne2 = $"ST {fst} - {sst}";

            var changes = new List<RenamePropertyChange>(2);
            AddPropChange(changes, sheet, "Emnelinje 1", newEmne1);
            AddPropChange(changes, sheet, "Emnelinje 2", newEmne2);

            var numberChanged = newNumber != sheet.Number;
            var titleChanged  = LiteralTitle != sheet.Title;
            var anyChange = numberChanged || titleChanged || changes.Count > 0;
            var status = anyChange ? RenameStatus.Matched : RenameStatus.NoChange;

            return new RenamePreviewRow(
                sheet.OriginalIndex, sheet.SubsetName,
                sheet.Number, numberChanged ? newNumber : null,
                sheet.Title, titleChanged ? LiteralTitle : null,
                changes,
                status);
        }

        private static void AddPropChange(
            List<RenamePropertyChange> changes, RenameSheetData sheet,
            string propName, string newValue)
        {
            sheet.CustomProperties.TryGetValue(propName, out var oldValue);
            if (oldValue != newValue)
                changes.Add(new RenamePropertyChange(propName, oldValue, newValue));
        }
    }
}
