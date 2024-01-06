// Moderation related helper methods.
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Helix.Models.Chat.GetChatters;
using TwitchLib.Api;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.Moderation.BanUser;

public static class SMAModHelper
{
    static void WL(object o) => Console.WriteLine(o);

    /*
    Retreives and prints info about the users specified in 'names'. paginationCursor can be optionally specified. If filterByCreatedAfter is true, users list will only include users account created after a date specified by createdAfter.
    */
    public static async Task GetAndPrintUsers(
        TwitchAPI api, List<string> names, DateTime? createdAt = null, bool filterByCreatedAt = false)
    {
        GetUsersResponse users = await api.Helix.Users.GetUsersAsync(
            null, names, null
        );
        string usersListStr = "";
        foreach(var user in users.Users)
        {
            if(filterByCreatedAt)
            {
                if(user.CreatedAt > createdAt)
                    usersListStr += $"Name: {user.Login} | ID: {user.Id} | Created: {user.CreatedAt}\n";
            }
            else
                usersListStr += $"Name: {user.Login} | ID: {user.Id} | Created: {user.CreatedAt}\n";
        }
        WL(usersListStr);
        // await Task.Run(()=>{});
    }
    
    // This method isn't really used anymore since you can accomplish the same thing with GetAndPrintUser method.
    public static async Task FilterUsersByDate(
        TwitchAPI api, List<string> logins, DateTime createdAfter
    )
    {
        GetUsersResponse users = await api.Helix.Users.GetUsersAsync(
            null, logins, null
        );
        foreach(var user in users.Users)
        {
            if(user.CreatedAt > createdAfter)
                WL($"{user.Login}, {user.CreatedAt}");
        }
    }

    /*
    Retreives and prints a list of the current chatters. paginationCursor can be optionally specified. If filterByCreatedAfter is true, chatters list will only include chatters account created after a date specified by createdAfter.
    */
    public static async Task GetAndPrintChatters(
        TwitchAPI api, string broadcaster_id, string mod_id, string? paginationCursor = null, DateTime? createdAfter = null, bool filterByCreatedAfter = false
    )
    {
        GetChattersResponse chatters = await api.Helix.Chat.GetChattersAsync(
            broadcaster_id, mod_id, 100, paginationCursor, null
        );
        List<string> logins = new List<string>();
        foreach(var chatter in chatters.Data)
        {
            logins.Add(chatter.UserLogin);
        }

        GetUsersResponse users = await api.Helix.Users.GetUsersAsync(
            null, logins, null
        );
        if(users.Users.Length > 0) WL("Chatters list:");
        string chattersListStr = "";
        foreach(var user in users.Users)
        {
            if(filterByCreatedAfter)
            {
                if(user.CreatedAt > createdAfter)
                    chattersListStr += $"{user.Login}, {user.CreatedAt}\n";
            }
            else
                chattersListStr += $"{user.Login}, {user.CreatedAt}\n";
        }
        WL(chattersListStr);
        WL($"Pagination Cursor: {chatters.Pagination.Cursor}");

    }

    /*
    Mass bans chatters with accounts that were created after 
    'createdAfter' date. 'confirmEachBan' can be set to true if you
    want to confirm before each ban. Prior to starting mass ban, 
    method asks twice for confirmation, just so you can be sure that
    you want to ban. 'paginationCursor' can be used if chatters
    list is longer than one page.
    NOTE: The line that calls BanUserAsync has been commented out
    for the time being. I'm yet to get into a situation where I have
    to test something like this. So for the time being it just says
    "Ok ban". Uncomment it at your own risk.
    */
    public static async Task MassBanChattersByCreatedAfterDate(
        bool confirmEachBan, TwitchAPI api, string broadcaster_id, 
        string mod_id, string? paginationCursor = null, 
        DateTime? createdAfter = null
    )
    {
        GetChattersResponse chatters = await api.Helix.Chat.GetChattersAsync(
            broadcaster_id, mod_id, 100, paginationCursor, null
        );
        List<string> logins = new List<string>();
        foreach(var chatter in chatters.Data)
        {
            logins.Add(chatter.UserLogin);
        }

        GetUsersResponse users = await api.Helix.Users.GetUsersAsync(
            null, logins, null
        );
        if(users.Users.Length > 0) WL("Chatters list:");
        string chattersListStr = "";
        List<User> banUsersList = new();
        foreach(var user in users.Users)
        {
            if(user.CreatedAt > createdAfter)
            {
                chattersListStr += $"{user.Login}, {user.CreatedAt}\n";
                banUsersList.Add(user);
            }
        }
        WL(chattersListStr);
        WL($"Pagination Cursor: {chatters.Pagination.Cursor}");

        WL("Ban all listed chatters? (y/n)");
        if(Console.ReadKey().Key != ConsoleKey.Y) return;

        WL("Are you sure? (y/n)");
        if(Console.ReadKey().Key != ConsoleKey.Y) return;

        foreach(var user in banUsersList)
        {
            bool shouldBan = false;
            if(confirmEachBan)
            {
                WL($"Ban {user.Login}? (y/n)");
                shouldBan = Console.ReadKey().Key == ConsoleKey.Y;
            }

            if(shouldBan)
            {
                // await api.Helix.Moderation.BanUserAsync(
                //     broadcaster_id, mod_id, new BanUserRequest {
                //         UserId=user.Id, Reason="Mass ban."
                //     }
                // );
                WL("Ok ban.");
            }
        }


    }

    // Takes a comma-separated string of values and generates the C# List<string> initializer string from it. That string can then be copy-pasted into the code for generating a List<string>.
    public static string ListStringFromCommaString(string commaString)
    {
        string s = "{";
        string[] sepStrings = commaString.Split(new char[]{','});
        for(int i=0; i<sepStrings.Length; i++)
        {
            s += $"\"{sepStrings[i]}\"";
            if(i < sepStrings.Length-1) s += ",";
        }
        s += "}";
        return s;
    }


}