using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using DarkAgeBot.Logging;
using Discord;
using Newtonsoft.Json;

namespace DarkAgeBot.Config
{
    public static class BotConfig
    {
        public static BotSettings BotSettings;
        public static void InitializeConfig()
        {
            try
            {
                var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var folder = "Config";
                var fileName = "Config.Json";

                var path = Path.Combine(assemblyLocation, folder, fileName);


                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    BotSettings = (BotSettings)serializer.Deserialize(file, typeof(BotSettings));
                }
            }
            catch (Exception ex)
            {
                 CommandWindowLogger.LogMessageAsync(
                         new LogMessage(LogSeverity.Critical, MethodBase.GetCurrentMethod().Name, ex.Message, ex));
            }
        }



    }
}
