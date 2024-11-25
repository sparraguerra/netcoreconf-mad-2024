public interface IDaprPubSubMessageService
{
    Task PublishEventAsync<T>(T message, string topicName, string pubSubName, CancellationToken cancellationToken);
    Task PublishEventAsync<T>(T message, Dictionary<string, string>? metadata, string topicName, string pubSubName, CancellationToken cancellationToken);
}

public class DaprPubSubMessageService : IDaprPubSubMessageService
{
    private readonly DaprClient client;
    private readonly ILogger<DaprPubSubMessageService> logger;

    public DaprPubSubMessageService(DaprClient client, ILogger<DaprPubSubMessageService> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task PublishEventAsync<T>(T message, string topicName, string pubSubName, CancellationToken cancellationToken) =>
        await PublishEventAsync(message, new Dictionary<string, string>() { { "rawPayload", "false" } }, topicName, pubSubName, cancellationToken);

    public async Task PublishEventAsync<T>(T message, Dictionary<string, string>? metadata, string topicName, string pubSubName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing event {@Event} to {PubsubName}.{TopicName}", message, pubSubName, topicName);

        await client.PublishEventAsync(pubSubName, topicName, message, metadata, cancellationToken);
    }
}
