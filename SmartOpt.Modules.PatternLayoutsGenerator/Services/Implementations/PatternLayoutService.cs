using System;
using System.Collections.Generic;
using System.Linq;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Core.Extensions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class PatternLayoutService : IPatternLayoutService
{
    public IList<PatternLayout> CreatePatternLayoutsFromOrders(IList<OrderInfo> orderInfos, int maxWidth,
        double maxWaste, int groupSize)
    {
        if (orderInfos.Count < groupSize)
        {
            groupSize = orderInfos.Count;
            // todo бросать или не бросать
            // throw new InvalidOperationException("Cannot create pattern layout, 'cause orders count is lesser than group size");
        }
        
        var patternLayouts = new List<PatternLayout>();
        
        var elementsForGroupingExist = true;

        List<OrderInfo> orders = orderInfos.ToList();
        while (elementsForGroupingExist)
        {
            orders = orders.OrderByDescending(order => order.RollsCount).ToList();

            List<OrderInfo> ordersGroup = orders.GetRange(0, groupSize);
            List<OrderInfo> unprocessedOrders = orders.GetRange(groupSize, orders.Count - groupSize);
            
            // todo этот код не делает ничего
            // if (ordersGroup.Any(order => order.RollsCount < 1))
            // {
            //     if (TryFindSuitableOrderIndexForSplitting(orders, out int suitableOrderForSplittingIndex))
            //     {
            //         SplitOrderIntoTwo(orders, suitableOrderForSplittingIndex);
            //         
            //         continue;
            //     }
            //
            //     break;
            // }

            if (TryCreatePatternLayout(orders, unprocessedOrders, ordersGroup, maxWaste, maxWidth,
                    out PatternLayout patternLayout, out elementsForGroupingExist))
            {
                patternLayouts.Add(patternLayout);
                orders = MergeSplitOrders(orders);
            }
        }

        orders = MergeSplitOrders(orders);
        patternLayouts.Add(new PatternLayout(orders, 100.0));
        return patternLayouts;
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
            if (TryFindSuitableOrderIndexForReplacing(ordersGroup, unprocessedOrders, groupWaste, maxWidth, out int suitableOrderForReplacingIndex))
            {
                (ordersGroup[0], unprocessedOrders[suitableOrderForReplacingIndex]) = (unprocessedOrders[suitableOrderForReplacingIndex], ordersGroup[0]);
            }
            else
            {
                if (TryFindSuitableOrderIndexForSplitting(orders, out int suitableOrderForSplittingIndex))
                {
                    SplitOrderIntoTwo(orders, suitableOrderForSplittingIndex);

                    return false;
                }

                elementsForGroupingExist = false;
                return false;
            }

            groupWaste = CalculateWasteForOrdersGroup(ordersGroup, maxWidth);
        }

        patternLayout = new PatternLayout(ordersGroup, groupWaste);
        return true;
    }

    private List<OrderInfo> MergeSplitOrders(IEnumerable<OrderInfo> splitOrders)
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

    private static bool TryFindSuitableOrderIndexForReplacing(List<OrderInfo> ordersGroup, List<OrderInfo> unprocessedOrders, double waste, int maxWidth, out int index)
    {
        index = 0;
        
        if (waste > 0)
        {
            ordersGroup.Sort((prev, next) =>
                prev.Width.CompareTo(next.Width));
            index = unprocessedOrders.FindIndex(item =>
                ordersGroup[0].Width < item.Width &&
                item.Width - ordersGroup[0].Width <
                maxWidth * waste / 100 &&
                item.RollsCount >= 1);
        }
        else
        {
            ordersGroup.Sort((prev, next) =>
                next.Width.CompareTo(prev.Width));
            index = unprocessedOrders.FindIndex(item =>
                ordersGroup[0].Width > item.Width &&
                ordersGroup[0].Width - item.Width >
                maxWidth * waste / 100 &&
                item.RollsCount >= 1);
        }

        return index >= 0;
    }
}