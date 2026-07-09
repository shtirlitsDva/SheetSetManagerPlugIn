<goal>
Consolidate `Form_RenameSheets`, `Form_RenameSheetsVF`, `Form_RenameSheetsNS` into ONE WPF
window (`RenameSheetsWindow`) where the user picks a *rename profile*, fills the profile's
inputs, sees a live preview of what will change, and applies. Reuse the SheetManager/
abstractions; do NOT cobble — design with SOLID + SoC.
</goal>

<what-the-three-forms-actually-do>
All three follow the same 4-step shape:
  1. Collect a few short string inputs from the user.
  2. Walk the open SSM database → sheet set → subsets → sheets.
  3. Per sheet derive tokens from either the subset name (OLD) or the sheet title regex (VF/NS),
     plus a global running sequence counter.
  4. Write back: SetNumber, optionally SetTitle, optionally update custom properties
     ("Emnelinje 1", "Emnelinje 2").

What differs (the 3 dimensions that the new abstraction must capture):

  Profile  | Inputs                                | Token source                | Outputs
  ---------|---------------------------------------|-----------------------------|------------------------------------
  OLD      | Project, Etape, SheetType             | Subset name regex `\d{2,3}` | Number only; cleans title (strips
           |                                       |  → PipelineNumber           |  station digits and `+`)
  VF       | Program, VFkommunekode, Energidistrikt| Sheet title regex           | Number; Title="LEDNINGSPLAN";
           |                                       |  (NR, FST, SST)             |  Emnelinje 1, Emnelinje 2
  NS       | Projekt, Etape                        | Sheet title regex           | Number; Title="LEDNINGSPLAN";
           |                                       |  (NR, FST, SST)             |  Emnelinje 1, Emnelinje 2
</what-the-three-forms-actually-do>

<recommendation-on-dynamic-config>
You asked whether to make this fully configurable (JSON/XML/UI-driven profile editor) instead
of hardcoded classes.

My recommendation: NO — at least not yet. Reasons:
  - You said you have not touched these in over a year. YAGNI applies.
  - A JSON-driven profile editor is a separate UI to design, validate and document. That cost
    dwarfs the cost of editing one C# class once a year.
  - The proper extension point is just the `IRenameProfile` interface. If the future ever
    demands runtime config, you drop in one extra class (`JsonRenameProfile`) that loads from
    disk and you ARE configurable — without paying the cost today.

What I AM proposing is a token-template engine inside each profile (e.g.
`"{Project}-{Etape}-{SheetType}{Pipeline}-{Seq:D3}"`). That gives you 90% of the dynamism
for 10% of the effort: the rule changes I have seen in the codebase are all *template*
changes, not *behavior* changes. Open–closed is satisfied — adding a new naming convention
= add one C# class.

If you disagree, say so and I'll spec the JSON-config flavour.
</recommendation-on-dynamic-config>

<proposed-architecture>
New folder: `SheetManager/Rename/` (sits next to existing Managers/Models/Views).

  Rename/
    Profiles/
      IRenameProfile.cs           — contract for one naming convention
      RenameInputField.cs         — definition of one user input (name, label, validator)
      RenameTemplate.cs           — token-substitution template (`{Project}`, `{Seq:D3}`, ...)
      OldRenameProfile.cs         — concrete: replaces RSSOLD logic
      VfRenameProfile.cs          — concrete: replaces RSSVF logic
      NsRenameProfile.cs          — concrete: replaces RSSNS logic
      IRenameProfileProvider.cs   — yields the profiles (one place to register new ones)
      DefaultRenameProfileProvider.cs
    Engine/
      RenameContext.cs            — per-sheet token dictionary (Project, Pipeline, NR, Seq, …)
      RenamePreviewRow.cs         — old/new Number, old/new Title, list<(prop, old, new)>, Status
      RenameStatus.cs             — enum: Matched / Skipped / NoChange / Error
      RenameEngine.cs             — pure: profile + inputs + Sheets → IReadOnlyList<RenamePreviewRow>
      RenameApplier.cs            — takes the preview rows, locks DB, writes through PropertyManager
    ViewModels/
      RenameSheetsViewModel.cs    — selected profile, dynamic input list, preview rows, ApplyCommand
      RenameInputFieldViewModel.cs
    Views/
      RenameSheetsWindow.xaml(.cs)
