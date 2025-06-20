using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Models
{
    internal class RevisionApplicationModel
    {
        private string _dbFileName;
        public string DatabaseFileName => _dbFileName;

        public RevisionApplicationModel(string dbFileName)
        {
            this._dbFileName = dbFileName;
        }

        public readonly List<RevisionApplicationAction> Actions = new();

        public void AddAction(RevisionApplicationAction action) { Actions.Add(action); }
    }
}
