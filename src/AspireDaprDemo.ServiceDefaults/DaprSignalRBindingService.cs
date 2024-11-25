public class DaprPayloadMessageSignalR
{
    public string Target { get; set; } = string.Empty;
    public Argument[]? Arguments { get; set; }
}

public class Argument
{
    public string Sender { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public record ClientNegotiationResult(string AccessToken, string Url);

public interface IDaprSignalRBindingService : IDaprOutputBindingService
{
    Task<ClientNegotiationResult> ClientNegotiateAsync<T>(T? data, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken);
}

public class DaprSignalRBindingService : DaprOutputBindingService, IDaprSignalRBindingService
{
    private readonly DaprClient client;
    private readonly ILogger<DaprOutputBindingService> logger;

    public DaprSignalRBindingService(DaprClient client, ILogger<DaprSignalRBindingService> logger)
        : base(client, logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task<ClientNegotiationResult> ClientNegotiateAsync<T>(T? data, IReadOnlyDictionary<string, string>? metadata, 
                                                                       string bindingName, CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Getting negotiate to {BindingName}", bindingName);

        return await client.InvokeBindingAsync<T, ClientNegotiationResult>(
            bindingName,
            "clientNegotiate",
            data,
            metadata,
            cancellationToken
        );
    }
}