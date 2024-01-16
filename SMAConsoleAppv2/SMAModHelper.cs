// Moderation related helper methods.
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Helix.Models.Chat.GetChatters;
using TwitchLib.Api;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.Moderation.BanUser;
using TwitchLib.Api.Helix.Models.Channels.GetChannelFollowers;

public static class SMAModHelper
{
    static void WL(object o) => Console.WriteLine(o);

    #region GetInfo
    /*
    Retreives and prints info about the users specified in 'names'. Use userlogin, not display name. paginationCursor can be optionally specified. If filterByCreatedAfter is true, users list will only include users account created after a date specified by createdAfter.
    If 'printNameListStr' is true, prints a comma separated list of logins as well, which can be used for coding purposes.
    If 'printIDListStr' is true, prints a comma separated list of IDs as well, which can be used for coding purposes.
    */
    public static async Task GetAndPrintUsers(
        TwitchAPI api, List<string> names, DateTime? createdAt = null, bool filterByCreatedAt = false, bool printNameListStr = false, bool printIdListStr = false)
    {
        GetUsersResponse users = await api.Helix.Users.GetUsersAsync(
            null, names, null
        );
        List<User> filteredUsers = new();
        foreach(var user in users.Users)
        {
            if(filterByCreatedAt)
            {
                if(user.CreatedAt > createdAt)
                    filteredUsers.Add(user);
            }
            else
                filteredUsers.Add(user);
                
        }
        string usersListStr = "";
        string nameListStr = "";
        string idListStr = "";
        foreach(var user in filteredUsers)
        {
            // user.CreatedAt is in UTC, so convert to local time.
            DateTime createdAtDate = user.CreatedAt.ToLocalTime();
            usersListStr += $"Login: {user.Login} | ID: {user.Id} | Created: {createdAtDate}\n";
            if(printNameListStr) nameListStr += $"{user.Login}, ";
            if(printIdListStr) idListStr += $"{user.Id}, ";
        }
        WL(usersListStr);
        if(printNameListStr) WL(nameListStr);
        if(printIdListStr) WL(idListStr);
        // await Task.Run(()=>{});
    }

