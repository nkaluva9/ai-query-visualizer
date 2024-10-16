namespace AIQueryVisualizer.Models;
public class ResultRow(Dictionary<string, object> values)
{
    public Dictionary<string, object> Values { get; } = values;
}