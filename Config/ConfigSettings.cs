using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DarkAgeBot.Config
{
    public class BotSettings
    {
        [JsonProperty]
        public int MessageCacheSize { get; set; }

        [JsonProperty]
        public ulong GuildID { get; set; }

        [JsonProperty]
        public string BotToken { get; set; }
    }
}
