using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ExcelInterop = global::Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Excel;

namespace TarjimonOfficeUZ.Excel
{
    public partial class ThisAddIn
    {
        private AddInUndoAutomationService undoAutomationService;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            ExcelTranslationUndoManager.Clear();
        }

        protected override object RequestComAddInAutomationService()
        {
            if (undoAutomationService == null)
                undoAutomationService = new AddInUndoAutomationService();

            return undoAutomationService;
        }

        public void UndoLastTranslation()
        {
            ExcelTranslationUndoManager.Undo();
        }

        #region Код, автоматически созданный VSTO

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAddInUndoAutomation
    {
        void UndoLastTranslation();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AddInUndoAutomationService : StandardOleMarshalObject, IAddInUndoAutomation
    {
        public void UndoLastTranslation()
        {
            Globals.ThisAddIn.UndoLastTranslation();
        }
    }

    internal sealed class ExcelTranslationUndoSnapshot
    {
        public string WorkbookFullName { get; set; }
        public string WorksheetName { get; set; }
        public string Address { get; set; }
        public object OriginalValue { get; set; }
    }

    internal static class ExcelTranslationUndoManager
    {
        private static readonly object Sync = new object();
        private static readonly List<ExcelTranslationUndoSnapshot> Snapshots = new List<ExcelTranslationUndoSnapshot>();
        private static bool isUndoing;

        public static void Clear()
        {
            lock (Sync)
            {
                Snapshots.Clear();
            }
        }

        public static void CaptureCell(ExcelInterop.Range cell, object originalValue)
        {
            if (cell == null || isUndoing)
                return;

            try
            {
                ExcelInterop.Worksheet worksheet = cell.Worksheet as ExcelInterop.Worksheet;
                ExcelInterop.Workbook workbook = worksheet == null ? null : worksheet.Parent as ExcelInterop.Workbook;

                if (worksheet == null || workbook == null)
                    return;

                lock (Sync)
                {
                    Snapshots.Add(new ExcelTranslationUndoSnapshot
                    {
                        WorkbookFullName = workbook.FullName,
                        WorksheetName = worksheet.Name,
                        Address = cell.Address[false, false, ExcelInterop.XlReferenceStyle.xlA1],
                        OriginalValue = originalValue
                    });
                }
            }
            catch
            {
                // A failed snapshot must never break translation.
            }
        }

        public static void Undo()
        {
            List<ExcelTranslationUndoSnapshot> snapshotCopy;

            lock (Sync)
            {
                if (Snapshots.Count == 0)
                    return;

                snapshotCopy = new List<ExcelTranslationUndoSnapshot>(Snapshots);
                Snapshots.Clear();
                isUndoing = true;
            }

            try
            {
                ExcelInterop.Application application = Globals.ThisAddIn.Application;

                foreach (ExcelTranslationUndoSnapshot snapshot in snapshotCopy)
                {
                    try
                    {
                        ExcelInterop.Workbook workbook = FindWorkbook(application, snapshot.WorkbookFullName);
                        if (workbook == null)
                            continue;

                        ExcelInterop.Worksheet worksheet = workbook.Worksheets[snapshot.WorksheetName] as ExcelInterop.Worksheet;
                        if (worksheet == null)
                            continue;

                        ExcelInterop.Range cell = worksheet.Range[snapshot.Address];
                        if (cell.HasFormula)
                            continue;

                        cell.Value2 = snapshot.OriginalValue;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            finally
            {
                lock (Sync)
                {
                    isUndoing = false;
                }
            }
        }

        private static ExcelInterop.Workbook FindWorkbook(ExcelInterop.Application application, string fullName)
        {
            foreach (ExcelInterop.Workbook workbook in application.Workbooks)
            {
                try
                {
                    if (string.Equals(workbook.FullName, fullName, StringComparison.OrdinalIgnoreCase))
                        return workbook;
                }
                catch
                {
                }
            }

            return null;
        }
    }
}