</proposed-architecture>

<key-design-decisions>
1. *Engine is pure.* `RenameEngine.BuildPreview(profile, inputs, sheets)` is side-effect-free.
   That makes the live preview cheap (re-run on any input change) and unit-testable without
   AutoCAD running.
2. *Apply is the only side-effect path.* `RenameApplier.Apply(previewRows)` reuses the existing
   `PropertyBase.ApplyChange()` / `Interop.SheetSetManager.LockDatabase()` patterns from
   `SheetSetViewModel.ApplyChanges`. One Lock/Unlock pair, try/catch with rollback (matches
   the convention you already established).
3. *Reuse `SheetSetManager.LoadSheets()`* for the SS walk, instead of writing a 4th hand-rolled
   `IAcSmEnumDatabase` loop. This kills the long-standing infinite-loop hazard in the legacy
   commands (see code-smell #1 below) for free.
4. *Profiles are classes, not data.* See recommendation above. `IRenameProfile` interface
   keeps it open–closed: a future `JsonRenameProfile` would just be another implementation.
5. *HandyControl + dark theme*, MVVM via CommunityToolkit.Mvvm — consistent with
   `SheetManagerWindow` so the user sees one coherent app.
6. *Validation* lives on `RenameInputField` (per-field `Func<string,ValidationResult>`) and
   on the profile (cross-field). `CanApply` = no validation errors AND ≥1 row Matched.
</key-design-decisions>

<window-layout-sketch>
+---------------------------------------------------------------+
| Profile: [ Old (RSSOLD) ▼ ]                                   |
+--------------------+------------------------------------------+
| Inputs             | Preview                                   |
|                    |                                           |
| Project:    [____] | # | Old Number | New Number | Old Title …|
| Etape:      [____] | …                                         |
| SheetType:  [____] |                                           |
|                    |                                           |
|                    | Status legend: Matched / Skipped / Error  |
+--------------------+------------------------------------------+
|                                       [ Cancel ]  [ Apply ]   |
+---------------------------------------------------------------+

- Inputs panel is generated from `SelectedProfile.InputFields` via `ItemsControl` +
  `DataTemplate`. Changing profile rebuilds the panel — no hand-coded per-profile XAML.
- DataGrid columns include a Properties column showing "Emnelinje 1: A → B" lines, expanded
  in row details (mirroring `SheetManagerWindow.RowDetailsTemplate`).
- Rows colored by `Status`.
</window-layout-sketch>

<commands-cs-changes>
- New: `[CommandMethod("RenameSheets")]` + `[CommandMethod("RSS")]` → opens
  `RenameSheetsWindow` with no profile preselected.
- Keep existing `RenameSheetsOLD / RSSOLD`, `RenameSheetsVF / RSSVF`, `RenameSheetsNS / RSSNS`
  as thin wrappers that open the window with the matching profile preselected — preserves
  muscle memory and your XML `<command>` documentation entries.
- The hand-rolled `RenameAndRenumberOLD/VF/NS` methods become 3-line wrappers that call
  `RenameEngine` + `RenameApplier` — no dead duplication.
- Old WinForms files (`Form_RenameSheets*.{cs,Designer.cs,resx}`) get DELETED in the same
  PR after I confirm the new window covers all three flows.
</commands-cs-changes>

<code-smells-found-rule-3>
1. *Infinite-loop hazard in all three legacy commands* (e.g. `Commands.cs:143`,
   `Commands.cs:323`, `Commands.cs:482`):

       if (smComponent.GetTypeName() != "AcSmSubset") continue;

   …inside `while (smComponent != null)`, but the `continue` does NOT advance the enumerator.
   The NS variant masks this with a `safetyCounter > 1000` band-aid (`Commands.cs:469`). Root
   cause is loop control, not iteration count. The new engine fixes this by iterating through
   the existing `SheetsManager` which already does correct enumeration.

2. *Silent success message.* All three legacy commands set `customMessage = ""` and then
   `prdDbg(customMessage)` at the end on success — user sees nothing. New engine should
   report `"Renamed N of M sheets, K skipped"` to the editor via `prtDbg`.

3. *Multiline text boxes for single-line fields* (`Form_RenameSheets.Designer.cs:105` etc.) —
   minor UX bug, will go away with the WPF rewrite.

4. *No input validation* in any of the three forms beyond `IsNullOrEmpty`. E.g. project
   numbers / etape codes that contain spaces or dashes will silently produce broken sheet
   numbers. The new `RenameInputField.Validator` slot fixes this per profile.

(removed: separator difference between OLD and VF/NS is a deliberate requirement, not a smell.)
</code-smells-found-rule-3>

<implementation-order>
A. Skeleton:
   1. Add `Rename/` folder + `IRenameProfile`, `RenameInputField`, `RenameTemplate`,
      `RenameContext`, `RenameStatus`, `RenamePreviewRow`.
   2. Implement `RenameEngine` against an in-memory fake SheetsManager so it is unit-
      testable without AutoCAD.

B. Profiles:
   3. Port OLD logic into `OldRenameProfile`.
   4. Port VF logic into `VfRenameProfile`.
   5. Port NS logic into `NsRenameProfile`.
   6. Wire them through `DefaultRenameProfileProvider`.

C. UI:
   7. `RenameSheetsViewModel` + dynamic input field VM.
   8. `RenameSheetsWindow.xaml(.cs)` with HandyControl + dark theme.

D. Apply:
   9. `RenameApplier` reusing `PropertyBase.ApplyChange` and the lock/unlock pattern from
      `SheetSetViewModel.ApplyChanges`.

E. Wire-up + cleanup:
   10. New `[CommandMethod("RenameSheets")]` in `Commands.cs`.
   11. Make legacy commands open the new window with profile preselected.
   12. Delete `Form_RenameSheets*.{cs,Designer.cs,resx}` (3 forms × 3 files = 9 files).

F. Smoke test in AutoCAD against a real .dst before deleting old forms.
</implementation-order>

<decisions-from-review>
1. *All sheets must be renamed — no per-row skip.* If any sheet's title (or subset name)
   does not match the profile's extractor regex, that is an ERROR condition, not a silent
   skip. Consequences for the design:
     - `RenameStatus` enum drops `Skipped`; values become `Matched` / `NoChange` / `Error`.
     - `RenameApplier.CanApply` = (no input validation errors) AND (every preview row is
       Matched or NoChange — i.e. zero Error rows).
     - The preview DataGrid highlights Error rows in red and shows the failure reason
       (e.g. "title 'foo' did not match `(?<NR>\d+)\sST…`") so the user can fix the
       offending sheet's title in SSM and re-preview.
     - This is stricter than the legacy commands, which silently logged
       `"Sheet title X did not match Regex!"` and moved on. That silent skip is the
       behavior change you are asking for.
2. *Persist last-used inputs per profile* via `Properties.Settings`. One settings entry per
   profile, keyed by `IRenameProfile.Id`, value is a serialized
   `Dictionary<inputFieldName,string>`. Loaded when the profile is selected, saved on
   successful Apply. No persistence of failed/cancelled attempts.
3. *No new profiles needed*, BUT the VF profile gets a new input — see below.
</decisions-from-review>

<vf-new-requirement-separator-style>
The VF profile (`VfRenameProfile`) gains one extra `RenameInputField`:

  Field name: `SeparatorStyle`
  Label:      "Separator style"
  Editor:     dropdown (ComboBox) — values:
                - `AllUnderscores`  → `{Program}_{Komm}_{Energi}_{NR:D3}_{Seq:D3}`
                                       (current behavior)
                - `DashesExceptLastTwo` → `{Program}-{Komm}-{Energi}_{NR:D3}_{Seq:D3}`
                                       (new requirement)

This is exactly why the plan uses a token-template engine instead of `string.Format`: the VF
profile holds *two* compiled `RenameTemplate` instances and picks one based on the user's
`SeparatorStyle` selection. Adding a third style later = add one more enum value and one
more template literal in `VfRenameProfile`. No engine change required.

Note: the input-field abstraction needs to support enum/dropdown editors, not only text
boxes. So `RenameInputField` becomes:

  abstract class RenameInputField           — name, label, validator
    TextRenameInputField                    — string, regex/length validator
    ChoiceRenameInputField<TEnum>           — bound enum, dropdown editor

The DataTemplate selector in `RenameSheetsWindow.xaml` picks textbox vs combobox per
field type.
</vf-new-requirement-separator-style>
