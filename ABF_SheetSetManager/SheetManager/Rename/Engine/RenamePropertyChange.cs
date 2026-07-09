namespace SheetSetManager.SheetManager.Rename.Engine
{
    /// <summary>
    /// One pending change to a sheet's custom property, computed by a rename profile
    /// and shown to the user before being written.
    /// </summary>
    public sealed record RenamePropertyChange(string Name, string? OldValue, string NewValue);
}
