namespace AIQueryVisualizer.Services;
public class QueryServiceFactory(IServiceProvider serviceProvider)
{
    public IQueryService GetQueryService(string serviceType)
    {
        return serviceType switch
        {
            "KQL" => serviceProvider.GetRequiredService<KqlQueryService>(),
            "SQL" => serviceProvider.GetRequiredService<SqlQueryService>(),
            _ => throw new ArgumentException("Invalid query service type")
        };
    }
}