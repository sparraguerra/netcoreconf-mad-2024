namespace AspireDaprDemo.Common.Constants;

public static class CommonConstants
{
    public const string APPID_GOKU = "goku";
    public const string APPID_FREEZER = "freezer";
    public const string APPID_TENKAICHIBUDOKAI = "tenkaichibudokai";

    public const string BINDING_NAME_CACHE = "statestore";
    public const string BINDING_NAME_SIGNALR = "signalr";
    public const string BINDING_NAME_QUEUE_STORAGE = "queue";
    public const string BINDING_NAME_PUBSUB = "pubsub";

    public const string NOTIFICATION_FROM_SYSTEM = "TenkaichiBudokai";
    public const string NOTIFICATION_HUBNAME = "tenkaichibudokai";
    public const string NOTIFICATION_SIGNALR_TARGET_MESSAGE = "ReceiveEvent";
    public const string NOTIFICATION_SIGNALR_SENDER = "dapr";

    public const string CACHE_KEY_GOKU_HEALTH_POINTS = "goku:hp";
    public const string CACHE_KEY_FREEZER_HEALTH_POINTS = "freezer:hp";
    public const string CACHE_KEY_BATTLE_HISTORY = "battle:history";

    public const string TOPIC_BATTLE_HISTORY = "battle-history";
}
