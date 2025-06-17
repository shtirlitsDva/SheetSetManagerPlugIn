using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

using oid = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;
using OpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using BlockReference = Autodesk.AutoCAD.DatabaseServices.BlockReference;
using ObjectIdCollection = Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection;
using SheetSetManager.SheetManager.Interop;

namespace SheetSetManager
{
    public static class Utils
    {
        public static void prdDbg(string msg = "") => 
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n" + msg);
        public static void prdDbg(object obj)
        {
            if (obj is SystemException ex1) prdDbg(obj.ToString().WrapThis(90));
            else if (obj is System.Exception ex2) prdDbg(obj.ToString().WrapThis(90));
            else prdDbg(obj.ToString());
        }
        /// <summary>
        /// Returns a list of strings no larger than the max length sent in.
        /// </summary>
        /// <remarks>useful function used to wrap string text for reporting.</remarks>
        /// <param name="text">Text to be wrapped into of List of Strings</param>
        /// <param name="maxLength">Max length you want each line to be.</param>
        /// <returns>List of Strings</returns>
        public static string WrapThis(this string s, int maxLength)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            var lines = s.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            var wrappedLines = new List<string>();

            foreach (var line in lines)
            {
                if (line.Length <= maxLength)
                {
                    wrappedLines.Add(line);
                }
                else
                {
                    int start = 0;
                    while (start < line.Length)
                    {
                        int length = Math.Min(maxLength, line.Length - start);
                        wrappedLines.Add(line.Substring(start, length));
                        start += length;
                    }
                }
            }
            return string.Join("\n", wrappedLines);
        }

        public static void prtDbg(string msg = "")
        {
            AcContext.Current.Post(_ => { prdDbg(msg); }, null);
        }
        public static void prtDbg(object obj)
        {
            AcContext.Current.Post(_ => { prdDbg(obj); }, null);
        }
    }
}
