using SheetSetManager.SheetManager.Rename.Engine;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SheetSetManager.SheetManager.Rename.Profiles.Vf
{
    /// <summary>
    /// Mechanical rewrite of a sheet's Number between the two VF separator styles.
    /// Reads the existing Number using the source-state regex, rejoins it with the
    /// target separator pattern, and leaves Title and Emnelinje 1+2 untouched —
    /// they were already set correctly by a prior VF rename.
    ///
    /// Does not consume user inputs. Does not advance the engine sequence counter:
    /// the existing Seq segment of the number is preserved verbatim.
    /// </summary>
    internal sealed class VfSeparatorSwapStrategy : IVfRenameStrategy
    {
        private readonly Regex _sourceNumberRgx;
        private readonly string _targetFormat;

        /// <summary>
        /// </summary>
        /// <param name="sourceNumberRgx">Regex capturing P1/P2/P3/NR/Seq from the source state's number shape.</param>
        /// <param name="targetFormat">
        /// Composite format string. Tokens, in order: P1, P2, P3, NR, Seq.
        /// E.g. <c>"{0}_{1}_{2}_{3}_{4}"</c> for AllUnderscores or
        /// <c>"{0}-{1}-{2}_{3}_{4}"</c> for DashesExceptLast2.
        /// </param>
        public VfSeparatorSwapStrategy(Regex sourceNumberRgx, string targetFormat)
        {
            _sourceNumberRgx = sourceNumberRgx;
            _targetFormat = targetFormat;
        }

        public RenamePreviewRow Build(
            RenameSheetData sheet,
            IReadOnlyDictionary<string, string> inputs,
            RenameSequenceState seqState)
        {
            // Classifier already verified the match.
            var m = _sourceNumberRgx.Match(sheet.Number);

            var newNumber = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                _targetFormat,
                m.Groups["P1"].Value,
                m.Groups["P2"].Value,
                m.Groups["P3"].Value,
                m.Groups["NR"].Value,
                m.Groups["Seq"].Value);

            var numberChanged = newNumber != sheet.Number;
            var status = numberChanged ? RenameStatus.Matched : RenameStatus.NoChange;

            return new RenamePreviewRow(
                sheet.OriginalIndex, sheet.SubsetName,
                sheet.Number, numberChanged ? newNumber : null,
                sheet.Title, null,
                Array.Empty<RenamePropertyChange>(),
                status);
        }
    }
}
