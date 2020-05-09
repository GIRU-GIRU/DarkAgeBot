using DarkAgeBot.Config;
using DarkAgeBot.Logging;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DarkAgeBot
{
    class BotReady
    {
        DiscordSocketClient _client;
        public BotReady(DiscordSocketClient client)
        {

            try
            {
                _client = client;
            }
            catch (Exception ex)
            {
                _ = CommandWindowLogger.LogMessageAsync(
                       new LogMessage(LogSeverity.Critical, MethodBase.GetCurrentMethod().Name, ex.Message, ex));
            }
        }

        internal async Task ReadyProcedures()
        {
            try
            {

                await Domain.InitializeDomain(_client);
                await Domain.GetLogChannel().SendMessageAsync("Bot initialized");
            }
            catch (Exception ex)
            {
                await CommandWindowLogger.LogMessageAsync(
                    new LogMessage(LogSeverity.Critical, DiscordLogger.GetAsyncMethodName(), ex.Message, ex));
            }
        }
    }
}
