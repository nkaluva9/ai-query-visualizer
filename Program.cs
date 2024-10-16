using AIQueryVisualizer.Components;
using AIQueryVisualizer.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<KqlQueryService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var endpoint = configuration.GetValue<string>("Chat:Endpoint");
    client.BaseAddress = new Uri(endpoint);
});
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new CosmosClient(configuration["CosmosDB:Endpoint"], configuration["CosmosDB:Key"]);
});

builder.Services.AddSingleton(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    return cosmosClient.GetDatabase(configuration["CosmosDB:DatabaseId"]);
});

builder.Services.AddSingleton(sp =>
{
    var database = sp.GetRequiredService<Database>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    return database.GetContainer(configuration["CosmosDB:ContainerId"]);
});
builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var httpClient = serviceProvider.GetRequiredService<HttpClient>();
    var logger = serviceProvider.GetRequiredService<ILogger<KqlQueryService>>();
    return new KqlQueryService(httpClient, configuration,logger);
});
builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var httpClient = serviceProvider.GetRequiredService<HttpClient>();
    var logger = serviceProvider.GetRequiredService<ILogger<SqlQueryService>>();
    return new SqlQueryService(httpClient, configuration, serviceProvider.GetRequiredService<Container>(), logger);
});
builder.Services.AddScoped<QueryServiceFactory>();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
