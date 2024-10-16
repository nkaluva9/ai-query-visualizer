
# AI Query Visualizer (KQL and SQL)

## Overview
The **AI Query Visualizer** demo showcases how you can use OpenAI GPT-4 to convert user prompts into KQL queries and execute them against a Log Analytics workspace. It also supports SQL (SQL API) queries, offering powerful query capabilities from a simple user prompt.

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
            "ClientSecret": "[ClientSecret]"
        }
        ```
    - `CosmosDB` values:
        ```json
        "CosmosDB": {
            "Endpoint": "[CosmosDB Endpoint]",
            "Key": "[Key]",
            "DatabaseId": "[DatabaseId]",
            "ContainerId": "[ContainerName]"
        }
        ```
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
