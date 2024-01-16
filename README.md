# stream-mod-app
 A simple app I use for interacting with the Twitch API using [TwitchLib](https://github.com/TwitchLib/TwitchLib). At the moment it's mostly for moderation related actions. But later on I might use it for other things. There is a Python script that can be used to generate auth URL and send the auth token to the app so it can be written to a file and used later.
 
 __NOTE:__
 * As I continue to add new code and improve existing code, there's a decent chance that each update will introduce a breaking change. 
 * I made this for my own use. Based on my setup, I have my own workflow, and this program works great for my own purposes. This isn't necesarily a general purpose program. But I'm sharing the code here, for whoever it helps.

### Requirements:
* Packages: This project doesn't ship with the libraries so you'll need to install them to be able to build and run this project. Versions must be equal or higher than specified versions.
 * [TwitchLib](https://github.com/TwitchLib/TwitchLib) v3.5.0 or above
 * [TwitchLib.Api](https://github.com/TwitchLib/TwitchLib.Api/) v3.9.0 or above: Explaining this is gonna be a bit of a paragraph. This component is a submodule in TwitchLib and gets installed along with other components when you install TwitchLib. However, at the time of updating this README (14/01/2024), the TwitchLib v3.5.0 package hosted on NuGet doesn't contain the TwitchLib.Api v3.9.0 submodule, it contains an older version. Certain methods like GetFollowedChannelsAsync and GetChannelFollowersAsync were only added in TwitchLib.Api v3.9.0. So after you have installed TwitchLib from NuGet, check and see if TwitchLib.Api v3.9.0 or above is installed. If it is not, install that from NuGet as well, and you're good to go.
* (Optional) Two machines. I use main app and auth generator on two separate machines that are on the same network, because that suits my workflow. You can use it how you wish.

### Note about auth workflow:
* This workflow was created based on my own setup. I wanted to be able to seamlessly generate auth tokens while livestreaming, without worrying about auth tokens accidentally appearing on screen. I have multiple systems on the network. The main (C#) app runs on one system, the auth generator (Python) script runs on another system. 

### Instructions:
* Setup:
    * Clone the repo.
        * [SMAConsoleAppv2](https://github.com/Demkeys/stream-mod-app/tree/main/SMAConsoleAppv2) is the C# Console app. This is the main app. Copy this folder to one machine on the network. This will be where the main program will run.
        * [SMAAuthGenerator](https://github.com/Demkeys/stream-mod-app/tree/main/SMAAuthGenerator) is the Python script. This is used for auth URL generation and sending a generated auth token over the network, to the main app. Copy this folder to another machine on the network. This will be where you will be generating and sending new auth tokens from.
* Authentication:
    * In SMAAuthGenerator, in py01.py, set the values for client_id (string), scope (space separated string) and remote_addr ((string,int) tuple). remote_addr is the address of the machine on the network that is running the main app. My redirectURI is set to ```https://localhost:3000``` because that the URI I've mentioned for my app in the Twitch Dev Console. Either set your app (in the Twitch Dev Console) to do the same, or change it to whatever yours is.
    * To generate auth URL, run py01.py script with the generate argument. ```python py01.py generate```. Copy the generated URL and paste it in your browser. Twitch will prompt you to grant access. Once you do, it will return a URL containing your auth token (along with other information). Copy only the auth token, this will be used later during the sending stage.
    * In SMAConsoleAppv2, open SMAAuth.cs, and navigate to the GetAuthData method and set the values of remoteEP to the machine that's running the Python script, and localEP to the machine that's running the main app. In Program.cs uncomment Example01 (auth example) and run the program with ```dotnet run```. The pogram starts a receive process and will inform you that it is waiting for data to be received. At any point in time you can quit the program by pressing the Q key. Once you see the message, head over to the py01.py script.
    * Run py01.py with two arguments - send and your token. ```python py01.py send <your token>```. The script will pack your clientID and token as one maessage and send this data over the network to the main app. Once the main app has received the data, it will do some processing and then write the data to a file in your local storage (On Window by default it is ```C:\Users\<Username>\AppData\Local\SMAConsoleApp\AuthData```). It will give you a message once this process completes. And with that your authentication is done.
* Use the program:
    * Now that authentication is done, you can head over to Program.cs and uncomment whichever example you want to run, and then run the program. Remember to uncomment only one example at a time and comment out the other examples. They are meant to be run as separate examples. If you run multiple at once, undefined behavriour may occur.
    * Of course, you can write your own code as well. The examples show you the workflow you need to follow. Basically just remember to call SMAAuth.Init() and then read the clientID and auth token from the SMAAuth class whenever you need them.
    * I've written a SMAModHelper.cs class with some methods that are useful to me, and will add more as necesasry in the future.

I will update this code over time as necessary and update the repo accordingly in the future. Like I mentioned before, this code works for me, for my own purposes. But if it helps you in any way, great.
