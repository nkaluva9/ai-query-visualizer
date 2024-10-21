namespace AIQueryVisualizer.Services;

public abstract class QueryService<T> : IQueryService
{
    protected readonly HttpClient _httpClient;
    protected static string _chatId;
    protected IConfiguration _configuration;
    protected ILogger<T> _logger;
    protected QueryType _queryType;
    protected QueryService(HttpClient httpClient, IConfiguration configuration, ILogger<T> logger, QueryType queryType)
    {
        _configuration = configuration;
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger;
        SetDefaultRequestHeaders();
        _queryType = queryType;
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

        var chatPersona = new ChatPersona
        {
            Title = $"Data Intelligent Copilot @ {DateTime.Now}",
            Id = "",
            SystemDescription = _queryType == QueryType.Kql ? _configuration["LogAnalytics:SystemDescription"] : _configuration["CosmosDB:SystemDescription"]
        };
        var url = $"{_configuration["Chat:Endpoint"]}/chats";

        var requestBody = new
        {
            title = chatPersona.Title
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
            if (root.TryGetProperty("chatSession", out var chatSessionElement))
            {
                chatPersona.CreatedOn = chatSessionElement.TryGetProperty("createdOn", out var createdOnElement)
                    ? createdOnElement.GetDateTime()
                    : DateTime.Now;

                if (chatSessionElement.TryGetProperty("id", out var idElement))
                {
                    _chatId = idElement.GetString();
                    chatPersona.Id = _chatId;
                    await PatchChatPersonaAsync(chatPersona);
                    return _chatId;
                }
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
    private async Task PatchChatPersonaAsync(ChatPersona chatPersona)
    {
        var requestUri = $"{_configuration["Chat:Endpoint"]}/chats/{chatPersona.Id}";
        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri)
        {
            Content = JsonContent.Create(chatPersona)
        };
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Logger.LogInformation("Patch request successful");
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
                        return Utilities.ExtractKqlFromValue(value);
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