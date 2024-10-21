namespace AIQueryVisualizer.Services;

public class KqlQueryService(HttpClient httpClient, IConfiguration configuration, ILogger<KqlQueryService> logger)
    : QueryService<KqlQueryService>(httpClient, configuration, logger, QueryType.Kql )
{
    public override async Task<List<ResultRow>> ExecuteQueryAsync(string? statement)
    {
        try
        {
            var queryEndpoint = $"https://api.loganalytics.io/v1/workspaces/{configuration["LogAnalytics:WorkspaceId"]}/query";
            var requestBody = new
            {
                query = statement
            };
            AccessToken token;
            if (string.IsNullOrEmpty(_configuration["LogAnalytics:ClientId"]))
            {
                var credential = new DefaultAzureCredential();
                token = await credential.GetTokenAsync(new TokenRequestContext(["https://api.loganalytics.io/.default"]));
            }
            else
            {
                var credential = new ClientSecretCredential(_configuration["LogAnalytics:TenantId"], _configuration["LogAnalytics:ClientId"], _configuration["LogAnalytics:ClientSecret"]);
                token = await credential.GetTokenAsync(new TokenRequestContext(["https://api.loganalytics.io/.default"]));
            }

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, queryEndpoint);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            var rows = new List<ResultRow>();
            if (root.TryGetProperty("tables", out var tablesElement) && tablesElement.ValueKind == JsonValueKind.Array)
            {
                var table = tablesElement[0];
                var columns = table.GetProperty("columns");
                var rowsElement = table.GetProperty("rows");

                var columnNames = new List<string>();
                foreach (var column in columns.EnumerateArray())
                {
                    columnNames.Add(column.GetProperty("name").GetString());
                }

                foreach (var row in rowsElement.EnumerateArray())
                {
                    var rowData = new Dictionary<string, object>();
                    for (var i = 0; i < columnNames.Count; i++)
                    {
                        rowData[columnNames[i]] = row[i].ToString();
                    }
                    rows.Add(new ResultRow(rowData));
                }
            }
            return rows;
        }
        catch (Exception ex)
        {
            Logger.LogError($"An error occurred while executing KQL: {ex.Message}");
            return [];
        }
    }
}
