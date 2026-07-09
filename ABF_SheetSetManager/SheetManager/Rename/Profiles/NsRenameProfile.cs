using SheetSetManager.SheetManager.Rename.Engine;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// Replicates the legacy RSSNS command. Number = "{Projekt}_{Etape}_02_{NR:D3}_{Seq:D3}",
    /// title = "LEDNINGSPLAN", Emnelinje 1+2 set from parsed station range.
    /// </summary>
    internal sealed class NsRenameProfile : IRenameProfile
    {
        public const string ProfileId = "NS";

        // Stricter than VF: no leading '-', no trailing '.NNN' on FST.
        private static readonly Regex _titleRgx = new(
            @"(?<NR>\d+)\sST\s(?<FST>\d\+\d{3})\s-\s(?<SST>\d\+\d{3})\.*\d*",
            RegexOptions.Compiled);

        private const string LiteralTitle = "LEDNINGSPLAN";

        private readonly RenameTemplate _numberTemplate =
            new("{Projekt}_{Etape}_02_{NR:D3}_{Seq:D3}");

        public string Id => ProfileId;
        public string DisplayName => "Norsyn (RSSNS)";
        public string Description =>
            "Projekt / Etape — Norsyn standard: number, title=LEDNINGSPLAN, Emnelinje 1+2.";

        public IReadOnlyList<RenameInputField> InputFields { get; } = new RenameInputField[]
        {
            new TextRenameInputField("Projekt", "Projekt"),
            new TextRenameInputField("Etape",   "Etape"),
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
                    $"Title '{sheet.Title}' did not match NS regex.");
            }

            var nr = int.Parse(titleMatch.Groups["NR"].Value, CultureInfo.InvariantCulture);
            var fst = titleMatch.Groups["FST"].Value.Replace("+", "");
            var sst = titleMatch.Groups["SST"].Value.Replace("+", "");
            var seq = seqState.Next();

            var newNumber = _numberTemplate.Render(new Dictionary<string, object?>
            {
                ["Projekt"] = inputs["Projekt"],
                ["Etape"]   = inputs["Etape"],
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
