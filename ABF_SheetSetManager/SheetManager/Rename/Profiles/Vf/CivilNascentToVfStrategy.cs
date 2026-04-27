using SheetSetManager.SheetManager.Rename.Engine;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace SheetSetManager.SheetManager.Rename.Profiles.Vf
{
    /// <summary>
    /// Civil-source sheet → VF naming, in the chosen target separator. This is the
    /// classic RSSVF behavior: parse NR / FST / SST out of the title, render a new
    /// number from user-supplied Program / Komm / Energi, set Title to "LEDNINGSPLAN",
    /// and write Emnelinje 1+2 from NR / FST / SST.
    /// </summary>
    internal sealed class CivilNascentToVfStrategy : IVfRenameStrategy
    {
        private readonly RenameTemplate _numberTemplate;

        public CivilNascentToVfStrategy(RenameTemplate numberTemplate)
        {
            _numberTemplate = numberTemplate;
        }

        public RenamePreviewRow Build(
            RenameSheetData sheet,
            IReadOnlyDictionary<string, string> inputs,
            RenameSequenceState seqState)
        {
            // Classifier already verified the title matches; no defensive re-check.
            var titleMatch = VfStateClassifier.CivilTitleRgx.Match(sheet.Title);

            var nr  = int.Parse(titleMatch.Groups["NR"].Value, CultureInfo.InvariantCulture);
            var fst = titleMatch.Groups["FST"].Value.Replace("+", "");
            var sst = titleMatch.Groups["SST"].Value.Replace("+", "");
            var seq = seqState.Next();

            var newNumber = _numberTemplate.Render(new Dictionary<string, object?>
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
            var titleChanged  = VfStateClassifier.LiteralVfTitle != sheet.Title;
            var anyChange = numberChanged || titleChanged || changes.Count > 0;
            var status = anyChange ? RenameStatus.Matched : RenameStatus.NoChange;

            return new RenamePreviewRow(
                sheet.OriginalIndex, sheet.SubsetName,
                sheet.Number, numberChanged ? newNumber : null,
                sheet.Title, titleChanged ? VfStateClassifier.LiteralVfTitle : null,
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
