using k8s.Models;

var builder = DistributedApplication.CreateBuilder(args);

var daprComponents = new DaprComponentOptions()
{
    LocalPath = Path.Combine("..", "dapr"),
}; 

var stateStore = builder.AddDaprStateStore("statestore", daprComponents);
var pubSub = builder.AddDaprPubSub("pubsub", daprComponents);
var signalr = builder.AddDaprComponent("signalr", "bindings.azure.signalr", daprComponents);
var queue = builder.AddDaprComponent("queue", "bindings.azure.storagequeues", daprComponents);

var tenkaiBudokaiApi = builder.AddProject<Projects.AspireDaprDemo_TenkaichiBudokaiService>("tenkaichibudokai")
    .WithDaprSidecar(new DaprSidecarOptions()
    {
        AppId = "tenkaichibudokai",
        ResourcesPaths = [Path.Combine("..", "dapr")]
    })
    .WithReference(stateStore)
    .WithReference(pubSub)
    .WithReference(signalr)
    .WithReference(queue);

var gokuApi = builder.AddProject<Projects.AspireDaprDemo_GokuService>("goku") 
     .WithDaprSidecar(new DaprSidecarOptions()
     {
         AppId = "goku",
         ResourcesPaths = [Path.Combine("..", "dapr")]
     })
    .WithReference(stateStore)
    .WithReference(pubSub)
    .WithReference(signalr)
    .WithReference(queue);

var freezerApi = builder.AddProject<Projects.AspireDaprDemo_FreezerService>("freezer")
    .WithDaprSidecar(new DaprSidecarOptions()
    {
        AppId = "freezer",
        ResourcesPaths = [Path.Combine("..", "dapr")]
    }) 
    .WithReference(stateStore)
    .WithReference(pubSub)
    .WithReference(signalr)
    .WithReference(queue);

builder.AddNpmApp("reactweb", "../battlefront", scriptName: "dev")
    .WithReference(tenkaiBudokaiApi)
    .WithReference(gokuApi)
    .WithReference(freezerApi);

builder.Build().Run();
