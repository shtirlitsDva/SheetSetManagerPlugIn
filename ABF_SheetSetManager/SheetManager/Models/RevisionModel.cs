using ACSMCOMPONENTS25Lib;

using CommunityToolkit.Mvvm.ComponentModel;

using SheetSetManager.SheetManager.Interfaces;

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Windows.Media.Protection;

namespace SheetSetManager.SheetManager.Models
{
    public partial class RevisionModel : ObservableObject
    {
        public RevisionId RevId { get; }

        [ObservableProperty] private IProperty _revisionLetter;
        [ObservableProperty] private IProperty _date;
        [ObservableProperty] private IProperty _description;
        [ObservableProperty] private IProperty _approvedBy;
        [ObservableProperty] private IProperty _checkedBy;
        [ObservableProperty] private IProperty _drawnBy;

        private Dictionary<string, IProperty> _propDict = new();
        public IProperty this[string name]
        {
            get => _propDict[name];
            set => _propDict[name] = value;
        }

        public RevisionModel(List<(string Name, AcSmCustomPropertyValue Value)> list)
        {
            if (list.Count == 0) throw new System.Exception("Property list empty!");
            var tokens = list[0].Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length >= 2)
            {
                var enumKey = $"{tokens[0]}{tokens[1]}";        //  "Rev" + "A" →  "RevA"
                if (Enum.TryParse(enumKey, out RevisionId parsed)) RevId = parsed;
            }

            //Assume the list contains properties for this revision
            foreach (var prop in list)
            {
                var p = new PropertyCustomModel(prop);
                p.PropertyChanged += OnInnerValueChanged;
                if (prop.Name.Contains("Dato")) { Date = p; _propDict["Dato"] = p; }
                else if (prop.Name.Contains("Emne")) { Description = p; _propDict["Emne"] = p; }
                else if (prop.Name.Contains("Godkendt")) { ApprovedBy = p; _propDict["Godkendt"] = p; }
                else if (prop.Name.Contains("Kontrol")) { CheckedBy = p; _propDict["Kontrol"] = p; }
                else if (prop.Name.Contains("Tegner")) { DrawnBy = p; _propDict["Tegner"] = p; }
                else { RevisionLetter = p; _propDict["Rev"] = p; }
            }
        }

        private void OnInnerValueChanged(object? _, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IProperty.Value))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }

        //Revision letter must not be checked for validity as it is always present
        public bool IsValid => !string.IsNullOrEmpty(RevisionLetter.Value) ||
                               !string.IsNullOrEmpty(Date.Value) ||
                               !string.IsNullOrEmpty(Description.Value) ||
                               !string.IsNullOrEmpty(ApprovedBy.Value) ||
                               !string.IsNullOrEmpty(CheckedBy.Value) ||
                               !string.IsNullOrEmpty(DrawnBy.Value);

        public enum RevisionId 
        {
            Unknown = 0,
            RevA, 
            RevB,
            RevC,
            RevD,
            RevE,
            RevF,
            RevG,
            RevH
        }
    }
}