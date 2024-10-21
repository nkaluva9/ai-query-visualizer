
# AI Query Visualizer (KQL and SQL)

## Overview
The **AI Query Visualizer** demo showcases how you can use OpenAI GPT-4o to convert user prompts into KQL queries and execute them against a Log Analytics workspace. It also supports SQL (SQL API) queries, offering powerful query capabilities from a simple user prompt. The demo leverages a system description to set a meta prompt (persona) for the chat session. Make sure to adjust the system description fields based on your data source and expected output. For chart display, it should return at least two columns, with one column being numeric; otherwise, it will display the result in a grid format.
## How to Run This Sample

### How to Configure OpenAI Chat Application
To run this demo, follow the steps below to configure the OpenAI chat application:

1. **Clone the repository** from [GitHub](https://github.com/microsoft/chat-copilot):
   ```bash
   git clone https://github.com/microsoft/chat-copilot
   ```

2. **Open** the `CopilotChat.sln` solution in your preferred IDE.
3. **Navigate to** the `CopilotChatWebApi` project.
4. **Open** the `appsettings.json` file.
5. **Update** the `AzureOpenAIText` and `AzureOpenAIEmbedding` values under the `KernelMemory` section.
6. **Launch** the `CopilotChatWebApi` project.

### Configuring AIQueryVisualizer Project
To configure the AIQueryVisualizer project, follow these steps:

1. **Clone the repository** from [GitHub](https://github.com/nhcloud/ai-query-visualizer):
   ```bash
   git clone https://github.com/nhcloud/ai-query-visualizer
   ```

2. **Open** the `AIQueryVisualizer.sln` solution in your preferred IDE.
3. **Open** the `appsettings.json` file.
4. **Update** the following values under the appropriate sections:
    - `LogAnalytics` values:
        ```json
        "LogAnalytics": {
            "WorkspaceId": "[WorkspaceId]",
            "TenantId": "[TenantId]",
            "ClientId": "[ClientId]",
            "ClientSecret": "[ClientSecret]",
            "SystemDescription": "[Meta Prompt descritpion.  ex., You are a helpful bot that generates KQL queries using the `AppPageViews` table. The available columns in this table are: `TenantId`, `TimeGenerated`, `Name`, `Url`, `DurationMs`, `PerformanceBucket`, `OperationName`, `OperationId`, `ParentId`, `UserId`, `ClientType`, `ClientOS`, `ClientIP`, `ClientCity`, `ClientStateOrProvince`, `ClientCountryOrRegion`, and `ClientBrowser`.\n\nWhen constructing queries:\n\n- Use the `summarize` operator for aggregation tasks such as counting rows or summarizing data. Avoid using the `extend` operator for such purposes.\n- Always name the result of the `count()` function as `Count`.\n- Only project the required columns based on the user's input or task.\n\nThe `extend` operator is for creating new calculated fields or modifying existing fields. However, do not use the `over` keyword in conjunction with the `extend` operator.]"
       }
        ```
    - `CosmosDB` values:
        ```json
        "CosmosDB": {
            "Endpoint": "[CosmosDB Endpoint]",
            "Key": "[Key]",
            "DatabaseId": "[DatabaseId]",
            "ContainerId": "[ContainerName]",
            "SystemDescription": "[Meta Prompt descritpion.  ex., You are a helpful bot that generates Azure Cosmos DB SQL (SQL API) queries using the user's container. The documents in the container follow a structure with the following column names:\n{\n  \"PartitionKey\",\n  \"RowKey\",\n  \"FirstName\",\n  \"LastName\",\n  \"Email\",\n  \"Department\",\n  \"id\",,\n  \"_ts\"\n}\n\nWhen generating queries, always project only the required columns based on user input. Null check should happen using null, example IS Null should use =null"
        }
        ```
    ## Keyless Connection Mode

    To leverage **keyless connection** mode (with Azure local authentication disabled):

    - For **Log Analytics**, Leave the `TenantId`, `ClientId`, and `ClientSecret` fields empty.
    - For **CosmosDB**, Leave the `Key` field empty.

    In both cases, the connection will be established using `AzureDefaultCredentials`. Ensure that the app identity has the required permissions granted.
5. **Launch** the AIQueryVisualizer project.

Now, you can experiment with both **KQL** and **SQL** (SQL API) queries by interacting with the app's interface.

## Technologies Used
- **C# / .NET Core**: Backend logic and query execution.
- **Blazor**: For building interactive UI components.
- **Azure OpenAI**: For processing user prompts and generating KQL and SQL queries.
- **Azure Speech Service**: For processing user prompt from speech to text.
- **Log Analytics**: For executing KQL queries.
- **CosmosDB**: For executing SQL queries.

## License
This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
