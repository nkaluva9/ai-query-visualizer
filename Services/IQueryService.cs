namespace AIQueryVisualizer.Services;

public interface IQueryService
{
    Task<string> CreateChatSessionAsync();
    Task<string?> PostChatMessageAsync(string chatSessionId, string userInput);
    Task<List<ResultRow>> ExecuteQueryAsync(string? statement);
}