var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapSwaggerEndpoints();

app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapGet("/health-points", async (IDaprStateStoreService daprStateStoreService,
                                   ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var healthPoints = await daprStateStoreService.GetStateAsync<int>(CommonConstants.CACHE_KEY_FREEZER_HEALTH_POINTS, CommonConstants.BINDING_NAME_CACHE, cancellationToken);
    return healthPoints;    
})
.WithName("HealthPoints")
.WithOpenApi();

app.MapPost("/attack-goku", async (Attack attack, DaprClient client,
                                     IDaprPubSubMessageService daprPubSubMessageService,
                                     ILoggerFactory loggerFactory,
                                     CancellationToken cancellationToken) =>
{
    // preparing strike energy
    Random random = new();
    int randomNumber = random.Next(1, 1001);

    var strike = attack ?? new Attack(randomNumber, Guid.NewGuid());
    strike.NumberOfAttack = attack!.NumberOfAttack ?? Guid.NewGuid();
    var logger = loggerFactory.CreateLogger("attack-goku");
    logger.LogInformation("Freezer attacks with {Energy} energy", strike.Energy);

    var response = await client.InvokeMethodAsync<Attack, AttackResult>(HttpMethod.Post, CommonConstants.APPID_GOKU, "damage-goku", attack, cancellationToken);

    logger.LogInformation("Goku has {HealthPoints} health points left", response.HealthPoints);

    var history = new AttackHistory("Freezer", "Goku", response.Damage, response.HealthPoints);

    await daprPubSubMessageService.PublishEventAsync(history, CommonConstants.TOPIC_BATTLE_HISTORY, CommonConstants.BINDING_NAME_PUBSUB, cancellationToken);

    return response;
})
.WithName("AttackGoku")
.WithOpenApi();

app.MapPost("/damage-freezer", async (Attack attack, HttpContext context, 
                              IDaprStateStoreService daprStateStoreService,
                              IDaprSignalRBindingService daprSignalRBindingService,
                              ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
{
    var logger = loggerFactory.CreateLogger("damage-freezer");
    logger.LogInformation("Receiving an attack {Energy} energy and hit number {Number}", attack.Energy, attack.NumberOfAttack);

    AttackResult result = new()
    {
        Damage = attack.Energy,
        HealthPoints = await SetStateAsync(attack)
    };

    await SignalRToClient($"Freezer receives an attack {result.Damage} energy. {result.HealthPoints} left");

    return result;

    // Method to save state before retries
    async Task<int> SetStateAsync(Attack attack)
    {
        var healthPoints = await daprStateStoreService.GetStateAsync<int>(CommonConstants.CACHE_KEY_FREEZER_HEALTH_POINTS, CommonConstants.BINDING_NAME_CACHE, cancellationToken);
        healthPoints -= attack.Energy;
        await daprStateStoreService.SaveStateAsync(healthPoints, CommonConstants.CACHE_KEY_FREEZER_HEALTH_POINTS, CommonConstants.BINDING_NAME_CACHE, cancellationToken);

        return healthPoints;
    }

    async Task SignalRToClient(string message)
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
})
.WithName("DamageFreezer")
.WithOpenApi();

app.MapDefaultEndpoints();

await app.RunAsync();