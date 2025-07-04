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

            var dwgGps = fileNamesAndRevisions.GroupBy(x => x.fileName);

            foreach (var dwgGp in dwgGps)
            {
                var revApp = new RevisionApplicationModel(dwgGp.Key);

                var revGps = dwgGp
                    .SelectMany(x => x.revisions)
                    .GroupBy(x => x.RevId);

                foreach (var revGp in revGps)
                {
                    if (revGp.All(x => x.IsValid)) revApp.AddAction(
                        new RevisionApplicationAction(revGp.Key, RevisionAction.On));
                    else if (revGp.All(x => !x.IsValid) &&
                        !revGp.First().Sheet
                            .AllSheetsOnDwg
                            .SelectMany(x => x.Revisions)
                            .Where(x => x.RevId == revGp.Key).Any()) //Second check to see if while some
                            //revisions are removed that there aren't any left that should not be removed
                            //ie. revisions that share the same layer but were not removed
                    {
                        revApp.AddAction(new RevisionApplicationAction(revGp.Key, RevisionAction.Off));
                    }
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