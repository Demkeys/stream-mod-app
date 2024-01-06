// Mainly for auth related things.
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text.Json;
using System.Web;
using TwitchLib.Api.Auth;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Buffers.Text;

public static class SMAAuth
{
    static void WL(object o) => Console.WriteLine(o);
    class DVTAuthData
    { 
        public string ClientId = "";
        public string AuthToken = "";
    }
    public static string ClientId = "";
    public static string AuthToken = "";
    
    // Reads data from file and populates fields
    public static void Init()
    {
        string LocalAppDataDir = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData
        );
        string authPath = $@"{LocalAppDataDir}\SMAConsoleApp\Auth";
        if(!File.Exists($@"{authPath}\AuthData"))
        {
            WL("Auth data missing. Generate auth data."); 
            return;
        }
        byte[] authFileData = File.ReadAllBytes($@"{authPath}\AuthData");
        for(int i=0; i < authFileData.Length; i++)
            authFileData[i] ^= 0xff;
        string fileDataStr = ASCIIEncoding.ASCII.GetString(authFileData);
        
        DVTAuthData? data = (DVTAuthData?)JsonSerializer.Deserialize(
            fileDataStr, typeof(DVTAuthData), new JsonSerializerOptions() {
                IncludeFields=true
            }
        );
        ClientId = data.ClientId;
        AuthToken = data.AuthToken;
    }

    // Not used anymore but I'm leaving this in here.
    [Obsolete(message:"Not used anymore. Use external Python script for that.")]
    public static void GenerateAuthURL()
    {
        UriBuilder url = new("https", "id.twitch.tv/oauth2/authorize");
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
        string scope = HttpUtility.UrlEncode(
            "moderator:read:chatters moderator:read:followers"
        );
        query.Add("client_id", ClientId);
        query.Add("redirect_uri", "none");
        query.Add("response_type", "token");
        query.Add("scope",scope);
        query.Add("something","abc");
        url.Query = query.ToString();
        Console.WriteLine(url.ToString());
    }

    // Coding drunk. This should go well...
    // This method starts a loop that keeps going until data is received or the loop is terminated. During this loop, a socket will try to receive data. Use the Python script on the other machine to send the based64 encoded auth data as bytes, and those bytes will be written to a binary file.
    public static async Task GetAuthData()
    {
        byte[] buffer = new byte[4096];
        ArraySegment<byte> bufferSeg = new(buffer);
        Socket s = new(
            AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint remoteEP = new(IPAddress.Parse("192.168.8.100"), 9010);
        IPEndPoint localEP = new(IPAddress.Parse("192.168.8.111"), 9010);
        
        s.Bind(localEP);
        
        WL("Starting recv process. Waiting for data...");
        
        Task<SocketReceiveFromResult> recv_tsk01 = s.ReceiveFromAsync(bufferSeg, remoteEP);
        await recv_tsk01;

        WL("Recv process completed. Data will be written to Auth file.");

        // Recv'd data will be (ClientID|AuthToken) so split them first.
        string[] dataPair = ASCIIEncoding.ASCII.GetString(
            buffer, 0, recv_tsk01.Result.ReceivedBytes
        ).Split(new char[]{'|'});

        DVTAuthData newAuthData = new() {
            ClientId=dataPair[0], 
            AuthToken=dataPair[1]
        };

        string fileDataStr = JsonSerializer.Serialize(
            newAuthData, typeof(DVTAuthData), new JsonSerializerOptions(){
                IncludeFields=true
            }
        );

        byte[] fileData = ASCIIEncoding.ASCII.GetBytes(fileDataStr);
        for(int i=0; i < fileData.Length; i++)
            fileData[i] ^= 0xff;
 
        string LocalAppDataDir = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData
        );
        string authPath = $@"{LocalAppDataDir}\SMAConsoleApp\Auth";
        if(!Directory.Exists(authPath)) Directory.CreateDirectory(authPath);
        File.WriteAllBytes($@"{authPath}\AuthData", fileData);

        s.Close();
    }
}