namespace AIQueryVisualizer;

public class Utilities
{
    public static string? ExtractKqlFromValue(string value)
    {
        var match = Regex.Match(value, @"```(?:kql|kusto|sql)?\s*(.*?)\s*```", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }
    public static string[] GenerateRandomColors(int count)
    {
        var colors = new List<string>();
        var random = new Random();

        for (var i = 0; i < count; i++)
        {
            var r = random.Next(0, 256);
            var g = random.Next(0, 256);
            var b = random.Next(0, 256);
            colors.Add($"rgba({r}, {g}, {b}, 0.8)");
        }

        return colors.ToArray();
    }
}

