#nullable enable
using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace TestConsole
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            object? excelCom = Marshal.GetActiveObject("Excel.Application");
            if (excelCom == null)
            {
                throw new InvalidOperationException("Unable to connect with an active Excel application");
            }

            var application = (excelCom as Application)!;

            Console.WriteLine(application.ActiveWorkbook.FullName);
            
            Marshal.ReleaseComObject(application);
        }
    }
}