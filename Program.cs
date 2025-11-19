using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Blaze3SDK;
using BlazeCommon;
using NLog;
using NLog.Layouts;
using Tdf;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Zamboni;
using Zamboni11.Components.Blaze;
using Zamboni11.Components.NHL11;

namespace Zamboni11;

internal class Program
{
    public const string Name = "Zamboni11 1.0";
    public const int RedirectorPort = 42127;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static ZamboniConfig ZamboniConfig;
    public static Database Database;

    public static readonly string PublicIp = new HttpClient().GetStringAsync("https://checkip.amazonaws.com/").GetAwaiter().GetResult().Trim();
    public static string GameServerIp;

    private static async Task Main(string[] args)
    {
        InitConfig();
        StartLogger();
        InitDatabase();
        GameServerIp = ZamboniConfig.GameServerIp.Equals("auto") ? PublicIp : ZamboniConfig.GameServerIp;

        var commandTask = Task.Run(StartCommandListener);
        var redirectorTask = StartRedirectorServer();
        var coreTask = StartCoreServer();
        var apiTask = new Api().StartAsync();
        Logger.Warn(Name + " started");
        await Task.WhenAll(redirectorTask, coreTask, commandTask, apiTask);
    }

    private static void StartLogger()
    {
        var logLevel = LogLevel.FromString(ZamboniConfig.LogLevel);
        var layout = new SimpleLayout("[${longdate}][${callsite-filename:includeSourcePath=false}(${callsite-linenumber})][${level:uppercase=true}]: ${message:withexception=true}");
        LogManager.Setup().LoadConfiguration(builder =>
        {
            builder.ForLogger().FilterMinLevel(logLevel)
                .WriteToConsole(layout)
                .WriteToFile("logs/server-${shortdate}.log", layout);
        });
    }

    private static async Task StartRedirectorServer()
    {
        var certBytes = File.ReadAllBytes("gosredirector_mod.pfx");
        X509Certificate cert = new X509Certificate2(certBytes, "123456");

        var redirector = Blaze3.CreateBlazeServer("RedirectorServer", new IPEndPoint(IPAddress.Any, RedirectorPort), cert);
        redirector.AddComponent<RedirectorComponent>();
        await redirector.Start(-1).ConfigureAwait(false);
    }

    private static void InitConfig()
    {
        const string configFile = "zamboni-config.yml";
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        if (!File.Exists(configFile))
        {
            ZamboniConfig = new ZamboniConfig();
            var yaml = serializer.Serialize(ZamboniConfig);

            var comments = "# GameServerIp: 'auto' = automatically detect public IP or specify a manual IP address, where GameServer is run on\n" +
                           "# GameServerPort: Port for GameServer to listen on. (Redirector server lives on " + RedirectorPort + ", clients request there)\n" +
                           "# LogLevel: Valid values: Trace, Debug, Info, Warn, Error, Fatal, Off.\n" +
                           "# DatabaseConnectionString: Connection string to PostgreSQL, for saving data. (Not required)\n\n";
            File.WriteAllText(configFile, comments + yaml);
            Logger.Warn("Config file created: " + configFile);
            return;
        }

        var yamlText = File.ReadAllText(configFile);
        ZamboniConfig = deserializer.Deserialize<ZamboniConfig>(yamlText);
    }

    private static void InitDatabase()
    {
        Database = new Database();
    }

    private static async Task StartCoreServer()
    {
        var tdfFactory = new TdfFactory();
        var config = new BlazeServerConfiguration("CoreServer", new IPEndPoint(IPAddress.Any, ZamboniConfig.GameServerPort), tdfFactory.CreateEncoder(true), tdfFactory.CreateDecoder(true));
        var core = new ZamboniCoreServer(config);

        core.AddComponent<UtilComponent>();
        core.AddComponent<AuthenticationComponent>();
        core.AddComponent<UserSessionsComponent>();
        core.AddComponent<RoomsComponent>();
        core.AddComponent<MessagingComponent>();
        core.AddComponent<LeagueComponent>();
        core.AddComponent<CensusDataComponent>();
        core.AddComponent<ClubsComponent>();
        core.AddComponent<StatsComponent>();
        core.AddComponent<GameManager>();
        core.AddComponent<GameReportingLegacyComponent>();


        //2250 NHL SPECIFIC COMPONENT
        // core.AddComponent<OsdkDynamicMessagingComponent>(); 
        //We are intentionally not creating this component. I haven't yet found a MessageResponse
        //that doesn't crash the client. The TDF Tags and Types should be all correct.
        //The client can live without this

        //2249 NHL SPECIFIC COMPONENT
        core.AddComponent<TwoTwoFourNineComponent>();

        //2148 NHL SPECIFIC COMPONENT
        //"Hut" Component, not official name
        //Work in progress
        core.AddComponent<HutComponent>();
        await core.Start(-1).ConfigureAwait(false);
    }

    private static void StartCommandListener()
    {
        Logger.Info("Type 'help' or 'status'.");

        while (true)
        {
            var input = ReadLine.Read();
            if (string.IsNullOrWhiteSpace(input))
                continue;

            switch (input.Trim().ToLowerInvariant())
            {
                case "help":
                    Logger.Warn("Available commands: help, status");
                    break;

                case "status":
                    Logger.Info(Name);
                    Logger.Info("Server running on ip: " + GameServerIp + " (" + PublicIp + ")");
                    Logger.Info("GameServerPort port: " + ZamboniConfig.GameServerPort);
                    Logger.Info("Redirector port: " + RedirectorPort);
                    Logger.Info("Online Players: " + ServerManager.GetServerPlayers().Count);
                    foreach (var serverPlayer in ServerManager.GetServerPlayers()) Logger.Info(serverPlayer.UserIdentification.mName);
                    Logger.Info("Queued Total Players: " + ServerManager.GetQueuedPlayers().Count);
                    foreach (var queuedPlayer in ServerManager.GetQueuedPlayers()) Logger.Info(queuedPlayer.ServerPlayer.UserIdentification.mName);
                    Logger.Info("Server Games: " + ServerManager.GetServerGames().Count);
                    foreach (var serverGame in ServerManager.GetServerGames()) Logger.Info(serverGame);
                    break;

                default:
                    Logger.Info($"Unknown command: {input}");
                    break;
            }
        }
    }
}