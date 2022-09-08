using System.Collections.Generic;
using NUnit.Framework;
using SmartOpt.Modules.PatternLayoutsGenerator.Services;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using SmartOpt.Modules.PatternLayoutsGenerator.Tests.Extensions;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Tests
{
    [TestFixture]
    public class PatternLayoutServiceTests
    {
        // Format: MethodName_StateUnderTest_ExpectedBehavior
        
        [Test]
        public void CreatePatternLayoutsFromOrders_Valid_True()
        {
            // Allocate
            const int maxWidth = 6000;
            const double maxWaste = 3.0;
            const int groupSize = 5;
            
            var patternLayoutService = new PatternLayoutGenerator();
            var ordersMerger = new OrderInfoMerger();

            var orders = new List<OrderInfo>
            {
                new("2773/18", 1300, 5.882352941),
                new("2773/18", 650, 5.882352941),
                new("2773/18", 1200, 5.882352941),
                new("2773/18", 800, 5.882352941),
                new("2773/18", 550, 5.882352941),
                new("2773/18", 600, 4.901960784),
                new("546/19-257", 1000, 3.529411765),
                new("546/19-257", 1100, 2.139037433),
                new("546/19-257", 1160, 2.535496957),
                new("546/19-257", 1200, 2.941176471),
                new("546/19-257", 1220, 2.892960463),
                new("546/19-259", 840, 3.008403361),
                new("2769/18-12", 320, 3.063725490),
                new("2769/18-12", 340, 1.153402537)
            };
            
            // Act
            IList<OrderInfo> mergedOrders = ordersMerger.MergeOrdersWithIdenticalWidth(orders);
            IList<PatternLayout> layouts = patternLayoutService.GeneratePatternLayoutsFromOrders(mergedOrders, maxWidth, maxWaste, groupSize);
            
            // Assert
            Assert.True(layouts[0].Waste.Equals7DigitPrecision(2.0));
            Assert.True(layouts[1].Waste.Equals7DigitPrecision(0));
            Assert.True(layouts[0].RollsCount.Equals7DigitPrecision(2.53549695740365));
            Assert.True(layouts[1].RollsCount.Equals7DigitPrecision(1.57200811359027));
        }
    }
}