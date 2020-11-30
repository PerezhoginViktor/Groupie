using System;
using System.Collections.Specialized;
using System.Net;

namespace SplitGroupie
{
    public class DiscordWebhook : IDisposable
    {
        private readonly WebClient discordWebClient;
        private static NameValueCollection discord = new NameValueCollection();
        public string WebHook { get; set; }
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }

        public DiscordWebhook()
        {
            discordWebClient = new WebClient();
        }


        public void SendMessage(string msgSend)
        {
            discord.Add("username", UserName);
            discord.Add("avatar_url", ProfilePicture);
            discord.Add("content", msgSend);

            discordWebClient.UploadValues(WebHook, discord);
        }

        public void Dispose()
        {
            discordWebClient.Dispose();
        }
    }
}
