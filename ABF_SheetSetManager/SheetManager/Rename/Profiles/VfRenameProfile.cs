using SheetSetManager.SheetManager.Rename.Engine;
using SheetSetManager.SheetManager.Rename.Profiles.Vf;

using System;
using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Profiles
{
    /// <summary>
    /// VF naming convention. Acts as a thin dispatcher over a state classifier and a
    /// transition table. Per-row work lives in <see cref="IVfRenameStrategy"/> implementations
    /// in the <c>Vf</c> sub-namespace; this class only owns:
    ///  - the user-facing inputs definition,
    ///  - the preflight policy (no mixed source states allowed),
    ///  - dispatch from <c>(detected state, chosen separator) → strategy</c>.
    /// </summary>
    internal sealed class VfRenameProfile : IRenameProfile
    {
        public const string ProfileId = "VF";

        public const string SepAllUnderscores = "AllUnderscores";
        public const string SepDashesExceptLast2 = "DashesExceptLastTwo";

        private readonly VfStateClassifier _classifier = new();
        private readonly VfTransitionTable _transitions;

        // Inputs the panel always *can* show. Preflight narrows this down per sheet-set.
        private static readonly RenameInputField[] _allInputFields = new RenameInputField[]
        {
            new TextRenameInputField("Program", "Program"),
            new TextRenameInputField("Komm",    "VF kommunekode"),
            new TextRenameInputField("Energi",  "Energidistrikt"),
            new ChoiceRenameInputField(
                name: "SeparatorStyle",
                label: "Separator style",
                choices: new[]
                {
                    new RenameChoice(SepAllUnderscores,    "All underscores  (A_B_C_NN_SS)"),
                    new RenameChoice(SepDashesExceptLast2, "Dashes, last two underscores  (A-B-C_NN_SS)"),
                },
                defaultValue: SepAllUnderscores),
        };

        // Subset of the above that excludes Program/Komm/Energi — used when the sheet set
        // is entirely in a VF state and the operation is a pure separator transition.
        private static readonly RenameInputField[] _separatorOnlyFields = new RenameInputField[]
        {
            _allInputFields[3], // SeparatorStyle
        };

        public VfRenameProfile()
        {
            var renderAllUnderscores = new RenameTemplate("{Program}_{Komm}_{Energi}_{NR:D3}_{Seq:D3}");
            var renderDashesExceptLast2 = new RenameTemplate("{Program}-{Komm}-{Energi}_{NR:D3}_{Seq:D3}");

            var civilToAll  = new CivilNascentToVfStrategy(renderAllUnderscores);
            var civilToDash = new CivilNascentToVfStrategy(renderDashesExceptLast2);

            // Composite formats consume the captured groups (P1, P2, P3, NR, Seq) verbatim.
            var swapAllToDash = new VfSeparatorSwapStrategy(
                VfStateClassifier.VfAllUnderscoresNumberRgx,
                "{0}-{1}-{2}_{3}_{4}");
            var swapDashToAll = new VfSeparatorSwapStrategy(
                VfStateClassifier.VfDashesExceptLast2NumberRgx,
                "{0}_{1}_{2}_{3}_{4}");

            var noChange = new VfNoChangeStrategy();

            _transitions = new VfTransitionTable(
                new Dictionary<(VfSheetState, string), IVfRenameStrategy>
                {
                    { (VfSheetState.CivilNascent,        SepAllUnderscores),    civilToAll  },
                    { (VfSheetState.CivilNascent,        SepDashesExceptLast2), civilToDash },
                    { (VfSheetState.VfAllUnderscores,    SepAllUnderscores),    noChange    },
                    { (VfSheetState.VfAllUnderscores,    SepDashesExceptLast2), swapAllToDash },
                    { (VfSheetState.VfDashesExceptLast2, SepDashesExceptLast2), noChange    },
                    { (VfSheetState.VfDashesExceptLast2, SepAllUnderscores),    swapDashToAll },
                });
        }

        public string Id => ProfileId;
        public string DisplayName => "Vestforbrænding (RSSVF)";
        public string Description =>
            "Program / Kommunekode / Energidistrikt — number, title=LEDNINGSPLAN, Emnelinje 1+2. " +
            "Already-renamed VF sheets can be retransitioned between separator styles without re-entering inputs.";

        public IReadOnlyList<RenameInputField> InputFields => _allInputFields;

        public RenameProfilePreflight Preflight(IReadOnlyList<RenameSheetData> sheets)
        {
            if (sheets.Count == 0)
                return new RenameProfilePreflight(_allInputFields, System.Array.Empty<string>());

            // Classify once — keep counts so we can produce a precise error.
            int civil = 0, vfAll = 0, vfDash = 0, unknown = 0;
            foreach (var s in sheets)
            {
                switch (_classifier.Classify(s))
                {
                    case VfSheetState.CivilNascent:        civil++;   break;
                    case VfSheetState.VfAllUnderscores:    vfAll++;   break;
                    case VfSheetState.VfDashesExceptLast2: vfDash++;  break;
                    default:                                unknown++; break;
                }
            }

            if (unknown > 0)
            {
                return new RenameProfilePreflight(
                    System.Array.Empty<RenameInputField>(),
                    new[]
                    {
                        $"{unknown} sheet(s) could not be classified as Civil-nascent or as already-VF. " +
                        "VF refuses to operate on a mixed or unknown set."
                    });
            }

            int statesPresent = (civil > 0 ? 1 : 0) + (vfAll > 0 ? 1 : 0) + (vfDash > 0 ? 1 : 0);
            if (statesPresent > 1)
            {
                return new RenameProfilePreflight(
                    System.Array.Empty<RenameInputField>(),
                    new[]
                    {
                        $"Mixed sheet states detected (Civil={civil}, VfAllUnderscores={vfAll}, VfDashes={vfDash}). " +
                        "VF requires a single source state per run. Resolve to one state and try again."
                    });
            }

            // Single state: Civil needs full inputs; either VF state needs only the SeparatorStyle.
            if (civil > 0)
                return new RenameProfilePreflight(_allInputFields, System.Array.Empty<string>());

            return new RenameProfilePreflight(_separatorOnlyFields, System.Array.Empty<string>());
        }

        public RenamePreviewRow BuildPreview(
            RenameSheetData sheet,
            IReadOnlyDictionary<string, string> inputs,
            RenameSequenceState seqState)
        {
            var state = _classifier.Classify(sheet);
            if (state == VfSheetState.Unknown)
            {
                return new RenamePreviewRow(
                    sheet.OriginalIndex, sheet.SubsetName,
                    sheet.Number, null,
                    sheet.Title, null,
                    System.Array.Empty<RenamePropertyChange>(),
                    RenameStatus.Error,
                    $"Sheet does not match any known VF state " +
                    $"(Title='{sheet.Title}', Number='{sheet.Number}').");
            }

            if (!inputs.TryGetValue("SeparatorStyle", out var target) || string.IsNullOrEmpty(target))
            {
                return new RenamePreviewRow(
                    sheet.OriginalIndex, sheet.SubsetName,
                    sheet.Number, null,
                    sheet.Title, null,
                    System.Array.Empty<RenamePropertyChange>(),
                    RenameStatus.Error,
                    "SeparatorStyle input is missing.");
            }

            var strategy = _transitions.Resolve(state, target);
            if (strategy is null)
            {
                return new RenamePreviewRow(
                    sheet.OriginalIndex, sheet.SubsetName,
                    sheet.Number, null,
                    sheet.Title, null,
                    System.Array.Empty<RenamePropertyChange>(),
                    RenameStatus.Error,
                    $"No transition is registered from {state} to '{target}'.");
            }

            return strategy.Build(sheet, inputs, seqState);
        }
    }
}
