using System.Collections.Generic;
using System.Linq;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Core.Extensions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
using Theraot.Collections;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class PatternLayoutGenerator : IPatternLayoutGenerator
{
    public Report GeneratePatternLayoutsFromOrders(IReadOnlyList<OrderInfo> orderInfos,
        int maxWidth, double maxWaste, int groupSize)
    {
        orderInfos = AggregateOrdersWithIdenticalWidth(orderInfos);
        
        if (orderInfos.Count < groupSize)
        {
            groupSize = orderInfos.Count;
        }
        
        var report = new Report();
        
        List<OrderInfo> allOrders = orderInfos.ToList();
        
        var elementsForGroupingExist = true;
        while (elementsForGroupingExist)
        {
            allOrders.Sort((prev, next) => next.RollsCount.CompareTo(prev.RollsCount));

            List<OrderInfo> ordersGroup = allOrders.GetRange(0, groupSize);
            List<OrderInfo> ungroupedOrders = allOrders.GetRange(groupSize, allOrders.Count - groupSize);

            if (TryCreatePatternLayout(allOrders, ungroupedOrders, ordersGroup, maxWaste, maxWidth,
                    out PatternLayout patternLayout, out elementsForGroupingExist))
            {
                allOrders = MergeSplitOrders(allOrders);
                AddPatternLayoutToReport(report, patternLayout);
            }
        }

        allOrders = MergeSplitOrders(allOrders);
        AddRemainingOrdersToReport(report, allOrders);
        return report;
    }

    private static void AddPatternLayoutToReport(Report report, PatternLayout patternLayout)
    {
        report.AddPatternLayout(patternLayout);
    }

    private static void AddRemainingOrdersToReport(Report report, List<OrderInfo> remainingOrders)
    {
        // Implementation details. CreatePatternLayout method is "subtracting rolls count from the remaining orders"
        PatternLayout patternLayout = CreatePatternLayout(remainingOrders, 100.0);
        report.AddUngroupedOrders(patternLayout.Orders);
    }
    
    private static bool TryCreatePatternLayout(
        List<OrderInfo> orders, List<OrderInfo> unprocessedOrders,
        List<OrderInfo> ordersGroup, double maxWaste, int maxWidth,
        out PatternLayout patternLayout, out bool elementsForGroupingExist)
    {
        patternLayout = null!;
        elementsForGroupingExist = true;
        
        double groupWaste = CalculateWasteForOrdersGroup(ordersGroup, maxWidth);
        while (groupWaste > maxWaste || groupWaste < 0)
        {
            if (TryFindSuitableOrderIndexForReplacing(ordersGroup, unprocessedOrders, groupWaste, maxWidth, maxWaste, out int suitableOrderForReplacingIndex))
            {
                (ordersGroup[0], unprocessedOrders[suitableOrderForReplacingIndex]) = (unprocessedOrders[suitableOrderForReplacingIndex], ordersGroup[0]);
                
                groupWaste = CalculateWasteForOrdersGroup(ordersGroup, maxWidth);
            }
            else
            {
                if (TryFindSuitableOrderIndexForSplitting(orders, out int suitableOrderForSplittingIndex))
                {
                    SplitOrderIntoTwo(orders, suitableOrderForSplittingIndex);
                }
                else
                {
                    elementsForGroupingExist = false;
                }
                
                return false;
            }
        }

        patternLayout = CreatePatternLayout(ordersGroup, groupWaste);
        return true;
    }
    
    private static PatternLayout CreatePatternLayout(List<OrderInfo> orders, double waste)
    {
        double minGroupRollsCount = orders.Min(x => x.RollsCount);
        var ordersGroup = new List<OrderInfo>();
        orders.ForEach(order =>
        {
            ordersGroup.Add(order.Clone());
            order.RollsCount -= minGroupRollsCount;
        });
        
        return new PatternLayout(ordersGroup, minGroupRollsCount, waste);
    }

    private static List<OrderInfo> MergeSplitOrders(IEnumerable<OrderInfo> splitOrders)
    {
        return splitOrders
            .GroupBy(c => c.Width)
            .Select(group => new OrderInfo(group.First().Name, group.Key, group.Sum(ci => ci.RollsCount)))
            .ToList();
    }

    private static double CalculateWasteForOrdersGroup(IEnumerable<OrderInfo> ordersGroup, int maxWidth)
    {
        return (1 - (double) ordersGroup.Sum(order => order.Width) / maxWidth) * 100;
    }

    private static void SplitOrderIntoTwo(List<OrderInfo> orders, int orderForSplittingIndex)
    {
        OrderInfo suitableOrder = orders[orderForSplittingIndex]; 
        
        suitableOrder.RollsCount /= 2;
        orders.Add(suitableOrder.Clone());
    }

    private static bool TryFindSuitableOrderIndexForSplitting(List<OrderInfo> orders, out int index)
    {
        index = 0;

        index = orders.FindIndex(x =>
            x.RollsCount >= 2 &&
            orders.Max(c => c.RollsCount).Equals7DigitPrecision(x.RollsCount));

        return index >= 0;
    }

    private static bool TryFindSuitableOrderIndexForReplacing(List<OrderInfo> ordersGroup, List<OrderInfo> unprocessedOrders, double groupWaste, int maxWidth, double maxWaste, out int index)
    {
        index = 0;
        
        if (groupWaste >= maxWaste)
        {
            ordersGroup.Sort((prev, next) =>
                prev.Width.CompareTo(next.Width));
            
            index = unprocessedOrders.FindIndex(item =>
                ordersGroup[0].Width < item.Width &&
                item.Width - ordersGroup[0].Width < maxWidth * groupWaste / 100 &&
                item.RollsCount >= 1);
        }
        else
        {
            ordersGroup.Sort((prev, next) =>
                next.Width.CompareTo(prev.Width));
            
            index = unprocessedOrders.FindIndex(item =>
                ordersGroup[0].Width > item.Width &&
                ordersGroup[0].Width - item.Width > maxWidth * groupWaste / 100 &&
                item.RollsCount >= 1);
        }

        return index >= 0;
    }
    
    private static IReadOnlyList<OrderInfo> AggregateOrdersWithIdenticalWidth(IReadOnlyList<OrderInfo> orders)
    {
        IEnumerable<int> elementsCount = orders
            .Select(x => x.Width)
            .Distinct();

        return elementsCount
            .Select(item => orders.Where(x => x.Width == item))
            .Select(tmp => tmp.Aggregate((prev, next) =>
            {
                prev.Name += ", " + next.Name;
                prev.RollsCount += next.RollsCount;
                return prev;
            }))
            .ToList()
            .AsIReadOnlyList();
    }
}