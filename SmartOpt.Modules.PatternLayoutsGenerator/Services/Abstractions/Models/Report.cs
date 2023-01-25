using System.Collections.Generic;
using System.Linq;
using Theraot.Collections;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

public class Report
{
    private readonly List<PatternLayout> _patternLayouts;
    private readonly List<OrderInfo> _ungroupedOrders;

    public Report(IEnumerable<PatternLayout> patternLayouts, IEnumerable<OrderInfo> ungroupedOrders)
    {
        _patternLayouts = patternLayouts.ToList();
        _ungroupedOrders = ungroupedOrders.ToList();
    }
    
    public Report()
    {
        _patternLayouts = new List<PatternLayout>();
        _ungroupedOrders = new List<OrderInfo>();
    }
    
    public IReadOnlyList<PatternLayout> GetPatternLayouts() => _patternLayouts.AsIReadOnlyList();
    public IReadOnlyList<OrderInfo> GetUngroupedOrders() => _ungroupedOrders.AsIReadOnlyList();

    public void AddPatternLayout(PatternLayout patternLayout)
    {
        _patternLayouts.Add(patternLayout);
    }    
    
    public void AddUngroupedOrders(IEnumerable<OrderInfo> ungroupedOrders)
    {
        _ungroupedOrders.AddRange(ungroupedOrders);
    }
}