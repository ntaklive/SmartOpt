using System.Collections.Generic;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

public class Report
{
    public Report(IEnumerable<PatternLayout> patternLayouts)
    {
        PatternLayouts = new List<PatternLayout>(patternLayouts);
    }

    public Report()
    {
        PatternLayouts = new List<PatternLayout>();
    }

    public List<PatternLayout> PatternLayouts { get; }

    public void AddPatternLayout(PatternLayout patternLayout)
    {
        PatternLayouts.Add(patternLayout);
    }
}