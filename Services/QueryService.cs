namespace AIQueryVisualizer.Services;

public abstract class QueryService<T> : IQueryService
{
    protected readonly HttpClient _httpClient;
    protected static string _chatId;
    protected IConfiguration _configuration;
    protected ILogger<T> _logger;
    protected QueryService(HttpClient httpClient, IConfiguration configuration, ILogger<T> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger;
        SetDefaultRequestHeaders();
    }
    protected ILogger<T> Logger => _logger;

    private void SetDefaultRequestHeaders()
    {
        var headersSection = _configuration.GetSection("Headers");
        foreach (var header in headersSection.GetChildren())
        {
            var keyName = header.Key;
            var keyValue = header.Value;
            if (!string.IsNullOrEmpty(keyName) && !string.IsNullOrEmpty(keyValue))
            {
                _httpClient.DefaultRequestHeaders.Add(keyName, keyValue);
            }
        }
    }
    public async Task<string> CreateChatSessionAsync()
    {
        if (!string.IsNullOrEmpty(_chatId))
        {
            return _chatId;
        }

        var url = $"{_configuration["Chat:Endpoint"]}/chats";

        var requestBody = new
        {
            title = "text to sql"
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, requestContent);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize response to get 'chatSession.id' property
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("chatSession", out var chatSessionElement) &&
                chatSessionElement.TryGetProperty("id", out var idElement))
            {
                _chatId = idElement.GetString();
                return _chatId;
            }

            Logger.LogInformation("'id' property not found in 'chatSession'.");
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> PostChatMessageAsync(string chatSessionId, string userInput)
    {
        var url = $"{_configuration["Chat:Endpoint"]}/chats/{chatSessionId}/messages";
        var requestBody = new
        {
            input = userInput,
            variables = new[]
            {
                new
                {
                    key = "messageType",
                    value = "0"
                }
            }
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, requestContent);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize response to get the 'input' field from 'variables' array
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("variables", out var variablesElement) && variablesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var variable in variablesElement.EnumerateArray())
                {
                    if (variable.TryGetProperty("key", out var keyElement) && keyElement.GetString() == "input" &&
                        variable.TryGetProperty("value", out var valueElement))
                    {
                        var value = valueElement.GetString();
                        var kqlStatement = Utilities.ExtractKqlFromValue(value);
                        return kqlStatement;
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError($"An error occurred: {ex.Message}");
            return null;
        }
    }
    public abstract Task<List<ResultRow>> ExecuteQueryAsync(string? statement);
}