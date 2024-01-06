using System.Runtime.InteropServices;
using TwitchLib;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Moderation.BanUser;
using TwitchLib.Communication.Models;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Handler.Channel.ChannelPoints.Redemptions;
using TwitchLib.PubSub.Events;

static void WL(object o) => Console.WriteLine(o);

/* 
Each of these code blocks is a separate example. Uncomment a code 
block to use it. When using one code block, make sure the others 
are commented out, otherwise undefined behaviour can occur.
*/
// ***************************************************************
// Example01 - Getting auth data
// Task authTsk = SMAAuth.GetAuthData();

// ConsoleKeyInfo keyInfo = new();
// while(true)
// {
//     if(Console.KeyAvailable) keyInfo = Console.ReadKey();
//     if(authTsk.IsCompleted) { await authTsk; break; }
//     if(keyInfo.Key == ConsoleKey.Q) break;
// }
// return;
// ***************************************************************


// ***************************************************************
// Example02 - Getting users
// SMAAuth.Init();
// TwitchAPI api = new();
// api.Settings.ClientId = SMAAuth.ClientId;
// api.Settings.AccessToken = SMAAuth.AuthToken;

// await SMAModHelper.GetAndPrintUsers(
//     api, new List<string>(){"nightbot","twitchdev"}, null, false);
// return;
// ***************************************************************

// ***************************************************************
// Example03 - Getting chatters.
// Note: Add your own channel ID when calling GetAndPrintChatters.
// SMAAuth.Init();
// TwitchAPI api = new();
// api.Settings.ClientId = SMAAuth.ClientId;
// api.Settings.AccessToken = SMAAuth.AuthToken;

// await SMAModHelper.GetAndPrintChatters(
//     api, "123", "123", null, null, false
// );
// return;

// ***************************************************************

// ***************************************************************
// Example04 - Getting chatters created after certain date.
// Note: Add your own channel ID when calling GetAndPrintChatters.
// SMAAuth.Init();
// TwitchAPI api = new();
// api.Settings.ClientId = SMAAuth.ClientId;
// api.Settings.AccessToken = SMAAuth.AuthToken;

// // Get chatters, and filter results to only show accounts created
// // after 01 Jan 2023.
// await SMAModHelper.GetAndPrintChatters(
//     api, "123", "123", null, new DateTime(2021,1,1), true
// );
// return;
// ***************************************************************
