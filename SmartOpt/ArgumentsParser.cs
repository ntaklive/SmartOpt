using System;
using System.IO;
using System.Text.RegularExpressions;
using SmartOpt.Core.Infrastructure.Enums;

namespace SmartOpt;

public class ArgumentsParser
{
    public bool TryParseExcelWorkbookFilepath(string args, out string filepath)
    {
        filepath = null!;

        var regex = new Regex(@"-workbookFilepath '(.+\.(xlsm|xlsx))\'");
        Match match = regex.Match(args);
        if (!match.Success)
        {
            return false;
        }

        string argumentValue = match.Groups[1].Value;

        var file = new FileInfo(argumentValue);
        if (!file.Exists)
        {
            return false;
        }

        filepath = argumentValue;
        return true;
    }

    public bool TryParseMaxWidth(string args, out int maxWidth)
    {
        maxWidth = 0;

        if (!TryParseValueFromRegex(@"-maxWidth ([0-9]+)", args, out string argumentValue))
        {
            return false;
        }

        return int.TryParse(argumentValue, out maxWidth);
    }

    public bool TryParseGroupSize(string args, out int groupSize)
    {
        groupSize = 0;

        if (!TryParseValueFromRegex(@"-groupSize ([0-9]+)", args, out string argumentValue))
        {
            return false;
        }

        return int.TryParse(argumentValue, out groupSize);
    }

    public bool TryParseMaxWaste(string args, out double maxWaste)
    {
        maxWaste = 0;

        if (!TryParseValueFromRegex(@"-maxWaste ([0-9]+(\.[0-9]+)?)", args, out string argumentValue))
        {
            return false;
        }

        return double.TryParse(argumentValue, out maxWaste);
    }

    public bool TryParseOperationType(string args, out OperationType operationType)
    {
        operationType = OperationType.None;

        if (!TryParseValueFromRegex(@"-op (generatePatternLayouts)", args, out string argumentValue))
        {
            return false;
        }

        if (!Enum.TryParse(argumentValue, true, out OperationType parsedOperationType))
        {
            return false;
        }

        operationType = parsedOperationType;
        return true;
    }
    
    public bool TryParseGuiType(string args, out GuiType guiType)
    {
        guiType = GuiType.Gui;

        if (!TryParseValueFromRegex(@"-(gui|noGui)", args, out string argumentValue))
        {
            return false;
        }

        if (!Enum.TryParse(argumentValue, true, out GuiType parsedGuiType))
        {
            return false;
        }

        guiType = parsedGuiType;
        return true;
    }
    
    public bool TryParseHelp(string args)
    {
        if (!TryParseValueFromRegex(@"(\/\?)", args, out string _))
        {
            return false;
        }

        return true;
    }

    private bool TryParseValueFromRegex(string pattern, string args, out string value)
    {
        value = default!;

        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        Match match = regex.Match(args);
        if (!match.Success)
        {
            return false;
        }

        value = match.Groups[1].Value;
        return true;
    }
}