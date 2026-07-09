namespace SheetSetManager.SheetManager.Rename.Engine
{
    /// <summary>
    /// Outcome for one sheet after a rename profile has computed its preview.
    /// </summary>
    public enum RenameStatus
    {
        /// <summary>Profile produced a new value differing from the current one.</summary>
        Matched,

        /// <summary>Profile produced values identical to the current ones — nothing to write.</summary>
        NoChange,

        /// <summary>Profile could not derive the required tokens (e.g. regex mismatch).
        /// Apply must be blocked while any row is in this state.</summary>
        Error,
    }
}
