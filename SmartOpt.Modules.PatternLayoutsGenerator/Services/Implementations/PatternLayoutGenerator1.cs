// using System.Collections.Generic;
// using System.Linq;
// using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
// using SmartOpt.Core.Extensions;
// using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;
// using Theraot.Collections;
//
// namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;
//
// public class PatternLayoutGenerator1 : IPatternLayoutGenerator
// {
//     public IReadOnlyList<PatternLayout> GeneratePatternLayoutsFromOrders(IReadOnlyCollection<OrderInfo> orderInfos,
//         int maxWidth, double maxWaste, int groupSize)
//     {
//         if (orderInfos.Count < groupSize)
//         {
//             groupSize = orderInfos.Count;
//         }
//         
//         var patternLayouts = new List<PatternLayout>();
//         
//         var elementsForGroupingExist = true;
//
//         List<OrderInfo> allOrders = orderInfos.ToList();
//         while (elementsForGroupingExist)
//         {
//             allOrders = allOrders.OrderByDescending(order => order.RollsCount).ToList();
//             
//             if (TryGroupOrders(allOrders, groupSize, maxWaste, maxWidth,
//                     out elementsForGroupingExist,
//                     out List<OrderInfo> ordersGroup,
//                     out double groupWaste,
//                     out List<OrderInfo> remainingOrders))
//             {
//                 allOrders = MergeSplitOrders(remainingOrders);
//                 patternLayouts.Add(CreatePatternLayout(ordersGroup, groupWaste));
//             }
//         }
//
//         allOrders = MergeSplitOrders(allOrders);
//         
//         patternLayouts.Add(CreatePatternLayout(allOrders, 100.0));
//         
//         return patternLayouts.AsIReadOnlyList();
//     }
//
//     private static List<OrderInfo> SubtractIncludedInGroupRollsFromOrders(
//         IEnumerable<OrderInfo> orders, double rollsCount)
//     {
//         List<OrderInfo> internalOrders = orders.ToList();
//         
//         internalOrders.ForEach(order =>
//         {
//             order.RollsCount -= rollsCount;
//         });
//
//         return internalOrders;
//     }
//
//     private static bool TryGroupOrders(List<OrderInfo> orders, int groupSize, double maxWaste, int maxWidth,
//         out bool elementsForGroupingExist, out List<OrderInfo> ordersGroup, out double groupWaste, out List<OrderInfo> remainingOrders)
//     {
//         elementsForGroupingExist = true;
//         
//         List<OrderInfo> internalOrders = orders.ToList();
//         List<OrderInfo> internalOrdersGroup = internalOrders.GetRange(0, groupSize);
//         List<OrderInfo> ungroupedOrders = internalOrders.GetRange(groupSize, internalOrders.Count - groupSize);
//         
//         double internalGroupWaste = CalculateWasteForOrdersGroup(internalOrdersGroup, maxWidth);
//         while (internalGroupWaste >= maxWaste || internalGroupWaste < 0)
//         {
//             if (TryFindSuitableOrderIndexForReplacing(internalOrdersGroup, ungroupedOrders, internalGroupWaste, maxWidth, maxWaste, out int suitableOrderForReplacingIndex))
//             {
//                 (internalOrdersGroup[0], ungroupedOrders[suitableOrderForReplacingIndex]) = (ungroupedOrders[suitableOrderForReplacingIndex], internalOrdersGroup[0]);
//                 
//                 internalGroupWaste = CalculateWasteForOrdersGroup(internalOrdersGroup, maxWidth);
//             }
//             else
//             {
//                 if (TryFindSuitableOrderIndexForSplitting(internalOrders, out int suitableOrderForSplittingIndex))
//                 {
//                     SplitOrderIntoTwo(internalOrders, suitableOrderForSplittingIndex);
//                     elementsForGroupingExist = true;
//                 }
//                 else
//                 {
//                     elementsForGroupingExist = false;
//                 }
//
//                 ordersGroup = internalOrdersGroup;
//                 remainingOrders = internalOrders;
//                 groupWaste = internalGroupWaste;
//                 return false;
//             }
//         }
//
//         ordersGroup = internalOrdersGroup;
//         remainingOrders = internalOrders;
//         groupWaste = internalGroupWaste;
//         return true;
//     }
//
//     private static PatternLayout CreatePatternLayout(List<OrderInfo> orders, double waste)
//     {
//         double minGroupRollsCount = orders.Min(x => x.RollsCount);
//         orders = SubtractIncludedInGroupRollsFromOrders(orders, minGroupRollsCount);
//         return new PatternLayout(orders, minGroupRollsCount, waste);
//     }
//
//     private static List<OrderInfo> MergeSplitOrders(IEnumerable<OrderInfo> splitOrders)
//     {
//         return splitOrders
//             .GroupBy(c => c.Width)
//             .Select(group => new OrderInfo(group.First().Name, group.Key, group.Sum(ci => ci.RollsCount)))
//             .ToList();
//     }
//
//     private static double CalculateWasteForOrdersGroup(IEnumerable<OrderInfo> ordersGroup, int maxWidth)
//     {
//         return (1 - (double) ordersGroup.Sum(order => order.Width) / maxWidth) * 100;
//     }
//
//     private static List<OrderInfo> SplitSpecifiedOrderIntoTwo(IEnumerable<OrderInfo> orders, int orderForSplittingIndex)
//     {
//         List<OrderInfo> internalOrders = orders.ToList();
//
//         OrderInfo suitableOrder = internalOrders[orderForSplittingIndex]; 
//         
//         suitableOrder.RollsCount /= 2;
//         internalOrders.Add(suitableOrder.Clone());
//
//         return internalOrders;
//     }
//     
//     private static void SplitOrderIntoTwo(List<OrderInfo> orders, int orderForSplittingIndex)
//     {
//         OrderInfo suitableOrder = orders[orderForSplittingIndex]; 
//         
//         suitableOrder.RollsCount /= 2;
//         orders.Add(suitableOrder.Clone());
//     }
//
//     private static bool TryFindSuitableOrderIndexForSplitting(List<OrderInfo> orders, out int index)
//     {
//         index = 0;
//
//         index = orders.FindIndex(x =>
//             x.RollsCount >= 2 &&
//             orders.Max(c => c.RollsCount).Equals7DigitPrecision(x.RollsCount));
//
//         return index >= 0;
//     }
//
//     private static bool TryFindSuitableOrderIndexForReplacing(List<OrderInfo> ordersGroup, List<OrderInfo> unprocessedOrders, double waste, int maxWidth, double maxWaste, out int index)
//     {
//         index = 0;
//         
//         if (waste >= maxWaste)
//         {
//             ordersGroup.Sort((prev, next) =>
//                 prev.Width.CompareTo(next.Width));
//             
//             index = unprocessedOrders.FindIndex(item =>
//                 ordersGroup[0].Width < item.Width &&
//                 item.Width - ordersGroup[0].Width < maxWidth * waste / 100 &&
//                 item.RollsCount >= 1);
//         }
//         else
//         {
//             ordersGroup.Sort((prev, next) =>
//                 next.Width.CompareTo(prev.Width));
//             
//             index = unprocessedOrders.FindIndex(item =>
//                 ordersGroup[0].Width > item.Width &&
//                 ordersGroup[0].Width - item.Width > maxWidth * waste / 100 &&
//                 item.RollsCount >= 1);
//         }
//
//         return index >= 0;
//     }
// }