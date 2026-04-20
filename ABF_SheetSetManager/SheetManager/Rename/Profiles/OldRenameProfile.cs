using SheetSetManager.SheetManager.Rename.Engine;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// Replicates the legacy RSSOLD command. Number = "{Project}-{Etape}-{SheetType}{Pipeline}-{Seq:D3}".
    /// Pipeline number is parsed from the subset name. Title is cleaned (station fragments stripped),
    /// not replaced by a literal.
    /// </summary>
    internal sealed class OldRenameProfile : IRenameProfile
    {
        public const string ProfileId = "OLD";

        // Pipeline number lives in the subset name as 2-3 digits.
        private static readonly Regex _subsetPipelineRgx = new(@"(?<number>\d{2,3})", RegexOptions.Compiled);

        // Strip station fragments like ".123" from the title.
        private static readonly Regex _stationFragmentRgx = new(@"\d(?<rest>\.\d\d\d)", RegexOptions.Compiled);

        private readonly RenameTemplate _numberTemplate =
            new("{Project}-{Etape}-{SheetType}{Pipeline}-{Seq:D3}");

        public string Id => ProfileId;
        public string DisplayName => "Old (RSSOLD)";
        public string Description =>
            "Project-Etape-TypePipeline-Seq. Pipeline read from subset name; title cleaned of station fragments.";

        public IReadOnlyList<RenameInputField> InputFields { get; } = new RenameInputField[]
        {
            new TextRenameInputField("Project",   "Project number"),
            new TextRenameInputField("Etape",     "Phase (etape)"),
            new TextRenameInputField("SheetType", "Sheet type number"),
        };

        public RenamePreviewRow BuildPreview(
            RenameSheetData sheet,
            IReadOnlyDictionary<string, string> inputs,
            RenameSequenceState seqState)
        {
            var subsetMatch = _subsetPipelineRgx.Match(sheet.SubsetName);
            if (!subsetMatch.Success)
            {
                return new RenamePreviewRow(
                    sheet.OriginalIndex, sheet.SubsetName,
                    sheet.Number, null,
                    sheet.Title, null,
                    System.Array.Empty<RenamePropertyChange>(),
                    RenameStatus.Error,
                    $"Subset name '{sheet.SubsetName}' has no pipeline number (2-3 digits).");
            }

            var pipeline = subsetMatch.Groups["number"].Value.PadLeft(3, '0');
            var seq = seqState.Next();

            var newNumber = _numberTemplate.Render(new Dictionary<string, object?>
            {
                ["Project"]   = inputs["Project"],
                ["Etape"]     = inputs["Etape"],
                ["SheetType"] = inputs["SheetType"],
                ["Pipeline"]  = pipeline,
                ["Seq"]       = seq,
            });

            // Title cleanup mirrors the legacy: strip ".XXX" station fragments and any '+'.
            var newTitle = sheet.Title;
            var titleMatch = _stationFragmentRgx.Match(newTitle);
            if (titleMatch.Success)
            {
                foreach (Group g in titleMatch.Groups)
                    if (g.Name == "rest") newTitle = newTitle.Replace(g.Value, "");
            }
            newTitle = newTitle.Replace("+", "");

            var numberChanged = newNumber != sheet.Number;
            var titleChanged  = newTitle != sheet.Title;
            var status = (numberChanged || titleChanged) ? RenameStatus.Matched : RenameStatus.NoChange;

            return new RenamePreviewRow(
                sheet.OriginalIndex, sheet.SubsetName,
                sheet.Number, numberChanged ? newNumber : null,
                sheet.Title, titleChanged ? newTitle : null,
                System.Array.Empty<RenamePropertyChange>(),
                status);
        }
    }
}
