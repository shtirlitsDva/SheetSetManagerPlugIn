using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using ACSMCOMPONENTS24Lib;
using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

using static ABF_SheetSetManager.Utils;
using System.IO;

// Instructions:
// 1) Add references: 
// AcCoreMgd
// AcDbMgd
// AcMgd
// ACSMCOMPONENTS23Lib
// System.Windows.Forms

// 2) Right click Project > Properties > Debug > Start External Program > "C:\Program Files\Autodesk\AutoCAD 2020\acad.exe"

// 3) Create MySSmEventHandler class

// 4) Add Using statements

// 5) Make sure references do not copy local: Select Reference > right click > properties > Copy Local = False

namespace ABF_SheetSetManager
{
    public class ABF_SheetSet
    {
        MySSmEventHandler eventHandler;
        Int32 eventSSMCookie;
        Int32 eventDbCookie;
        Int32 eventSSetCookie;
        IAcSmSheetSetMgr m_sheetSetManager;
        IAcSmDatabase m_sheetSetDatabase;
        IAcSmSheetSet m_sheetSet;

        // Open a Sheet Set 
        //[CommandMethod("ABF_OpenSheetSet")]
        public void OpenSheetSet()
        {
            // User Input: editor equals command line
            // To talk to the user you use the command line, aka the editor
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptStringOptions pso = new PromptStringOptions("\nHello Nadia! \nWhat Sheet Set would you like to open?");
            pso.DefaultValue = @"C:\Users\rhale\Documents\AutoCAD Sheet Sets\Squid666.dst";
            pso.UseDefaultValue = true;
            pso.AllowSpaces = true;
            PromptResult pr = ed.GetString(pso);

            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = default(IAcSmSheetSetMgr);
            sheetSetManager = new AcSmSheetSetMgr();

            // Open a Sheet Set file 
            AcSmDatabase sheetSetDatabase = default(AcSmDatabase);
            //sheetSetDatabase = sheetSetManager.OpenDatabase(@"C:\Users\Robert\Documents\AutoCAD Sheet Sets\Expedia.dst", false);
            sheetSetDatabase = sheetSetManager.OpenDatabase(pr.StringResult, false);

            // Return the namd and description of the sheet set
            MessageBox.Show("Sheet Set Name: " + sheetSetDatabase.GetSheetSet().GetName() + "\nSheet Set Description: " + sheetSetDatabase.GetSheetSet().GetDesc());

            // Close the sheet set 
            sheetSetManager.Close(sheetSetDatabase);
        }

        // Create a new sheet set 
        //[CommandMethod("ABF_CreateSheetSet")]
        public void CreateSheetSet()
        {
            // User Input: editor equals command line
            // To talk to the user you use the command line, aka the editor
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptResult pr = ed.GetFileNameForSave("Hello Nadia, Where would you would like to save the new Sheet Set?");

            PromptStringOptions pso = new PromptStringOptions("\nHello Nadia! \nWhat will be the name of the new Sheet Set?");
            pso.AllowSpaces = true;
            PromptResult prSheetSetName = ed.GetString(pso);

            PromptStringOptions psoDescription = new PromptStringOptions("\nHello Nadia! \nWhat will be the description of the new Sheet Set?");
            psoDescription.AllowSpaces = true;
            PromptResult prSheetSetDescription = ed.GetString(psoDescription);


            //pso.DefaultValue = @"C:\Users\Robert\Documents\AutoCAD Sheet Sets\Expedia.dst";
            //pso.UseDefaultValue = true;

            // works...

            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();

            // Create a new sheet set file 
            //AcSmDatabase sheetSetDatabase = sheetSetManager.CreateDatabase(@"C:\Users\Robert\Documents\AutoCAD Sheet Sets\ExpediaSheetSetDemo.dst", "", true);
            AcSmDatabase sheetSetDatabase = sheetSetManager.CreateDatabase(pr.StringResult, "", true);

            // Get the sheet set from the database 
            AcSmSheetSet sheetSet = sheetSetDatabase.GetSheetSet();

            // Attempt to lock the database
            if (LockDatabase(ref sheetSetDatabase, true) == true)
            {
                // Set the name and description of the sheet set 
                //sheetSet.SetName("ExpediaSheetSetTest");
                sheetSet.SetName(prSheetSetName.StringResult);

                //sheetSet.SetDesc("Aluminum Bronze Fabricator's Sheet Set Object Demo");
                sheetSet.SetDesc(prSheetSetDescription.StringResult);

                // Unlock the database 
                LockDatabase(ref sheetSetDatabase, false);

                // Return the name and description of the sheet set 
                MessageBox.Show("Sheet Set Name: " + sheetSetDatabase.GetSheetSet().GetName() + "\nSheet Set Description: " + sheetSetDatabase.GetSheetSet().GetDesc());
            }
            else
            {
                // Display error message 
                MessageBox.Show("Sheet set could not be opened for write.");
            }

            // Close the sheet set 
            sheetSetManager.Close(sheetSetDatabase);
        }

        // Step through all open sheet sets 
        //[CommandMethod("RenameSheetsDev")]
        //[CommandMethod("RSSDEV")]
        public void StepThroughOpenSheetSetsDev()
        {
            //***********************************************************
            string projectNumber = "1264";
            string etapeNumber = "K02";
            string sheetTypeNumber = "2";
            int currentSheetNumber = 0;
            string currentSheetNumberString = "";
            string currentPipelineNumber = "";
            //***********************************************************
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();
            // Get the first open database 
            IAcSmPersist item = enumDatabase.Next();
            string customMessage = "";
            // If a database is open continue 
            if (item != null)
            {
                // Step through the database enumerator 
                while (item != null)
                {
                    // Append the file name of the open sheet set to the output string 
                    customMessage = customMessage + "\n" + item.GetDatabase().GetFileName();

                    AcSmDatabase ssDb = item.GetDatabase();
                    AcSmSheetSet sSet = ssDb.GetSheetSet();
                    prdDbg(sSet.GetName());

                    //Get sheet enumerator
                    IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
                    IAcSmComponent smComponent = enumSubSet.Next();
                    IAcSmSubset subSet;
                    IAcSmSheet sheet;

                    //Lock database
                    if (LockDatabase(ref ssDb, true) != true) return;

                    while (true)
                    {
                        if (smComponent == null) break;

                        //Always test to see what kind of object you get!
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //prdDbg(smComponent.GetTypeName());
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        if (smComponent.GetTypeName() != "AcSmSubset") continue;
                        subSet = smComponent as AcSmSubset;
                        string currentSubSetName = subSet.GetName();

                        Regex regex = new Regex(@"(?<number>^\d\d)");

                        if (regex.IsMatch(currentSubSetName))
                        {
                            Match match = regex.Match(currentSubSetName);
                            currentPipelineNumber = match.Groups["number"].Value;
                            prdDbg($"Strækning nr: {currentPipelineNumber}");
                        }

                        var enumSheets = subSet.GetSheetEnumerator();
                        smComponent = enumSheets.Next();

                        int idx = 0;

                        while (true)
                        {
                            if (smComponent == null) break;
                            //prdDbg(smComponent.GetTypeName());
                            //prdDbg(smComponent.GetName());
                            if (smComponent.GetTypeName() != "AcSmSheet") continue;

                            sheet = smComponent as AcSmSheet;
                            prdDbg("T: " + sheet.GetTitle());
                            prdDbg("N: " + sheet.GetNumber());
                            //layoutRef = sheet.GetLayout();

                            ////Get the referenced layout
                            //if (idx == 0)
                            //{
                            //    string dbPath = layoutRef.GetFileName();
                            //    db = new Database(false, true);
                            //    db.ReadDwgFile(dbPath, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
                            //    tx = db.TransactionManager.StartTransaction();

                            //}

                            //Build number
                            currentSheetNumber++;
                            currentSheetNumberString = currentSheetNumber.ToString("D3");

                            string sheetNumber = $"{projectNumber}-{etapeNumber}-" +
                                                 $"{sheetTypeNumber}{currentPipelineNumber}-" +
                                                 $"{currentSheetNumberString}";

                            prdDbg("Number: " + sheetNumber);

                            //Build sheet name
                            string currentSheetName = smComponent.GetName();

                            //Clean up rests of stations
                            regex = new Regex(@"\d(?<rest>\.\d\d\d)");
                            if (regex.IsMatch(currentSheetName))
                            {
                                Match match = regex.Match(currentSheetName);
                                foreach (System.Text.RegularExpressions.Group group in match.Groups)
                                    if (group.Name == "rest")
                                        currentSheetName = currentSheetName.Replace(group.Value, "");
                            }

                            regex = new Regex(@"(?<number>^\d+\s)");
                            if (regex.IsMatch(currentSheetName))
                                currentSheetName = regex.Replace(currentSheetName, "");
                            currentSheetName = currentSheetName.Replace("+", "");

                            prdDbg("Name: " + currentSheetName);

                            string curTitle = sheet.GetTitle();

                            curTitle = curTitle.Replace(sheetNumber, "");
                            sheet.SetTitle(curTitle);

                            //Change the number and name of sheet
                            sheet.SetNumber(sheetNumber);
                            sheet.SetTitle(currentSheetName);
                            sheet.SetName(currentSheetName);

                            //prdDbg("Layout name: " + layoutRef.GetName());
                            //prdDbg("File name: " + layoutRef.GetFileName());

                            idx++;
                            smComponent = enumSheets.Next();
                        }
                        //Dispose of database and transaction
                        //tx.Commit();
                        //tx.Dispose();
                        //db.Dispose();

                        //Open the next sheet
                        smComponent = enumSubSet.Next();
                    }

                    //Unlock database
                    LockDatabase(ref ssDb, false);
                    // Get the next open database and increment the counter 
                    item = enumDatabase.Next();
                }
            }
            else
            {
                customMessage = "No sheet sets are currently open.";
            }

            // Display the custom message 
            //MessageBox.Show(customMessage);
            prdDbg(customMessage);
        }

