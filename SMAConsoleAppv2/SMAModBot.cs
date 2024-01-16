using System;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

// Twitch IRC Bot.
public class SMAModBot
{
    static void WL(object o) => Console.WriteLine(o);
    public TwitchClient twitchClient;
    TwitchAPI api;

    // High alert account names get recorded here.
    HashSet<string> highAlertAccountNames;

    public enum ModBotMode {
        // Regular operating mode. Print user joins and user messages.
        Normal, 
        // Print user joins.
        MonitorAccounts,  
        // Prints user joins. The user's acct age is compared again a TimeSpan to decide if user is HIGH or LOW alert. Alert level is printed along with user join message.
        MonitorNewAccounts, 
        // Same as above but only prints HIGH alert accounts.
        MonitorNewHighAlertAccounts
    }

    public SMAModBot(TwitchAPI twitchApi, string twitchUsername, string channelName, ModBotMode botMode, TimeSpan? acctCreationTimeSpan = null)
    {
        api = twitchApi;
        highAlertAccountNames = new();
        ConnectionCredentials cred = new(twitchUsername, SMAAuth.AuthToken);
        ClientOptions options = new(){
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        WebSocketClient webSocketClient = new(options);
        twitchClient = new(webSocketClient);
        twitchClient.Initialize(cred, channelName);
        WL($"ModBot initialized.");
        twitchClient.OnConnected += (sender, o) => {
            WL($"{o.BotUsername} connected to {channelName} chat.");
        };

        if(botMode == ModBotMode.Normal)
        {
            twitchClient.OnMessageReceived += OnMessageReceived;
            twitchClient.OnUserJoined += async (sender, o) => {
                var user = await api.Helix.Users.GetUsersAsync(
                    null, new List<string>(){o.Username}, null);
                WL($"{o.Username} joined. Created {user.Users[0].CreatedAt.ToLocalTime()}.");
            };
        }
        else if(botMode == ModBotMode.MonitorAccounts)
        {
            twitchClient.OnUserJoined += async (sender, o) => {
                var user = await api.Helix.Users.GetUsersAsync(
                    null, new List<string>(){o.Username}, null);
                WL($"{o.Username} joined. Created {user.Users[0].CreatedAt.ToLocalTime()}.");
            };
        }
        else if(botMode == ModBotMode.MonitorNewAccounts)
        {
            twitchClient.OnUserJoined += async (sender, o) => {
                var user = await api.Helix.Users.GetUsersAsync(
                    null, new List<string>(){o.Username}, null);
                DateTime createdAt = user.Users[0].CreatedAt.ToLocalTime();
                bool flagAccount = (DateTime.Now-createdAt)<acctCreationTimeSpan;
                if(flagAccount) highAlertAccountNames.Add(o.Username);
                string alertLevel = flagAccount ? "HIGH" : "LOW";
                WL($"{o.Username} joined. Created {user.Users[0].CreatedAt}. Alert Level: {alertLevel}");
                // if(createdAt)
            };
        }
        else if(botMode == ModBotMode.MonitorNewHighAlertAccounts)
        {
            twitchClient.OnUserJoined += async (sender, o) => {
                var user = await api.Helix.Users.GetUsersAsync(
                    null, new List<string>(){o.Username}, null);
                DateTime createdAt = user.Users[0].CreatedAt.ToLocalTime();
                bool flagAccount = (DateTime.Now-createdAt)<acctCreationTimeSpan;
                if(flagAccount)
                {
                    highAlertAccountNames.Add(o.Username);
                    WL($"{o.Username} joined. Created {user.Users[0].CreatedAt}. Alert Level: HIGH");
                }
                // if(createdAt)
            };
        }

        
        // twitchClient.Connect();
        // twitchClient = new()
    }

    public void Start() => twitchClient.Connect();
    public void Stop() => twitchClient.Disconnect();

    // Call this method on the instance to get a list of comma separated names.
    public void PrintHighAlertAccountNames()
    {
        WL(string.Join(",", highAlertAccountNames));
    }

    void OnMessageReceived(object? sender, OnMessageReceivedArgs o)
    {
        WL($"{o.ChatMessage.Username}: {o.ChatMessage.Message}");
    }

}