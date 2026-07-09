namespace SheetSetManager.SheetManager.Rename.Profiles.Vf
{
    /// <summary>
    /// The naming state a sheet is currently in, as detected by <see cref="VfStateClassifier"/>.
    /// Drives both transition selection and the no-mixed-sets policy.
    /// </summary>
    internal enum VfSheetState
    {
        /// <summary>Sheet does not match any known VF-related state.</summary>
        Unknown,

        /// <summary>
        /// Fresh from the Civil sheet-creation tool. Title carries NR / FST / SST,
        /// number has not yet been remapped, Emnelinje 1+2 not yet set by VF.
        /// </summary>
        CivilNascent,

        /// <summary>
        /// Already in VF naming with all-underscore separators, e.g.
        /// <c>PROG_KOMM_ENERGI_001_001</c>. Title is the literal "LEDNINGSPLAN".
        /// </summary>
        VfAllUnderscores,

        /// <summary>
        /// Already in VF naming with dashes between the first three segments and
        /// underscores for NR/Seq, e.g. <c>PROG-KOMM-ENERGI_001_001</c>. Title
        /// is the literal "LEDNINGSPLAN".
        /// </summary>
        VfDashesExceptLast2,
    }
}
