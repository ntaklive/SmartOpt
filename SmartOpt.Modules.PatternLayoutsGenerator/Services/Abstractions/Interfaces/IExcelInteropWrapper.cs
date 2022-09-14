namespace SmartOpt.Modules.PatternLayoutsGenerator.Services.Abstractions;

public interface IExcelInteropWrapper
{
    public void WriteTextInCell(int x, int y, string text);
    public void MergeCells(int x1, int y1, int x2, int y2);
    public void ReleaseComObjects();
}