        // Step through all open sheet sets 
        [CommandMethod("RenameSheets")]
        [CommandMethod("RSS")]
        public void renamesheetscallform()
        {
            Form_RenameSheets frs = new Form_RenameSheets();
            frs.ShowDialog();
            if (frs.RenameAndRenumber)
            {
                //Validate
                if (string.IsNullOrEmpty(frs.projectNumber)) return;
                if (string.IsNullOrEmpty(frs.etapeNumber)) return;
                if (string.IsNullOrEmpty(frs.sheetTypeNumber)) return;

                RenameAndRenumber(
                    frs.projectNumber, frs.etapeNumber, frs.sheetTypeNumber);
            }
        }

        public void RenameAndRenumber(
            string projectNumber, string etapeNumber, string sheetTypeNumber)
        {
            //***********************************************************
            //string projectNumber = "1264";
            //string etapeNumber = "K02";
            //string sheetTypeNumber = "2";
            int currentSheetNumber = 0;
            string currentSheetNumberString = "";
            string currentPipelineNumber = "";
            //***********************************************************
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();
            // Get the first open database 
            IAcSmPersist item = enumDatabase.Next();
            string customMessage = "";
            // If a database is open continue 
            if (item != null)
            {
                // Step through the database enumerator 
                while (item != null)
                {
                    // Append the file name of the open sheet set to the output string 
                    prdDbg(item.GetDatabase().GetFileName());

                    AcSmDatabase ssDb = item.GetDatabase();
                    AcSmSheetSet sSet = ssDb.GetSheetSet();
                    prdDbg(sSet.GetName());

                    //Get sheet enumerator
                    IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
                    IAcSmComponent smComponent = enumSubSet.Next();
                    IAcSmSubset subSet;
                    IAcSmSheet sheet;

                    //Lock database
                    if (LockDatabase(ref ssDb, true) != true) return;

                    while (true)
                    {
                        if (smComponent == null) break;

                        //Always test to see what kind of object you get!
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //prdDbg(smComponent.GetTypeName());
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        if (smComponent.GetTypeName() != "AcSmSubset") continue;
                        subSet = smComponent as AcSmSubset;
                        string currentSubSetName = subSet.GetName();

                        Regex regex = new Regex(@"(?<number>\d{2,3}?\s)");

                        if (regex.IsMatch(currentSubSetName))
                        {
                            Match match = regex.Match(currentSubSetName);
                            currentPipelineNumber = match.Groups["number"].Value;
                            currentPipelineNumber = currentPipelineNumber.TrimEnd();
                            currentPipelineNumber = currentPipelineNumber.PadLeft(3, '0');
                            prdDbg($"Strækning nr: {currentPipelineNumber}");
                        }

                        var enumSheets = subSet.GetSheetEnumerator();
                        smComponent = enumSheets.Next();

                        int idx = 0;

                        while (true)
                        {
                            if (smComponent == null) break;
                            if (smComponent.GetTypeName() != "AcSmSheet") continue;

                            sheet = smComponent as AcSmSheet;
                            //layoutRef = sheet.GetLayout();

                            ////Get the referenced layout
                            //if (idx == 0)
                            //{
                            //    string dbPath = layoutRef.GetFileName();
                            //    db = new Database(false, true);
                            //    db.ReadDwgFile(dbPath, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
                            //    tx = db.TransactionManager.StartTransaction();

                            //}

                            //Build number
                            currentSheetNumber++;
                            prdDbg("CurrentSheetNumber: " + currentSheetNumber.ToString());
                            currentSheetNumberString = currentSheetNumber.ToString("D3");

                            string sheetNumber = $"{projectNumber}-{etapeNumber}-" +
                                                 $"{sheetTypeNumber}{currentPipelineNumber}-" +
                                                 $"{currentSheetNumberString}";

                            //Build sheet name
                            string currentSheetTitle = sheet.GetTitle();

                            //Clean up rests of stations
                            regex = new Regex(@"\d(?<rest>\.\d\d\d)");
                            if (regex.IsMatch(currentSheetTitle))
                            {
                                Match match = regex.Match(currentSheetTitle);
                                foreach (System.Text.RegularExpressions.Group group in match.Groups)
                                    if (group.Name == "rest")
                                        currentSheetTitle = currentSheetTitle.Replace(group.Value, "");
                            }

                            //regex = new Regex(@"(?<number>^\d+\s)");
                            //if (regex.IsMatch(currentSheetName))
                            //    currentSheetName = regex.Replace(currentSheetName, "");

                            currentSheetTitle = currentSheetTitle.Replace("+", "");

                            //string curTitle = sheet.GetTitle();

                            //curTitle = curTitle.Replace(sheetNumber, "");
                            //sheet.SetTitle(curTitle);

                            //Change the number and name of sheet
                            sheet.SetNumber(sheetNumber);
                            sheet.SetTitle(currentSheetTitle);
                            //sheet.SetName(currentSheetName);

                            prdDbg("GetNumber: " + sheet.GetNumber());
                            prdDbg("NewNumber: " + sheetNumber);
                            prdDbg("GetName: " + sheet.GetName());
                            prdDbg("GetTitle: " + sheet.GetTitle());
                            prdDbg("NewTitle: " + currentSheetTitle);

                            //prdDbg("Layout name: " + layoutRef.GetName());
                            //prdDbg("File name: " + layoutRef.GetFileName());

                            idx++;
                            smComponent = enumSheets.Next();
                        }
                        //Dispose of database and transaction
                        //tx.Commit();
                        //tx.Dispose();
                        //db.Dispose();

                        //Open the next sheet
                        smComponent = enumSubSet.Next();
                    }

                    //Unlock database
                    LockDatabase(ref ssDb, false);
                    // Get the next open database and increment the counter 
                    item = enumDatabase.Next();
                }
            }
            else
            {
                customMessage = "No sheet sets are currently open.";
            }

            // Display the custom message 
            //MessageBox.Show(customMessage);
            prdDbg(customMessage);
        }

