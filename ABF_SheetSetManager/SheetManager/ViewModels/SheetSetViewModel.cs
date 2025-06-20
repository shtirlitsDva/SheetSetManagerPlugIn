using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using SheetSetManager.SheetManager.Interfaces;
using SheetSetManager.SheetManager.Interop;
using SheetSetManager.SheetManager.Managers;
using SheetSetManager.SheetManager.Models;
using SheetSetManager.SheetManager.Views;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using static SheetSetManager.Utils;


namespace SheetSetManager.SheetManager.ViewModels
{
    internal partial class SheetSetViewModel : ObservableObject
    {
        private readonly Interop.SheetSetManager _sheetSetManager = new();

        public SheetsManager Sheets => _sheetSetManager.Sheets;
        [ObservableProperty] private bool _hasPendingChanges;

        public SheetSetViewModel()
        {
            _sheetSetManager.LoadSheets();
            AttachWatchedChanged();
        }

        private void AttachWatchedChanged()
        {
            foreach (var sheet in Sheets)
            {
                foreach (var property in sheet.Properties)
                {
                    property.PropertyChanged += WatchedChanged;
                }

                foreach (var property in sheet.Revisions
                    .AllRevisions.SelectMany(x => x.Properties))
                {
                    property.PropertyChanged += WatchedChanged;
                }

                foreach (var revision in sheet.Revisions)
                {
                    revision.PropertyChanged += WatchedChanged;
                }
            }
        }

        private void WatchedChanged(object? s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IProperty.ChangePending) ||
                e.PropertyName == nameof(RevisionModel.ChangePending))
            {
                RecomputePending();
            }
        }

        private void RecomputePending() =>
            HasPendingChanges =
            Sheets.Any(sheet =>
            sheet.Properties.Any(p => p.ChangePending) ||
            sheet.Revisions.AllRevisions.Any(r => r.ChangePending) ||
            sheet.Revisions.AllRevisions.SelectMany(
                x => x.Properties).Any(p => p.ChangePending));

        [RelayCommand]
        private void ApplyChanges()
        {
            var modifiedSheetProps = Sheets
                .SelectMany(x => x.Properties)
                .Where(x => x.ChangePending);
            var modifiedRevisionsProps = Sheets
                .SelectMany(x => x.Revisions.AllRevisions)
                .SelectMany(x => x.Properties)
                .Where(x => x.ChangePending);
            var sheetsWithModifiedRevisions = Sheets
                .Where(x => x.Revisions.AllRevisions.Any(
                    r => r.ChangePending));                

            var dlg = new ApplyConfirmationWindow();
            if (dlg.ShowDialog() != true) return; //Cancel

            try
            {
                _sheetSetManager.LockDatabase();
                foreach (var prop in modifiedSheetProps) prop.ApplyChange();
                foreach (var prop in modifiedRevisionsProps) prop.ApplyChange();
            }
            catch (Exception ex)
            {
                _sheetSetManager.UnlockDatabase(false);
                throw;
            }
            _sheetSetManager.UnlockDatabase(true);

            //Implement the application of revisions
            List<(string fileName, List<RevisionModel> revisions)> 
                revisionsWithFilenames = new();
            foreach (var sheet in sheetsWithModifiedRevisions)
            {
                var comSheet = sheet.Oid.GetPersistObject() as AcSmSheet;
                if (comSheet == null) continue;
                var comLayout = comSheet.GetLayout() as AcSmAcDbLayoutReference;
                revisionsWithFilenames.Add((comLayout.GetFileName(),
                    sheet.Revisions.AllRevisions.Where(
                        x => x.ChangePending).ToList()));                
            }

            var applications = RevisionApplicationFactory
                .Fabricate(revisionsWithFilenames);

            AcContext.Current.Post(_ =>
            {
                AcOnOffRevisionLayers.Apply(applications);
            }, null);

            ResetSheets();
        }

        [RelayCommand]
        private void ResetSheets()
        {
            _sheetSetManager.LoadSheets();
            AttachWatchedChanged();
            HasPendingChanges = false;
        }

        [RelayCommand]
        private void AddRevisionToSelected()
        {
            if (!Sheets.Where(s => s.IsSelected).Any()) return;

            var dlg = new RevisionDialog();

            if (dlg.ShowDialog() != true) return;

            var template = dlg.Result;

            foreach (var sheet in Sheets.Where(
                s => s.IsSelected))
                sheet.Revisions.AddNextRevision(template);
        }

        [RelayCommand]
        private void RemoveRevisionFromSelected()
        {
            foreach (var sheet in Sheets.Where(s => s.IsSelected))
                sheet.Revisions.RemoveLatestRevision();
        }

        public void BroadcastEdit(object editedItem, string key, string? value)
        {
            if (editedItem is SheetModel sheetRow)
            {
                foreach (var s in Sheets.Where(s => s.IsSelected || s == sheetRow))
                    s.Properties[key].Value = value;
            }
            else if (editedItem is RevisionModel revRow)
            {
                foreach (var s in Sheets.Where(s => s.IsSelected))
                {
                    var query = s.Revisions.FirstOrDefault(
                        r => r.RevId == revRow.RevId);
                    if (query != null) query[key].Value = value;
                }
            }
        }
    }
}