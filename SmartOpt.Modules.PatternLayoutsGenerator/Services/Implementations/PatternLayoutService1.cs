using System.Collections.Generic;
using System.Linq;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;
using SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services;

public class PatternLayoutService1 : IPatternLayoutService
{
    private double _maxWidth = 6000;
    private double _maxWaste = 4;
    private int _orderCount = 5;
    private readonly List<PatternLayout> _orders = new List<PatternLayout>();
    private bool _ordersCanBeSplit = true;

    public IList<PatternLayout> CreatePatternLayoutsFromOrders(IList<OrderInfo> orderInfos, int maxWidth,
        double maxWaste, int groupSize)
    {
        List<OrderInfo> orders = orderInfos.ToList();
        _maxWidth = maxWidth;
        _maxWaste = maxWaste;
        _orderCount = groupSize;

        bool elementsForCutExists = true; // есть ли мембраны (члены?) на отрезание
        bool elementsForAddExists = true;

        while (elementsForCutExists) // пока есть, что отрезать
        {
            orders.Sort((prev, next) =>
                next.RollsCount.CompareTo(prev.RollsCount)); // сортировка заказов по убыванию количества
            List<OrderInfo> selectedOrders = new List<OrderInfo>();
            selectedOrders.AddRange(orders.GetRange(0, _orderCount)); // выбираем первую группу (5 или 6) заказов

            if (selectedOrders.Any(o => o.RollsCount < 1))
            {
                if (_ordersCanBeSplit)
                {
                    SplitCommonInfos(orders);
                    continue;
                }

                break;
            }

            double waste = (1 - (selectedOrders.Sum(x => x.Width) / _maxWidth)) *
                           100; // определяем процент отходов в первой группе
            var leftOrders = orders.Skip(_orderCount).ToList(); // определяем оставшиеся заказы в новую переменную

            while (waste > _maxWaste || waste < 0) // пока отход плохой
            {
                elementsForAddExists = true;
                int commonInfoIndex = 0; // индекс первого подходящего для замены элемента

                if (waste > 0) // если отход положительный, на самом деле если он больше допустимого (waste > MaxWaste)
                {
                    // вывод: элемент слишком маленький (отход превышает предел)
                    // наименьший по ширине элемент заменяется на первый попавшийся элемент чуть побольше, подходящий под условия ниже

                    selectedOrders.Sort((prev, next) =>
                        prev.Width.CompareTo(next.Width)); // Сортируем выбранную группу заказов по возрастанию ширины
                    commonInfoIndex = leftOrders.FindIndex(item => // из оставшихся заказов выбираем заказ, который
                        selectedOrders[0].Width < item.Width && // по ширине больше первого заказа из выбранной группы и
                        item.Width - selectedOrders[0].Width <
                        _maxWidth * waste / 100 && // чтобы новый отход в группе был меньше 240
                        item.RollsCount >= 1); // и чтобы были заказы
                }
                else // если отход <= 0, то есть общая ширина группы больше максимальной ширины (MaxWidth)
                {
                    // вывод: элемент слишком большой (отход отрицательный — ширина больше допустимой)
                    // наибольший по ширине элемент заменяется на первый попавшийся элемент чуть поменьше, подходящий под условия ниже

                    selectedOrders.Sort((prev, next) =>
                        next.Width.CompareTo(prev.Width)); // Сортируем выбранную группу заказов по убыванию ширины
                    commonInfoIndex = leftOrders.FindIndex(item => // из оставшихся заказов выбираем заказ, который
                        selectedOrders[0].Width > item.Width && // по ширине меньше первого заказа из выбранной группы и
                        selectedOrders[0].Width - item.Width >
                        _maxWidth * waste / 100 && // чтобы новый отход в группе был меньше 240
                        item.RollsCount >= 1); // и чтобы были заказы
                }

                if (commonInfoIndex >= 0) // если найден подходящий индекс
                {
                    // производим замену элементов по одному из условий выше

                    (selectedOrders[0], leftOrders[commonInfoIndex]) = (leftOrders[commonInfoIndex], selectedOrders[0]);
                }
                else
                {
                    if (_ordersCanBeSplit)
                    {
                        SplitCommonInfos(orders);
                        elementsForAddExists = false;
                        break;
                    }

                    // подходящий элемент для замены не найден
                    // больше резать не получится (9(

                    elementsForCutExists = false;
                    elementsForAddExists = false;
                    break;
                }

                waste = (1 - selectedOrders.Sum(x => x.Width) / _maxWidth) * 100; // пересчитываем отходы
            }

            if (elementsForAddExists) // если есть шо добавить
            {
                _orders.Add(new PatternLayout(selectedOrders, waste)); // добавляем
                orders = CombineSeparatedCommonInfos(orders);
            }
        }

        orders = CombineSeparatedCommonInfos(orders);
        _orders.Add(new PatternLayout(orders, 100.00)); // записываем оставшиеся неразрезанные заказы с отходом 100
        return _orders;
    }

    private void SplitCommonInfos(List<OrderInfo> commonInfos)
    {
        var suitableCommonInfo = commonInfos.FirstOrDefault(ci =>
            ci.RollsCount >= 2 && ci.RollsCount == commonInfos.Max(c => c.RollsCount));

        if (suitableCommonInfo == null)
        {
            _ordersCanBeSplit = false;
            return;
        }

        suitableCommonInfo.RollsCount /= 2;
        commonInfos.Add(suitableCommonInfo.Clone());
    }

    private List<OrderInfo> CombineSeparatedCommonInfos(IEnumerable<OrderInfo> commonInfos)
    {
        return commonInfos
            .GroupBy(c => c.Width)
            .Select(g => new OrderInfo(g.First().Name, g.Key, g.Sum(ci => ci.RollsCount)))
            .ToList();
    }
}