        // Step through all open sheet sets 
        [CommandMethod("RenameOldSheetsToNew")]
        [CommandMethod("ROS")]
        public void renameoldsheetstonew()
        {
            RenameOldAndRenumberOld();
        }

        public void RenameOldAndRenumberOld()
        {
            Regex rgxNum = new Regex(@"(?<projectnumber>\d+)-(?<etapenumber>[\d.]+\D?)-(?<drawingtype>1)(?<pipelinenumber>\d+)-(?<number>\d+)");
            Regex rgxTtl = new Regex(@"(?<pipelinenumber>\d+)\s(?<streetname>[\w\sæøåA-ÆØÅ]+)\sST\s(?<stationrange>\d+\s-\s\d+)");

            //***********************************************************
            //string projectNumber = "1264";
            //string etapeNumber = "K02";
            //string sheetTypeNumber = "2";
            int currentSheetNumber = 0;
            string currentSheetNumberString = "";
            string currentPipelineNumber = "";
            //***********************************************************
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();
            // Get the first open database 
            IAcSmPersist item = enumDatabase.Next();
            string customMessage = "";
            // If a database is open continue 
            if (item != null)
            {
                // Step through the database enumerator 
                while (item != null)
                {
                    // Append the file name of the open sheet set to the output string 
                    prdDbg(item.GetDatabase().GetFileName());

                    AcSmDatabase ssDb = item.GetDatabase();
                    AcSmSheetSet sSet = ssDb.GetSheetSet();
                    prdDbg(sSet.GetName());

                    //Get sheet enumerator
                    IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
                    IAcSmComponent smComponent = enumSubSet.Next();
                    IAcSmSubset subSet;
                    IAcSmSheet sheet;

                    //Lock database
                    if (LockDatabase(ref ssDb, true) != true) return;

                    while (true)
                    {
                        if (smComponent == null) break;

                        //Always test to see what kind of object you get!
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //prdDbg(smComponent.GetTypeName());
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        if (smComponent.GetTypeName() != "AcSmSubset") continue;
                        subSet = smComponent as AcSmSubset;
                        string currentSubSetName = subSet.GetName();

                        Regex regex = new Regex(@"(?<number>\d{2,3})");

                        if (regex.IsMatch(currentSubSetName))
                        {
                            Match match = regex.Match(currentSubSetName);
                            currentPipelineNumber = match.Groups["number"].Value;
                            currentPipelineNumber = currentPipelineNumber.TrimEnd();
                            currentPipelineNumber = currentPipelineNumber.PadLeft(3, '0');
                            //prdDbg($"Strækning nr: {currentPipelineNumber}");
                        }

                        var enumSheets = subSet.GetSheetEnumerator();
                        smComponent = enumSheets.Next();

                        int idx = 0;

                        while (true)
                        {
                            if (smComponent == null) break;
                            if (smComponent.GetTypeName() != "AcSmSheet") continue;

                            sheet = smComponent as AcSmSheet;
                            //layoutRef = sheet.GetLayout();

                            ////Get the referenced layout
                            //if (idx == 0)
                            //{
                            //    string dbPath = layoutRef.GetFileName();
                            //    db = new Database(false, true);
                            //    db.ReadDwgFile(dbPath, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
                            //    tx = db.TransactionManager.StartTransaction();

                            //}

                            //prdDbg("GetNumber: " + sheet.GetNumber());
                            //prdDbg("NewNumber: " + sheetNumber);
                            //prdDbg("GetName: " + sheet.GetName());
                            //prdDbg("GetTitle: " + sheet.GetTitle());
                            //prdDbg("NewTitle: " + currentSheetTitle);

                            //Build number


                            //Build sheet name
                            //string currentSheetTitle = sheet.GetTitle();

                            //Clean up rests of stations
                            //regex = new Regex(@"\d(?<rest>\.\d\d\d)");
                            //if (regex.IsMatch(currentSheetTitle))
                            //{
                            //    Match match = regex.Match(currentSheetTitle);
                            //    foreach (System.Text.RegularExpressions.Group group in match.Groups)
                            //        if (group.Name == "rest")
                            //            currentSheetTitle = currentSheetTitle.Replace(group.Value, "");
                            //}


                            //currentSheetTitle = currentSheetTitle.Replace("+", "");

                            

                            //Change the number and name of sheet
                            //sheet.SetNumber(sheetNumber);
                            //sheet.SetTitle(currentSheetTitle);
                            //sheet.SetName(currentSheetName);

                            string curNumber = sheet.GetNumber();
                            string curTitle = sheet.GetTitle();

                            //prdDbg("\nStrækning: " + currentSubSetName+". Værdier:");

                            Match match = rgxNum.Match(curNumber);
                            string projectnumber = match.Groups["projectnumber"].Value;
                            string etapenumber = match.Groups["etapenumber"].Value;
                            string drawingtype = "0" + match.Groups["drawingtype"].Value;
                            string pipelinenumber = match.Groups["pipelinenumber"].Value;
                            string number = match.Groups["number"].Value;

                            //etapenumber = etapenumber.Replace('.', '-');

                            match = rgxTtl.Match(curTitle);
                            string pipelinenumber2 = match.Groups["pipelinenumber"].Value;
                            string streetname = match.Groups["streetname"].Value;
                            string stationrange = match.Groups["stationrange"].Value;

                            List<string> list = new List<string>()
                            {
                                "Projectnumber " + projectnumber,
                                "Etapenumber " + etapenumber,
                                "Drawingtype " + drawingtype,
                                "Pipelinenumber " + pipelinenumber,
                                "Number " + number,
                                "Pipelinenumber2 " + pipelinenumber2,
                                "Streetname " + streetname,
                                "Stationrange " + stationrange
                            };

                            //prdDbg(string.Join("\n", list));

                            //Make new values
                            string newNumber = $"{projectnumber}_{etapenumber}_{drawingtype}_{pipelinenumber}_{number}";
                            string newTitle = "LEDNINGSPLAN";

                            string newEmneLine2 = $"STRÆKNING {pipelinenumber}";
                            string newEmneLine3 = $"ST {stationrange}";

                            prdDbg(newNumber);
                            prdDbg(newTitle);
                            prdDbg(newEmneLine2);
                            prdDbg(newEmneLine3);

                            if (rgxNum.IsMatch(curNumber) && rgxTtl.IsMatch(curTitle))
                            {
                                sheet.SetNumber(newNumber);
                                sheet.SetTitle(newTitle);

                                var cpb = sheet.GetCustomPropertyBag();
                                var prop = cpb.GetProperty("Emnelinje 1");
                                prop.SetValue(newEmneLine2);

                                prop = cpb.GetProperty("Emnelinje 2");
                                prop.SetValue(newEmneLine3);
                            }

                            idx++;
                            smComponent = enumSheets.Next();
                        }
                        //Dispose of database and transaction
                        //tx.Commit();
                        //tx.Dispose();
                        //db.Dispose();

                        //Open the next sheet
                        smComponent = enumSubSet.Next();
                    }

                    //Unlock database
                    LockDatabase(ref ssDb, false);
                    // Get the next open database and increment the counter 
                    item = enumDatabase.Next();
                }
            }
            else
            {
                customMessage = "No sheet sets are currently open.";
            }

            // Display the custom message 
            //MessageBox.Show(customMessage);
            prdDbg(customMessage);
        }

