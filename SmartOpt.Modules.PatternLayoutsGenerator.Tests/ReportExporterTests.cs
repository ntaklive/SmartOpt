using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SmartOpt.Modules.PatternLayoutsGenerator.Services;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using SmartOpt.Modules.PatternLayoutsGenerator.Tests.Extensions;
using Theraot.Collections;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Tests
{
    [TestFixture]
    public class ReportExporterTests
    {
        // Format: MethodName_StateUnderTest_ExpectedBehavior

        [Test]
        public void Test_Valid_True()
        {
            // Allocate
            var reportExporter = new ReportExporter(new ExcelInteropWrapper());
            
            // Act
            reportExporter.ExportToNewExcelWorkbook(Constants.ExampleReport);

            // Assert
        }
    }
}