    /*
    NOTE: Requires 'Twitch.Api' 3.9.0 or above. Older versions don't have some of the required endpoints implemented.

    Gets and prints a list of broadcaster's followers. 
    If 'filterByFollowedAfter' is true, list only contains users that follower after 'followedAfter' date and time. Dates are in local timezone.
    If 'printNameListStr' is true, prints a comma separated list of logins as well, which can be used for coding purposes.
    If 'printIDListStr' is true, prints a comma separated list of IDs as well, which can be used for coding purposes.
    */
    public static async Task GetAndPrintFollowers(
        TwitchAPI api, string broadcaster_id, string? paginationCursor = null, DateTime? followedAfter = null, bool filterByFollowedAfter = false, bool printNameListStr = false, bool printIDListStr = false)
    {
        var followers = await api.Helix.Channels.GetChannelFollowersAsync(
            broadcaster_id, null, 100, paginationCursor, null
        );
        List<ChannelFollower> filteredFollowers = new();
        foreach(var follower in followers.Data)
        {
            DateTime followedAt = DateTime.Parse(follower.FollowedAt);
            if(filterByFollowedAfter)
            {
                if(followedAt > followedAfter) 
                    filteredFollowers.Add(follower);
            }
            else
                filteredFollowers.Add(follower);
        }
        string followersStr = "";
        string nameListStr = "";
        string idListStr = "";
        foreach(var follower in filteredFollowers)
        {
            DateTime followedAt = DateTime.Parse(follower.FollowedAt);
            followersStr += $"{follower.UserName}, {follower.UserLogin}, {followedAt}\n";
            if(printNameListStr) nameListStr += $"{follower.UserLogin}, ";
            if(printIDListStr) idListStr += $"{follower.UserId}, ";
        }

        WL(followersStr);
        if(printNameListStr) WL(nameListStr);
        if(printIDListStr) WL(idListStr);
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
    If 'printNameListStr' is true, prints a comma separated list of logins as well, which can be used for coding purposes.
    If 'printIDListStr' is true, prints a comma separated list of IDs as well, which can be used for coding purposes.
    */
    public static async Task GetAndPrintChatters(
        TwitchAPI api, string broadcaster_id, string mod_id, string? paginationCursor = null, DateTime? createdAfter = null, bool filterByCreatedAfter = false, bool printNameListStr = false, bool printIDListStr = false
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
        List<User> filteredUsers = new();
        foreach(var user in users.Users)
        {
            if(filterByCreatedAfter)
            {
                if(user.CreatedAt > createdAfter) filteredUsers.Add(user);
                    // chattersListStr += $"{user.Login}, {user.CreatedAt}\n";
            }
            else
                filteredUsers.Add(user);
        }

        string chattersListStr = "";
        string nameListStr = "";
        string idListStr = "";
        foreach(var user in filteredUsers)
        {
            // user.CreatedAt is in UTC, so convert to local time.
            DateTime createdAtDate = user.CreatedAt.ToLocalTime();
            chattersListStr += $"{user.Login}, {createdAtDate}\n";
            if(printNameListStr) nameListStr += $"{user.Login}, ";
            if(printIDListStr) idListStr += $"{user.Id}, ";
        }
        WL(chattersListStr);
        WL($"Pagination Cursor: {chatters.Pagination.Cursor}");
        WL($"Total Chatters: {chatters.Total}");

        if(printNameListStr) WL(nameListStr);
        if(printIDListStr) WL(idListStr);
    }
    #endregion

    #region MassBan

    public static async Task MassBanUsers(
        bool confirmEachBan, TwitchAPI api, string broadcaster_id, string mod_id, List<string> banUserIDs, string? reason = null
    )
    {
        // Get info on all users.
        var users = await api.Helix.Users.GetUsersAsync(banUserIDs, null, null);

        // Just to be sure...
        WL($"Ban all listed users?");
        if(Console.ReadKey().Key != ConsoleKey.Y) return;
        // Absolutely sure...
        WL($"Are you sure?");
        if(Console.ReadKey().Key != ConsoleKey.Y) return;

        foreach(var user in users.Users)
        {
            bool shouldBan = true;
            WL($"Ban user {user.Login}, created at {user.CreatedAt} ?");
            if(confirmEachBan)
                shouldBan = Console.ReadKey().Key == ConsoleKey.Y;

            if(shouldBan)
            {
                WL("Ok ban.");
                await api.Helix.Moderation.BanUserAsync(
                    broadcaster_id, mod_id, new BanUserRequest(){
                        UserId = user.Id, Reason = reason
                    }
                );
            }
        }
    }

    /*
    Takes list of userIDs and bans them if they have followed the broadcaster within a certain date and time range. Useful for mass bannning after a follow bot attack.
    */
    public static async Task MassBanUsersWhoFollowedWithinTimeRange()
    {
        /*
        TODO: Implement this later, maybe. For now, roughly the same can be accomplished using GetAndPrintFollowers() and MassBanUsers().
        */
        await Task.Run(()=>{});
    }

    /*
    Mass bans chatters with accounts that were created after 
    'createdAfter' date. 'confirmEachBan' can be set to true if you
    want to confirm before each ban. Prior to starting mass ban, 
    method asks twice for confirmation, just so you can be sure that
    you want to ban. 'paginationCursor' can be used if chatters
    list is longer than one page.
    */
    public static async Task MassBanChattersByCreatedAfterDate(
        bool confirmEachBan, TwitchAPI api, string broadcaster_id, 
        string mod_id, DateTime createdAfter, string? paginationCursor = null
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
                await api.Helix.Moderation.BanUserAsync(
                    broadcaster_id, mod_id, new BanUserRequest {
                        UserId=user.Id, Reason="Mass ban."
                    }
                );
                WL("Ok ban.");
            }
        }


    }

    /* 
    Honestly just a different version of MassBanChattersByCreatedAfterDate. 
    This uses a TimeSpan to compare instead of just comparing dates. 'acctCreationTimeSpan' specifies the minimum age an account must be to avoid a ban. If the account age is below the specified TimeSpan, it's added to the ban list.
    */
    public static async Task MassBanChattersByAcctCreationTimeSpan(
        bool confirmEachBan, TwitchAPI api, string broadcaster_id, 
        string mod_id, TimeSpan acctCreationTimeSpan, string? paginationCursor = null 
        
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
            if((DateTime.Now-user.CreatedAt.ToLocalTime())<acctCreationTimeSpan)
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
                await api.Helix.Moderation.BanUserAsync(
                    broadcaster_id, mod_id, new BanUserRequest {
                        UserId=user.Id, Reason="Mass ban."
                    }
                );
                WL("Ok ban.");
            }
        }


    }
    #endregion

    #region Helper Methods
    // Takes a comma-separated string of values and generates the C# List<string> initializer string from it. That string can then be copy-pasted into the code for generating a List<string>.
    public static string ListStringFromCommaString(string commaString)
    {
        string s = "{";
        string[] sepStrings = commaString.Split(new char[]{','});
        for(int i=0; i<sepStrings.Length; i++)
        {
            sepStrings[i] = sepStrings[i].Replace(" ", "");
            s += $"\"{sepStrings[i]}\"";
            if(i < sepStrings.Length-1) s += ",";
        }
        s += "}";
        return s;
    }
    #endregion


}