        // Step through all open sheet sets 
        [CommandMethod("DUMPALLSHEETNAMES")]
        public void dumpallsheetnames()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Number;Station");

            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();
            // Get the first open database 
            IAcSmPersist item = enumDatabase.Next();
            string customMessage = "";
            // If a database is open continue 
            if (item != null)
            {
                // Step through the database enumerator 
                while (item != null)
                {
                    AcSmDatabase ssDb = item.GetDatabase();
                    AcSmSheetSet sSet = ssDb.GetSheetSet();

                    //Get sheet enumerator
                    IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
                    IAcSmComponent smComponent = enumSubSet.Next();
                    IAcSmSubset subSet;
                    IAcSmSheet sheet;

                    //Lock database
                    if (LockDatabase(ref ssDb, true) != true) return;

                    while (true)
                    {
                        if (smComponent == null) break;

                        //Always test to see what kind of object you get!
                        if (smComponent.GetTypeName() != "AcSmSubset") continue;
                        subSet = smComponent as AcSmSubset;

                        var enumSheets = subSet.GetSheetEnumerator();
                        smComponent = enumSheets.Next();

                        while (true)
                        {
                            if (smComponent == null) break;
                            if (smComponent.GetTypeName() != "AcSmSheet") continue;

                            sheet = smComponent as AcSmSheet;

                            sb.AppendLine($"{sheet.GetNumber()};{sheet.GetTitle()}");

                            smComponent = enumSheets.Next();
                        }
                        //Open the next sheet
                        smComponent = enumSubSet.Next();
                    }

                    //Unlock database
                    LockDatabase(ref ssDb, false);
                    // Get the next open database and increment the counter 
                    item = enumDatabase.Next();
                }
            }
            else
            {
                customMessage = "No sheet sets are currently open.";
            }

            // Display the custom message 
            //MessageBox.Show(customMessage);
            prdDbg("Data skrevet til: C:\\Temp\\liste.csv");

            File.WriteAllText(@"C:\Temp\liste.csv", sb.ToString(), Encoding.UTF8);
        }

        // Step through all open sheet sets 
        [CommandMethod("DUMPALLNUMBERSANDSTATIONS")]
        public void dumpallnumbersandstations()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Number;Station");

            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();
            // Get the first open database 
            IAcSmPersist item = enumDatabase.Next();
            string customMessage = "";
            // If a database is open continue 
            if (item != null)
            {
                // Step through the database enumerator 
                while (item != null)
                {
                    AcSmDatabase ssDb = item.GetDatabase();
                    AcSmSheetSet sSet = ssDb.GetSheetSet();

                    //Get sheet enumerator
                    IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
                    IAcSmComponent smComponent = enumSubSet.Next();
                    IAcSmSubset subSet;
                    IAcSmSheet sheet;

                    //Lock database
                    if (LockDatabase(ref ssDb, true) != true) return;

                    while (true)
                    {
                        if (smComponent == null) break;

                        //Always test to see what kind of object you get!
                        if (smComponent.GetTypeName() != "AcSmSubset") continue;
                        subSet = smComponent as AcSmSubset;

                        var enumSheets = subSet.GetSheetEnumerator();
                        smComponent = enumSheets.Next();

                        while (true)
                        {
                            if (smComponent == null) break;
                            if (smComponent.GetTypeName() != "AcSmSheet") continue;

                            sheet = smComponent as AcSmSheet;

                            var cpb = sheet.GetCustomPropertyBag();
                            var prop = cpb.GetProperty("Emnelinje 2");

                            sb.AppendLine($"{sheet.GetNumber()};{prop.GetValue()}");

                            smComponent = enumSheets.Next();
                        }
                        //Open the next sheet
                        smComponent = enumSubSet.Next();
                    }

                    //Unlock database
                    LockDatabase(ref ssDb, false);
                    // Get the next open database and increment the counter 
                    item = enumDatabase.Next();
                }
            }
            else
            {
                customMessage = "No sheet sets are currently open.";
            }

            // Display the custom message 
            //MessageBox.Show(customMessage);
            prdDbg(customMessage);

