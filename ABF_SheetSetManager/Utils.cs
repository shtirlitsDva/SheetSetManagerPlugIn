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
        // ────────────────────────────────────────────────────────────────
        //  Alphabetic sequence: A, B, … Z, AA, AB, … ZZ, AAA …
        // ────────────────────────────────────────────────────────────────
        public static string NextAlpha(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "A";

            var chars = value.ToUpperInvariant().ToCharArray();

            // increment like a base-26 counter where A=0 … Z=25
            for (int i = chars.Length - 1; i >= 0; i--)
            {
                if (chars[i] < 'A' || chars[i] > 'Z')
                    throw new ArgumentException("Input contains non-letter characters.", nameof(value));

                if (chars[i] == 'Z')
                {
                    chars[i] = 'A';
                    if (i == 0)                         // overflow at leftmost char
                        return "A" + new string(chars); // prepend a new digit
                    continue;                           // carry to next position
                }

                chars[i]++;                             // simple increment, done
                return new string(chars);
            }

            return new string(chars); // never reached
        }

        // ────────────────────────────────────────────────────────────────
        //  Numeric sequence: 1, 2, … 9, 10, 11 … 99, 100 …
        //  Preserves leading-zero padding width (e.g. 004 → 005)
        // ────────────────────────────────────────────────────────────────
        public static string NextNumeric(string? value)
        {
            // first element when string is null/empty
            if (string.IsNullOrWhiteSpace(value))
                return "01";

            if (!value.All(char.IsDigit))
                throw new ArgumentException("Input contains non-digit characters.", nameof(value));

            int width = Math.Max(value.Length, 2);     // always at least 2 digits
            long number = long.Parse(value) + 1;

            return number.ToString($"D{width}");         // zero-padded
        }
    }
}
