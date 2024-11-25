using AspireDaprDemo.Common.Models;
using static Google.Api.ResourceDescriptor.Types;

var builder = WebApplication.CreateBuilder(args); 

builder.AddServiceDefaults();

var app = builder.Build();

app.MapSwaggerEndpoints();

app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapPost("/negotiate", async (IDaprSignalRBindingService daprSignalRBindingService,
                                ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("negotiate");
    logger.LogInformation("Negotiating SignalR");
    var metadata = new Dictionary<string, string>()
    {
        { "hub", CommonConstants.NOTIFICATION_HUBNAME },
    };

    var negotiateResponse = await daprSignalRBindingService.ClientNegotiateAsync(
        string.Empty,
        metadata: metadata,
        bindingName: CommonConstants.BINDING_NAME_SIGNALR,
        cancellationToken: cancellationToken
    );

    return negotiateResponse;
})
.WithName("Negotiate")
.WithOpenApi(); 

app.MapPost("/init-battle", async (int? healthPoints, IDaprStateStoreService daprStateStoreService,
                                ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("init-battle");
    logger.LogInformation("Init battle Goku vs Freezer. Each player has {HealthPoints}", healthPoints ?? 1000); 

    List<Task> tasks = [
        daprStateStoreService.SaveStateAsync(healthPoints ?? 1000, CommonConstants.CACHE_KEY_GOKU_HEALTH_POINTS, CommonConstants.BINDING_NAME_CACHE, cancellationToken),
        daprStateStoreService.SaveStateAsync(healthPoints ?? 1000, CommonConstants.CACHE_KEY_FREEZER_HEALTH_POINTS, CommonConstants.BINDING_NAME_CACHE, cancellationToken),
        daprStateStoreService.SaveStateAsync(new List<AttackHistory>(), CommonConstants.CACHE_KEY_BATTLE_HISTORY, CommonConstants.BINDING_NAME_CACHE, cancellationToken)
    ];

    await Task.WhenAll(tasks);
})
.WithName("InitBattle")
.WithOpenApi();

app.MapPost("/battle-history", async (AttackHistory attackHistory, IDaprStateStoreService daprStateStoreService,
                                ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("battle-history");
    logger.LogInformation("Updating battle history");
    await SetStateAsync(attackHistory);

    // Method to save state before retries
    async Task SetStateAsync(AttackHistory attackHistory)
    {
        var history = await daprStateStoreService.GetStateAsync<List<AttackHistory>>(CommonConstants.CACHE_KEY_BATTLE_HISTORY, CommonConstants.BINDING_NAME_CACHE, cancellationToken) 
                            ?? [];
        history.Add(attackHistory);
        await daprStateStoreService.SaveStateAsync(history, CommonConstants.CACHE_KEY_BATTLE_HISTORY, CommonConstants.BINDING_NAME_CACHE, cancellationToken);
    }

})
.WithName("UpdateBattleHistory")
.WithTopic(CommonConstants.BINDING_NAME_PUBSUB, CommonConstants.TOPIC_BATTLE_HISTORY)
.ExcludeFromDescription();

app.MapGet("/battle-history", async (IDaprStateStoreService daprStateStoreService,
                                ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("negotiate");
    logger.LogInformation("Negotiating SignalR");
    return await daprStateStoreService.GetStateAsync<List<AttackHistory>>(CommonConstants.CACHE_KEY_BATTLE_HISTORY, CommonConstants.BINDING_NAME_CACHE, cancellationToken);
})
.WithName("GetBattleHistory")
.WithOpenApi();

app.MapDefaultEndpoints();

app.Run();