            File.WriteAllText(@"C:\Temp\liste.csv", sb.ToString(), Encoding.UTF8);
        }

        // Step through all open sheet sets 
        //[CommandMethod("removerevisionfromname")]
        public void removerevisionfromname()
        {
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();
            // Get the first open database 
            IAcSmPersist item = enumDatabase.Next();
            string customMessage = "";
            // If a database is open continue 
            if (item != null)
            {
                // Step through the database enumerator 
                while (item != null)
                {
                    // Append the file name of the open sheet set to the output string 
                    customMessage = customMessage + "\n" + item.GetDatabase().GetFileName();

                    AcSmDatabase ssDb = item.GetDatabase();
                    AcSmSheetSet sSet = ssDb.GetSheetSet();
                    prdDbg(sSet.GetName());

                    //Get sheet enumerator
                    IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
                    IAcSmComponent smComponent = enumSubSet.Next();
                    IAcSmSubset subSet;
                    IAcSmSubset2 subSet2;
                    IAcSmSheet sheet;
                    IAcSmSheet2 sheet2;

                    //Lock database
                    if (LockDatabase(ref ssDb, true) != true) return;

                    while (true)
                    {
                        if (smComponent == null) break;

                        //Always test to see what kind of object you get!
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //prdDbg(smComponent.GetTypeName());
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        if (smComponent.GetTypeName() != "AcSmSubset") continue;
                        subSet = smComponent as AcSmSubset;
                        string currentSubSetName = subSet.GetName();

                        var enumSheets = subSet.GetSheetEnumerator();
                        smComponent = enumSheets.Next();

                        int idx = 0;

                        while (true)
                        {
                            if (smComponent == null) break;
                            //prdDbg(smComponent.GetTypeName());
                            //prdDbg(smComponent.GetName());
                            if (smComponent.GetTypeName() != "AcSmSheet") continue;

                            sheet = smComponent as AcSmSheet;
                            //prdDbg("T: " + sheet.GetTitle());
                            prdDbg("N: " + sheet.GetNumber());
                            //layoutRef = sheet.GetLayout();

                            ////Get the referenced layout
                            //if (idx == 0)
                            //{
                            //    string dbPath = layoutRef.GetFileName();
                            //    db = new Database(false, true);
                            //    db.ReadDwgFile(dbPath, FileOpenMode.OpenForReadAndWriteNoShare, true, "");
                            //    tx = db.TransactionManager.StartTransaction();

                            //}

                            //Build number

                            string number = sheet.GetNumber();
                            number = number.Remove(number.Length - 3);
                            prdDbg("New N: " + number);

                            //string sheetNumber = $"{projectNumber}-{etapeNumber}-" +
                            //                     $"{sheetTypeNumber}{currentPipelineNumber}-" +
                            //                     $"{currentSheetNumberString}-00";

                            //prdDbg("Number: " + sheetNumber);

                            ////Build sheet name

                            ////Change the number
                            sheet.SetNumber(number);

                            //prdDbg("Layout name: " + layoutRef.GetName());
                            //prdDbg("File name: " + layoutRef.GetFileName());


                            idx++;
                            smComponent = enumSheets.Next();
                        }
                        //Dispose of database and transaction
                        //tx.Commit();
                        //tx.Dispose();
                        //db.Dispose();

                        //Open the next sheet
                        smComponent = enumSubSet.Next();
                    }

                    //Unlock database
                    LockDatabase(ref ssDb, false);
                    // Get the next open database and increment the counter 
                    item = enumDatabase.Next();
                }
            }
            else
            {
                customMessage = "No sheet sets are currently open.";
            }

            // Display the custom message 
            //MessageBox.Show(customMessage);
            prdDbg(customMessage);
        }

        // Step through all open sheet sets 
        [CommandMethod("DeleteSheets")]
        public void DeleteAllSheets()
        {
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();
            // Get the first open database 
            IAcSmPersist item = enumDatabase.Next();
            string customMessage = "";
            // If a database is open continue 
            if (item != null)
            {
                // Step through the database enumerator 
                while (item != null)
                {
                    // Append the file name of the open sheet set to the output string 
                    prdDbg(item.GetDatabase().GetFileName());

                    AcSmDatabase ssDb = item.GetDatabase();
                    AcSmSheetSet sSet = ssDb.GetSheetSet();
                    prdDbg(sSet.GetName());

                    //Get sheet enumerator
                    IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
                    IAcSmComponent smComponent = enumSubSet.Next();
                    IAcSmSubset subSet;
                    IAcSmSubset2 subSet2;
                    IAcSmSheet sheet;
                    IAcSmSheet2 sheet2;
                    IAcSmAcDbLayoutReference layoutRef;

                    //Lock database
                    if (LockDatabase(ref ssDb, true) != true) return;

                    int i = 0;
                    while (true)
                    {
                        i++;
                        if (smComponent == null) break;

                        //Always test to see what kind of object you get!
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //prdDbg(smComponent.GetTypeName());
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        if (smComponent.GetTypeName() != "AcSmSubset") continue;
                        subSet = smComponent as AcSmSubset;
                        string currentSubSetName = subSet.GetName();

                        var enumSheets = subSet.GetSheetEnumerator();
                        smComponent = enumSheets.Next();

                        while (true)
                        {
                            if (smComponent == null) break;
                            //prdDbg(smComponent.GetTypeName());
                            //prdDbg(smComponent.GetName());
                            if (smComponent.GetTypeName() != "AcSmSheet") continue;

                            sheet = smComponent as AcSmSheet;
                            prdDbg("T: " + sheet.GetTitle());
                            prdDbg("N: " + sheet.GetNumber());
                            layoutRef = sheet.GetLayout();

                            subSet.RemoveSheet((AcSmSheet)sheet);
                            smComponent = enumSheets.Next();
                        }

                        sSet.RemoveSubset(subSet);

                        //Open the next sheet
                        smComponent = enumSubSet.Next();

                        if (i == 1000) break;
                    }

                    //Unlock database
                    LockDatabase(ref ssDb, false);
                    // Get the next open database and increment the counter 
                    item = enumDatabase.Next();
                }
            }
            else
            {
                customMessage = "No sheet sets are currently open.";
            }

            // Display the custom message 
            //MessageBox.Show(customMessage);
            prdDbg(customMessage);
        }

        [CommandMethod("MODIFYCUSTOMPROPERTIES")]
        [CommandMethod("MODPROPS")]
        public void modifycustomproperties()
        {
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();

            #region Safeguarding for mulitple open databases
            //Safeguarding for multiple open databases
            int dbCount = 0;
            IAcSmPersist item = enumDatabase.Next();
            while (item != null)
            {
                dbCount++;
                item = enumDatabase.Next();
            }
            if (dbCount > 1)
            {
                prdDbg("Multiple databases open! Only one database must be open (.dst file)!");
                return;
            }
            if (dbCount < 1)
            {
                prdDbg("No database is open! Open one and only one database (.dst file)!");
                return;
            }
            #endregion

            #region Get custom properties bag
            enumDatabase.Reset();
            item = enumDatabase.Next();
            AcSmDatabase ssDb = item.GetDatabase();
            AcSmSheetSet sSet = ssDb.GetSheetSet();

            if (LockDatabase(ref ssDb, true) != true) return;

            AcSmCustomPropertyBag cpb = sSet.GetCustomPropertyBag();
            IAcSmEnumProperty propEnum = cpb.GetPropertyEnumerator();
            List<string> propList = new List<string>() { "Cancel" };
            string propName = default;
            AcSmCustomPropertyValue value = default;
            propEnum.Next(out propName, out value);
            while (!string.IsNullOrEmpty(propName))
            {
                propList.Add(propName);
                propEnum.Next(out propName, out value);
            }
            #endregion

            var form = new Form_ModifyCustomProperties(propList);
            form.ShowDialog();

            LockDatabase(ref ssDb, false);

            //MessageBox.Show(string.Join(Environment.NewLine, form.PropsAndValues.Select(x => x.Key + " -> " + x.Value)));

            correctallcustomproperties(form.PropsAndValues);
        }
        private void correctallcustomproperties(Dictionary<string, string> properties)
        {
            if (properties.Any(x => x.Key == "Cancel")) { prdDbg("Cancelled!"); return; }

            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();
            // Get the first open database 
            IAcSmPersist item = enumDatabase.Next();
            string customMessage = "";
            // If a database is open continue 
            if (item != null)
            {
                // Step through the database enumerator 
                while (item != null)
                {
                    // Append the file name of the open sheet set to the output string 
                    customMessage = customMessage + "\n" + item.GetDatabase().GetFileName();

                    AcSmDatabase ssDb = item.GetDatabase();
                    AcSmSheetSet sSet = ssDb.GetSheetSet();
                    prdDbg(sSet.GetName());

                    //Get sheet enumerator
                    IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
                    IAcSmComponent smComponent = enumSubSet.Next();
                    IAcSmSubset subSet;
                    IAcSmSubset2 subSet2;
                    IAcSmSheet sheet;
                    IAcSmSheet2 sheet2;

                    //Lock database
                    if (LockDatabase(ref ssDb, true) != true) return;

                    while (true)
                    {
                        if (smComponent == null) break;

                        //Always test to see what kind of object you get!
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        //prdDbg(smComponent.GetTypeName());
                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        if (smComponent.GetTypeName() != "AcSmSubset") continue;
                        subSet = smComponent as AcSmSubset;

                        var enumSheets = subSet.GetSheetEnumerator();
                        smComponent = enumSheets.Next();

                        while (true)
                        {
                            if (smComponent == null) break;
                            //prdDbg(smComponent.GetTypeName());
                            //prdDbg(smComponent.GetName());
                            if (smComponent.GetTypeName() != "AcSmSheet") continue;

                            sheet = smComponent as AcSmSheet;

                            #region Change custom property

                            foreach (var entry in properties)
                            {
                                AcSmCustomPropertyBag cpb = sheet.GetCustomPropertyBag();
                                AcSmCustomPropertyValue property = cpb.GetProperty(entry.Key);
                                property.SetValue(entry.Value);
                            }
                            #endregion

                            smComponent = enumSheets.Next();
                        }
                        //Open the next sheet
                        smComponent = enumSubSet.Next();
                    }

                    //Unlock database
                    LockDatabase(ref ssDb, false);
                    // Get the next open database and increment the counter 
                    item = enumDatabase.Next();
                }
            }
            else
            {
                customMessage = "No sheet sets are currently open.";
            }

            // Display the custom message 
            //MessageBox.Show(customMessage);
            prdDbg(customMessage);
        }

        [CommandMethod("MANAGEREVISIONSONSHEETS")]
        public void managerevisionsonsheets()
        {
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();
            // Get the loaded databases 
            IAcSmEnumDatabase enumDatabase = sheetSetManager.GetDatabaseEnumerator();

            #region Safeguarding for mulitple open databases
            //Safeguarding for multiple open databases
            int dbCount = 0;
            IAcSmPersist item = enumDatabase.Next();
            while (item != null)
            {
                dbCount++;
                item = enumDatabase.Next();
            }
            if (dbCount > 1)
            {
                prdDbg("Multiple databases open! Only one database must be open (.dst file)!");
                return;
            }
            if (dbCount < 1)
            {
                prdDbg("No database is open! Open one and only one database (.dst file)!");
            }
            #endregion

            #region Gather data
            enumDatabase.Reset();
            item = enumDatabase.Next();
            AcSmDatabase ssDb = item.GetDatabase();
            AcSmSheetSet sSet = ssDb.GetSheetSet();

            if (LockDatabase(ref ssDb, true) != true) return;

            //Get sheet enumerator
            IAcSmEnumComponent enumSubSet = sSet.GetSheetEnumerator();
            IAcSmComponent smComponent = enumSubSet.Next();
            IAcSmSubset subSet;
            IAcSmSheet sheet;

            while (smComponent != null)
            {

            }


            #endregion



            //MessageBox.Show(string.Join(Environment.NewLine, form.PropsAndValues.Select(x => x.Key + " -> " + x.Value)));


        }

        // Create a new sheet set with custom subsets
        //[CommandMethod("ABF_CreateSheetSetWithSubsets")]
        public void CreateSheetSet_WithSubset()
        {
            // User Input: editor equals command line
            // To talk to the user you use the command line, aka the editor
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptResult pr = ed.GetFileNameForSave("Hello Nadia, Where would you would like to save the new Sheet Set?");

            PromptStringOptions pso = new PromptStringOptions("\nHello Nadia! \nWhat will be the name of the new Sheet Set?");
            pso.AllowSpaces = true;
            PromptResult prSheetSetName = ed.GetString(pso);

            PromptStringOptions psoDescription = new PromptStringOptions("\nHello Nadia! \nWhat will be the description of the new Sheet Set?");
            psoDescription.AllowSpaces = true;
            PromptResult prSheetSetDescription = ed.GetString(psoDescription);

            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();

            // Create a new sheet set file 
            //AcSmDatabase sheetSetDatabase = sheetSetManager.CreateDatabase("C:\\Datasets\\CP318-4\\CP318-4.dst", "", true);
            AcSmDatabase sheetSetDatabase = sheetSetManager.CreateDatabase(pr.StringResult, "", true);

            // Get the sheet set from the database 
            AcSmSheetSet sheetSet = sheetSetDatabase.GetSheetSet();

            // Attempt to lock the database 
            if (LockDatabase(ref sheetSetDatabase, true) == true)
            {
                // Set the name and description of the sheet set 
                sheetSet.SetName(prSheetSetName.StringResult);
                sheetSet.SetDesc(prSheetSetDescription.StringResult);

                // Create two new subsets 
                AcSmSubset subset = default(AcSmSubset);
                //subset = CreateSubset(sheetSetDatabase, "Plans", "Building Plans", "", "C:\\Datasets\\CP318-4\\CP318-4.dwt", "Sheet", false);
                subset = CreateSubset(sheetSetDatabase, "Submittals", "Project Submittals", "", @"C:\Users\Robert\Documents\AutoCAD Sheet Sets\ABFStylesSS.dwt", "Sheet", false);

                //subset = CreateSubset(sheetSetDatabase, "Elevations", "Building Elevations", "", "C:\\Datasets\\CP318-4\\CP318-4.dwt", "Sheet", true);
                subset = CreateSubset(sheetSetDatabase, "As Builts", "Project As Builts", "", @"C:\Users\Robert\Documents\AutoCAD Sheet Sets\ABFStylesSS.dwt", "Sheet", true);

                // Unlock the database 
                LockDatabase(ref sheetSetDatabase, false);
            }
            else
            {
                // Display error message 
                MessageBox.Show("Sheet set could not be opened for write.");
            }

            // Close the sheet set 
            sheetSetManager.Close(sheetSetDatabase);
        }

        // Create a new sheet set 
        //[CommandMethod("ABF_SheetSet_AddCustomProperty")]
        public void CreateSheetSet_AddCustomProperty()
        {
            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();

            // Create a new sheet set file 
            //AcSmDatabase sheetSetDatabase = sheetSetManager.CreateDatabase("C:\\Datasets\\CP318-4\\CP318-4.dst", "", true);
            //AcSmDatabase sheetSetDatabase = sheetSetManager.CreateDatabase("C:\\Datasets\\CP318-4\\CP318-4.dst", "", true);
            //AcSmDatabase sheetSetDatabase = sheetSetManager.OpenDatabase(@"C:\Users\rhale\Documents\AutoCAD Sheet Sets\Expedia.dst", true);           
            //AcSmDatabase sheetSetDatabase = sheetSetManager.(@"C:\Users\rhale\Documents\AutoCAD Sheet Sets\Expedia.dst", true);

            // Open a Sheet Set file 
            AcSmDatabase sheetSetDatabase = default(AcSmDatabase);
            //sheetSetDatabase = sheetSetManager.OpenDatabase("C:\\Program Files\\AutoCAD 2010\\Sample\\Sheet Sets\\Architectural\\IRD Addition.dst", false); 
            sheetSetDatabase = sheetSetManager.OpenDatabase(@"C:\Users\rhale\Documents\AutoCAD Sheet Sets\Expedia.dst", false);

            // Get the sheet set from the database 
            AcSmSheetSet sheetSet = sheetSetDatabase.GetSheetSet();

            // Attempt to lock the database 
            if (LockDatabase(ref sheetSetDatabase, true) == true)
            {
                // Get the folder the sheet set is stored in 
                string sheetSetFolder = null;
                sheetSetFolder = sheetSetDatabase.GetFileName().Substring(0, sheetSetDatabase.GetFileName().LastIndexOf("\\"));

                // Set the default values of the sheet set 
                SetSheetSetDefaults(sheetSetDatabase, "My Expedia Sheet Set", "A&B Fabricators Sheet Set Object Demo", sheetSetFolder, @"C:\Users\rhale\Documents\AutoCAD Sheet Sets\ABFStylesSS.dwt", "Sheet");

                // Create a sheet set property 
                SetCustomProperty(sheetSet, "Project Approved By", "Erin Tasche", PropertyFlags.CUSTOM_SHEETSET_PROP);

                // Create sheet properties 
                SetCustomProperty(sheetSet, "Checked By", "NK", PropertyFlags.CUSTOM_SHEET_PROP);

                SetCustomProperty(sheetSet, "Complete Percentage", "90%", PropertyFlags.CUSTOM_SHEET_PROP);

                SetCustomProperty(sheetSet, "Version #", "D", PropertyFlags.CUSTOM_SHEET_PROP);

                SetCustomProperty(sheetSet, "Version Issue Date", "08/29/19", PropertyFlags.CUSTOM_SHEET_PROP);

                //AddSheet(sheetSetDatabase, "Title Page", "Project Title Page", "Title Page", "T1"); 

                // Create two new subsets 
                AcSmSubset subset = default(AcSmSubset);
                subset = CreateSubset(sheetSetDatabase, "Plans", "Building Plans", "", @"C:\Users\rhale\Documents\AutoCAD Sheet Sets\ABFStylesSS.dwt", "Sheet", false);

                //AddSheet(subset, "North Plan", "Northern section of building plan", "North Plan", "P1"); 

                subset = CreateSubset(sheetSetDatabase, "Elevations", "Building Elevations", "", @"C:\Users\rhale\Documents\AutoCAD Sheet Sets\ABFStylesSS.dwt", "Sheet", true);

                // Sync the properties of the sheet set with the sheets and subsets 
                SyncProperties(sheetSetDatabase);

                // Unlock the database 
                LockDatabase(ref sheetSetDatabase, false);
            }
            else
            {
                // Display error message 
                MessageBox.Show("Sheet set could not be opened for write.");
            }

            // Close the sheet set 
            sheetSetManager.Close(sheetSetDatabase);
        }

        // Create a new sheet set 
        //[CommandMethod("ABF_SyncProperties")]
        public void SyncProperties()
        {
            // User Input: editor equals command line
            // To talk to the user you use the command line, aka the editor
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptResult pr1 = ed.GetFileNameForOpen("Hello Nadia, Please select the Sheet Set you would like to edit.");

            PromptStringOptions pso = new PromptStringOptions("\nHello Nadia! \nWhat version # is the drawing at?");
            //pso.DefaultValue = @"C:\Users\Robert\Documents\AutoCAD Sheet Sets\Expedia.dst";
            //pso.UseDefaultValue = true;
            pso.AllowSpaces = true;
            PromptResult pr = ed.GetString(pso);
            string versionNumber = pr.StringResult;

            pso = new PromptStringOptions("\nHello Nadia! \nWhat is the version issue date?");
            pr = ed.GetString(pso);
            string versionIssueDate = pr.StringResult;

            // Get a reference to the Sheet Set Manager object 
            IAcSmSheetSetMgr sheetSetManager = new AcSmSheetSetMgr();

            // Create a new sheet set file 
            //AcSmDatabase sheetSetDatabase = sheetSetManager.CreateDatabase("C:\\Datasets\\CP318-4\\CP318-4.dst", "", true); 
            //AcSmDatabase sheetSetDatabase = sheetSetManager.CreateDatabase(@"C:\Users\rhale\Documents\AutoCAD Sheet Sets\Expedia.dst", "", true);

            // Open a Sheet Set file 
            AcSmDatabase sheetSetDatabase = default(AcSmDatabase);
            //sheetSetDatabase = sheetSetManager.OpenDatabase("C:\\Program Files\\AutoCAD 2010\\Sample\\Sheet Sets\\Architectural\\IRD Addition.dst", false); 
            //sheetSetDatabase = sheetSetManager.OpenDatabase(@"C:\Users\rhale\Documents\AutoCAD Sheet Sets\Expedia.dst", false);
            sheetSetDatabase = sheetSetManager.OpenDatabase(pr1.StringResult, false);

            // Get the sheet set from the database 
            AcSmSheetSet sheetSet = sheetSetDatabase.GetSheetSet();

            // Attempt to lock the database 
            if (LockDatabase(ref sheetSetDatabase, true) == true)
            {
                // Get the folder the sheet set is stored in 
                string sheetSetFolder = null;
                sheetSetFolder = sheetSetDatabase.GetFileName().Substring(0, sheetSetDatabase.GetFileName().LastIndexOf("\\"));

                // Set the default values of the sheet set 
                //SetSheetSetDefaults(sheetSetDatabase, "CP318-4", "AU2009 Sheet Set Object Demo", sheetSetFolder, "C:\\Datasets\\CP318-4\\CP318-4.dwt", "Sheet"); 
                SetSheetSetDefaults(sheetSetDatabase, "Expedia Sheet Set", "ABF Sheet Set Object Demo", sheetSetFolder, @"C:\Users\rhale\Documents\AutoCAD Sheet Sets\ABFStylesSS.dwt", "Sheet");

                // Create a sheet set property 
                SetCustomProperty(sheetSet, "Project Approved By", "Erin Tasche", PropertyFlags.CUSTOM_SHEETSET_PROP);

                // Create sheet properties 
                //SetCustomProperty(sheetSet, "Checked By", "LAA", PropertyFlags.CUSTOM_SHEET_PROP);

                //SetCustomProperty(sheetSet, "Complete Percentage", "0%", PropertyFlags.CUSTOM_SHEET_PROP);

                SetCustomProperty(sheetSet, "Version #", versionNumber, PropertyFlags.CUSTOM_SHEET_PROP);

                SetCustomProperty(sheetSet, "Version Issue Date", versionIssueDate, PropertyFlags.CUSTOM_SHEET_PROP);


                //AddSheet(sheetSetDatabase, "Title Page", "Project Title Page", "Title Page", "T1"); 

                // Create two new subsets 
                AcSmSubset subset = default(AcSmSubset);
                //subset = CreateSubset(sheetSetDatabase, "Plans", "Building Plans", "", "C:\\Datasets\\CP318-4\\CP318-4.dwt", "Sheet", false);
                subset = CreateSubset(sheetSetDatabase, "Submittals", "Project Submittals", "", @"C:\Users\Robert\Documents\AutoCAD Sheet Sets\ABFStylesSS.dwt", "Sheet", false);

                //subset = CreateSubset(sheetSetDatabase, "Elevations", "Building Elevations", "", "C:\\Datasets\\CP318-4\\CP318-4.dwt", "Sheet", true);
                subset = CreateSubset(sheetSetDatabase, "As Builts", "Project As Builts", "", @"C:\Users\Robert\Documents\AutoCAD Sheet Sets\ABFStylesSS.dwt", "Sheet", true);

                // Add a sheet property 
                //SetCustomProperty(sheetSet, "Drafted By", "KMA", PropertyFlags.CUSTOM_SHEET_PROP);

                // Add a subset property 
                SetCustomProperty(sheetSet, "Count", "0", PropertyFlags.CUSTOM_SHEETSET_PROP);

                // Sync the properties of the sheet set with the sheets and subsets 
                SyncProperties(sheetSetDatabase);

                // Unlock the database 
                LockDatabase(ref sheetSetDatabase, false);
            }
            else
            {
                // Display error message 
                MessageBox.Show("Sheet set could not be opened for write.");
            }

            // Close the sheet set 
            sheetSetManager.Close(sheetSetDatabase);
        }

        // Helper Methods ////////////////////////////////////////////////////////////////////////////////////////

        // Used to lock/unlock a sheet set database 
        private bool LockDatabase(ref AcSmDatabase database, bool lockFlag)
        {
            bool dbLock = false;
            // If lockFalg equals True then attempt to lock the database, otherwise 
            // attempt to unlock it. 
            if (lockFlag == true & database.GetLockStatus() == AcSmLockStatus.AcSmLockStatus_UnLocked)
            {
                database.LockDb(database);
                dbLock = true;
            }
            else if (lockFlag == false && database.GetLockStatus() == AcSmLockStatus.AcSmLockStatus_Locked_Local)
            {
                database.UnlockDb(database, true);
                dbLock = true;
            }
            else
            {
                dbLock = false;
            }

            return dbLock;
        }

        private AcSmSubset CreateSubset(AcSmDatabase sheetSetDatabase, string name, string description, string newSheetLocation,
                                string newSheetDWTLocation, string newSheetDWTLayout, bool promptForDWT)
        {
            // Create a subset with the provided name and description 
            AcSmSubset subset = (AcSmSubset)sheetSetDatabase.GetSheetSet().CreateSubset(name, description);

            // Get the folder the sheet set is stored in 
            string sheetSetFolder = sheetSetDatabase.GetFileName().Substring(0, sheetSetDatabase.GetFileName().LastIndexOf("\\"));

            // Create a reference to a File Reference object 
            IAcSmFileReference fileReference = subset.GetNewSheetLocation();

            // Check to see if a path was provided, if not default 
            // to the location of the sheet set 
            if (!string.IsNullOrEmpty(newSheetLocation))
            {
                fileReference.SetFileName(newSheetLocation);
            }
            else
            {
                fileReference.SetFileName(sheetSetFolder);
            }

            // Set the location for new sheets added to the subset 
            subset.SetNewSheetLocation(fileReference);

            // Create a reference to a Layout Reference object 
            AcSmAcDbLayoutReference layoutReference = default(AcSmAcDbLayoutReference);
            layoutReference = subset.GetDefDwtLayout();

            // Check to see that a default DWT location and name was provided 
            if (!string.IsNullOrEmpty(newSheetDWTLocation))
            {
                // Set the template location and name of the layout 
                //for the Layout Reference object 
                layoutReference.SetFileName(newSheetDWTLocation);
                layoutReference.SetName(newSheetDWTLayout);

                // Set the Layout Reference for the subset 
                subset.SetDefDwtLayout(layoutReference);
            }

            // Set the Prompt for Template option of the subset 
            subset.SetPromptForDwt(promptForDWT);
            return subset;
        }

        // Set/create a custom sheet or sheet set property 
        private void SetCustomProperty(IAcSmPersist owner, string propertyName, object propertyValue, PropertyFlags sheetSetFlag)
        {
            // Create a reference to the Custom Property Bag
            AcSmCustomPropertyBag customPropertyBag = default(AcSmCustomPropertyBag);

            if (owner.GetTypeName() == "AcSmSheet")
            {
                AcSmSheet sheet = (AcSmSheet)owner;
                customPropertyBag = sheet.GetCustomPropertyBag();
            }
            else
            {
                AcSmSheetSet sheetSet = (AcSmSheetSet)owner;
                customPropertyBag = sheetSet.GetCustomPropertyBag();
            }

            // Create a reference to a Custom Property Value 
            AcSmCustomPropertyValue customPropertyValue = new AcSmCustomPropertyValue();
            customPropertyValue.InitNew(owner);

            // Set the flag for the property 
            customPropertyValue.SetFlags(sheetSetFlag);

            // Set the value for the property 
            customPropertyValue.SetValue(propertyValue);

            // Create the property 
            customPropertyBag.SetProperty(propertyName, customPropertyValue);
        }

        // Synchronize the properties of a sheet with the sheet set 
        private void SyncProperties(AcSmDatabase sheetSetDatabase)
        {
            // Get the objects in the sheet set 
            IAcSmEnumPersist enumerator = sheetSetDatabase.GetEnumerator();

            // Get the first object in the Enumerator 
            IAcSmPersist item = enumerator.Next();

            // Step through all the objects in the sheet set 
            while ((item != null))
            {
                IAcSmSheet sheet = null;

                // Check to see if the object is a sheet 
                if (item.GetTypeName() == "AcSmSheet")
                {
                    sheet = (IAcSmSheet)item;

                    // Create a reference to the Property Enumerator for 
                    // the Custom Property Bag 
                    IAcSmEnumProperty enumeratorProperty = item.GetDatabase().GetSheetSet().GetCustomPropertyBag().GetPropertyEnumerator();

                    // Get the values from the Sheet Set to populate to the sheets 
                    string name = "";
                    AcSmCustomPropertyValue customPropertyValue = null;

                    // Get the first property 
                    enumeratorProperty.Next(out name, out customPropertyValue);

                    // Step through each of the properties 
                    while ((customPropertyValue != null))
                    {
                        // Check to see if the property is for a sheet 
                        if (customPropertyValue.GetFlags() == PropertyFlags.CUSTOM_SHEET_PROP)
                        {

                            //// Create a reference to the Custom Property Bag 
                            //AcSmCustomPropertyBag customSheetPropertyBag = sheet.GetCustomPropertyBag(); 

                            //// Create a reference to a Custom Property Value 
                            //AcSmCustomPropertyValue customSheetPropertyValue = new AcSmCustomPropertyValue();
                            //customSheetPropertyValue.InitNew(sheet); 

                            //// Set the flag for the property 
                            //customSheetPropertyValue.SetFlags(customPropertyValue.GetFlags()); 

                            //// Set the value for the property 
                            //customSheetPropertyValue.SetValue(customPropertyValue.GetValue()); 

                            //// Create the property 
                            //customSheetPropertyBag.SetProperty(name, customSheetPropertyValue); 

                            SetCustomProperty(sheet, name, customPropertyValue.GetValue(), customPropertyValue.GetFlags());
                        }

                        // Get the next property 
                        enumeratorProperty.Next(out name, out customPropertyValue);
                    }
                }

                // Get the next Sheet 
                item = enumerator.Next();
            }
        }


        // Set the default properties of a sheet set
        private void SetSheetSetDefaults(AcSmDatabase sheetSetDatabase, string name)
        {
            SetSheetSetDefaults(sheetSetDatabase, name, "", "", "", "", false);
        }

        private void SetSheetSetDefaults(AcSmDatabase sheetSetDatabase, string name, string description)
        {
            SetSheetSetDefaults(sheetSetDatabase, name, description, "", "", "", false);
        }

        private void SetSheetSetDefaults(AcSmDatabase sheetSetDatabase, string name, string description,
                                         string newSheetLocation)
        {
            SetSheetSetDefaults(sheetSetDatabase, name, description, newSheetLocation, "", "", false);
        }

        private void SetSheetSetDefaults(AcSmDatabase sheetSetDatabase, string name, string description,
                                         string newSheetLocation, string newSheetDWTLocation)
        {
            SetSheetSetDefaults(sheetSetDatabase, name, description, newSheetLocation, newSheetDWTLocation, "", false);
        }

        private void SetSheetSetDefaults(AcSmDatabase sheetSetDatabase, string name, string description,
                                         string newSheetLocation, string newSheetDWTLocation, string newSheetDWTLayout)
        {
            SetSheetSetDefaults(sheetSetDatabase, name, description, newSheetLocation, newSheetDWTLocation, newSheetDWTLayout, false);
        }

        private void SetSheetSetDefaults(AcSmDatabase sheetSetDatabase, string name, string description,
                                         string newSheetLocation, string newSheetDWTLocation, string newSheetDWTLayout,
                                         bool promptForDWT)
        {
            // Set the Name and Description for the sheet set 
            sheetSetDatabase.GetSheetSet().SetName(name);
            sheetSetDatabase.GetSheetSet().SetDesc(description);

            // Check to see if a Storage Location was provided 
            if (!string.IsNullOrEmpty(newSheetLocation))
            {
                // Get the folder the sheet set is stored in 
                string sheetSetFolder = sheetSetDatabase.GetFileName().Substring(0, sheetSetDatabase.GetFileName().LastIndexOf("\\"));

                // Create a reference to a File Reference object 
                IAcSmFileReference fileReference = default(IAcSmFileReference);
                fileReference = sheetSetDatabase.GetSheetSet().GetNewSheetLocation();

                // Set the default storage location based on the location of the sheet set 
                fileReference.SetFileName(sheetSetFolder);

                // Set the new Sheet location for the sheet set 
                sheetSetDatabase.GetSheetSet().SetNewSheetLocation(fileReference);
            }

            // Check to see if a Template was provided 
            if (!string.IsNullOrEmpty(newSheetDWTLocation))
            {
                // Set the Default Template for the sheet set 
                AcSmAcDbLayoutReference layoutReference = default(AcSmAcDbLayoutReference);
                layoutReference = sheetSetDatabase.GetSheetSet().GetDefDwtLayout();

                // Set the template location and name of the layout 
                // for the Layout Reference object 
                layoutReference.SetFileName(newSheetDWTLocation);
                layoutReference.SetName(newSheetDWTLayout);

                // Set the Layout Reference for the sheet set 
                sheetSetDatabase.GetSheetSet().SetDefDwtLayout(layoutReference);
            }

            // Set the Prompt for Template option of the subset 
            sheetSetDatabase.GetSheetSet().SetPromptForDwt(promptForDWT);
        }

    }
}
