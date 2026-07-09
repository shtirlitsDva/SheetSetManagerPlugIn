using System.Collections.Generic;

namespace SheetSetManager.SheetManager.Rename.Engine
{
    /// <summary>
    /// One row of the preview the user sees before pressing Apply.
    /// Carries everything needed to (a) display the diff and (b) write it back to SSM.
    /// </summary>
    public sealed class RenamePreviewRow
    {
        public RenamePreviewRow(
            int originalIndex,
            string subsetName,
            string oldNumber,
            string? newNumber,
            string oldTitle,
            string? newTitle,
            IReadOnlyList<RenamePropertyChange> propertyChanges,
            RenameStatus status,
            string? errorMessage = null)
        {
            OriginalIndex = originalIndex;
            SubsetName = subsetName;
            OldNumber = oldNumber;
            NewNumber = newNumber;
            OldTitle = oldTitle;
            NewTitle = newTitle;
            PropertyChanges = propertyChanges;
            Status = status;
            ErrorMessage = errorMessage;
        }

        public int OriginalIndex { get; }
        public string SubsetName { get; }
        public string OldNumber { get; }
        /// <summary>New number, or <c>null</c> if the profile leaves the number unchanged.</summary>
        public string? NewNumber { get; }
        public string OldTitle { get; }
        /// <summary>New title, or <c>null</c> if the profile leaves the title unchanged.</summary>
        public string? NewTitle { get; }
        public IReadOnlyList<RenamePropertyChange> PropertyChanges { get; }
        public RenameStatus Status { get; }
        /// <summary>Human-readable explanation when <see cref="Status"/> is <see cref="RenameStatus.Error"/>.</summary>
        public string? ErrorMessage { get; }
    }
}
