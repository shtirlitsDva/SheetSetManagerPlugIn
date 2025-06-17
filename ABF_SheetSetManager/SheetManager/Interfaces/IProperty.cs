using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Interfaces
{
    public interface IProperty : INotifyPropertyChanged
    {
        string Name { get; }
        string? Value { get; set; }
        void ApplyChange();
        bool ChangePending { get; }
    }
}
