using SheetSetManager.SheetManager.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SheetSetManager.SheetManager.Models
{
    internal static class RevisionApplicationFactory
    {
        internal static List<RevisionApplicationModel> Fabricate(
            List<(string fileName, List<RevisionModel> revisions)> fileNamesAndRevisions)
        {
            List<RevisionApplicationModel> apps = new();

            var gps = fileNamesAndRevisions.GroupBy(x => x.fileName);

            foreach (var gp in gps)
            {
                var revApp = new RevisionApplicationModel(gp.Key);

                var revGps = gp
                    .SelectMany(x => x.revisions)
                    .GroupBy(x => x.RevId);

                foreach (var revGp in revGps)
                {
                    if (revGp.All(x => x.IsValid)) revApp.AddAction(
                        new RevisionApplicationAction(revGp.Key, RevisionAction.On));
                    else if (revGp.All(x => !x.IsValid)) revApp.AddAction(
                        new RevisionApplicationAction(revGp.Key, RevisionAction.Off));
                    //Mixed if On
                    else revApp.AddAction(
                        new RevisionApplicationAction(
                            revGp.Key, RevisionAction.On));
                }

                apps.Add(revApp);
            }

            return apps;
        }
    }
}