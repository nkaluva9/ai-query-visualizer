using Microsoft.Azure.Cosmos;

namespace AIQueryVisualizer.Services;

public class SqlQueryService(HttpClient httpClient, IConfiguration configuration, Container container, ILogger<SqlQueryService> logger)
    : QueryService<SqlQueryService>(httpClient, configuration, logger, QueryType.Sql)
{
    public override async Task<List<ResultRow>> ExecuteQueryAsync(string? statement)
    {
        try
        {
            var query = new QueryDefinition(statement);
            var queryResultSetIterator = container.GetItemQueryIterator<dynamic>(query);
            var results = new List<ResultRow>();

            while (queryResultSetIterator.HasMoreResults)
            {
                var response = await queryResultSetIterator.ReadNextAsync();

                foreach (var item in response)
                {
                    var rowData = new Dictionary<string, object>();
                    foreach (var property in item)
                    {
                        rowData[property.Name] = property.Value.ToString();
                    }

                    results.Add(new ResultRow(rowData));
                }
            }
            return results;
        }
        catch (CosmosException cosmosEx)
        {
            Logger.LogError($"Cosmos DB query error: {cosmosEx.Message}");
            return [];
        }
        catch (Exception ex)
        {
            Logger.LogError($"An error occurred: {ex.Message}");
            return [];
        }
    }
}
