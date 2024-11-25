var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapSwaggerEndpoints();

app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapGet("/health-points", async (IDaprStateStoreService daprStateStoreService,
                                   ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var healthPoints = await daprStateStoreService.GetStateAsync<int>(CommonConstants.CACHE_KEY_GOKU_HEALTH_POINTS, CommonConstants.BINDING_NAME_CACHE, cancellationToken);
    return healthPoints;
})
.WithName("HealthPoints")
.WithOpenApi();

app.MapPost("/launch-genki-dama", async (GenkiDamaEnergy genkidama,
                                        IDaprStateStoreService daprStateStoreService,
                                        IDaprOutputBindingService daprOutputBindingService,
                                        IDaprPubSubMessageService daprPubSubMessageService,
                                        ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("launch-genki-dama");
    logger.LogInformation("Launching Genki Dama with {Energy} energy", genkidama.Energy);

    var healthpoints = await SetStateAsync(new Attack(genkidama.Energy, Guid.NewGuid()), daprStateStoreService, CommonConstants.CACHE_KEY_FREEZER_HEALTH_POINTS, cancellationToken);

    List<Task> tasks = new(genkidama.Energy);

    for (int energyAccumulated = 0; energyAccumulated < genkidama.Energy; energyAccumulated++)
    {
        var accumulateGenkiDamaEnergy = new AccumulateGenkiDamaEnergy(energyAccumulated);
        tasks.Add(daprOutputBindingService.PublishMessageAsync<AccumulateGenkiDamaEnergy>(accumulateGenkiDamaEnergy, 
                                                                                         CommonConstants.BINDING_NAME_QUEUE_STORAGE, cancellationToken));
    }

    await Task.WhenAll(tasks);

    var history = new AttackHistory("Goku", "Freezer", genkidama.Energy, healthpoints);
    await daprPubSubMessageService.PublishEventAsync(history, CommonConstants.TOPIC_BATTLE_HISTORY, CommonConstants.BINDING_NAME_PUBSUB, cancellationToken);

    return new AttackResult(damage: genkidama.Energy, healthPoints: healthpoints);
})
.WithName("LaunchGenkiDama")
.WithOpenApi();

app.MapPost("/process-genki-dama", async (AccumulateGenkiDamaEnergy accumulated,
                                          IDaprSignalRBindingService daprSignalRBindingService,
                                          ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("process-genki-dama");
    logger.LogInformation("Processing Genki Dama with {Energy} energy", accumulated.Energy);
    await SignalRToClient($"Processing Genki Dama with {accumulated.Energy} energy", daprSignalRBindingService, cancellationToken);
})
.WithName("ProcessGenkiDama")
.ExcludeFromDescription();

app.MapPost("/damage-goku", async (Attack attack, HttpContext context,
                                  IDaprStateStoreService daprStateStoreService,
                                  IDaprSignalRBindingService daprSignalRBindingService,
                                  ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("damage-goku");
    logger.LogInformation("Receiving an attack {Energy} energy and hit number {Number}", attack.Energy, attack.NumberOfAttack);

    AttackResult result = new()
    {
        Damage = attack.Energy,
        HealthPoints = await SetStateAsync(attack, daprStateStoreService, CommonConstants.CACHE_KEY_GOKU_HEALTH_POINTS, cancellationToken)
    };

    await SignalRToClient($"Goku receives an attack {result.Damage} energy. {result.HealthPoints} left", daprSignalRBindingService, cancellationToken);

    return result;
})
.WithName("DamageGoku")
.WithOpenApi();

app.MapPost("/attack-freezer", async (Attack attack, DaprClient client,
                                     IDaprPubSubMessageService daprPubSubMessageService,     
                                     ILoggerFactory loggerFactory, 
                                     CancellationToken cancellationToken) =>
{
    // preparing strike energy
    Random random = new();
    int randomNumber = random.Next(1, 1001);

    var strike = attack ?? new Attack(randomNumber, Guid.NewGuid());
    strike.NumberOfAttack = attack!.NumberOfAttack ?? Guid.NewGuid();
    var logger = loggerFactory.CreateLogger("attack-freezer");
    logger.LogInformation("Goku attacks with {Energy} energy", strike.Energy);
 
    var response = await client.InvokeMethodAsync<Attack, AttackResult>(HttpMethod.Post, CommonConstants.APPID_FREEZER, "damage-freezer", attack, cancellationToken);

    logger.LogInformation("Freezer has {HealthPoints} health points left", response.HealthPoints);

    var history = new AttackHistory("Goku", "Freezer", response.Damage, response.HealthPoints);

    await daprPubSubMessageService.PublishEventAsync(history, CommonConstants.TOPIC_BATTLE_HISTORY, CommonConstants.BINDING_NAME_PUBSUB, cancellationToken);

    return response;
})
.WithName("AttackFreezer")
.WithOpenApi();

async Task SignalRToClient(string message, IDaprSignalRBindingService daprSignalRBindingService, CancellationToken cancellationToken)
{
    const string target = "tenkaichibudokai-freezer";

    // SignalR to client
    var metadata = new Dictionary<string, string>()
        {
            { "hub", CommonConstants.NOTIFICATION_HUBNAME }
        };

    var messageText = new
    {
        message
    };

    DaprPayloadMessageSignalR messageSignalR = new()
    {
        Target = target,
        Arguments =
        [
            new()
                {
                    Sender = CommonConstants.NOTIFICATION_SIGNALR_SENDER,
                    Text = System.Text.Json.JsonSerializer.Serialize(messageText)
            }
        ]
    };

    await daprSignalRBindingService.PublishMessageAsync(messageSignalR, metadata, CommonConstants.BINDING_NAME_SIGNALR, cancellationToken);
}

// Method to save state before retries
async Task<int> SetStateAsync(Attack attack, IDaprStateStoreService daprStateStoreService, string cacheKey, CancellationToken cancellationToken)
{
    var healthPoints = await daprStateStoreService.GetStateAsync<int>(cacheKey, CommonConstants.BINDING_NAME_CACHE, cancellationToken);
    healthPoints -= attack.Energy;
    await daprStateStoreService.SaveStateAsync(healthPoints, cacheKey, CommonConstants.BINDING_NAME_CACHE, cancellationToken);

    return healthPoints;
}

app.MapDefaultEndpoints();

await app.RunAsync();