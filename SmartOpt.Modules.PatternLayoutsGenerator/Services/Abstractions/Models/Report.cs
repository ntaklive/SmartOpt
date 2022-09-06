using System.Collections.Generic;

namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions.Models;

public class Report
{
    public Report(ICollection<PatternLayout> patternLayouts)
    {
        PatternLayouts = patternLayouts;
    }
    
    public ICollection<PatternLayout> PatternLayouts { get; }
}