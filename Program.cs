using System;
using System.Threading;
using System.Threading.Tasks;
using DarkAgeBot.Config;
using DarkAgeBot.Logging;
using Discord;

namespace DarkAgeBot
{
    class Program
    {
        private static CancellationTokenSource _tokenSource;
        static async Task Main()
        {
            Console.WriteLine("Initializing Config");
            BotConfig.InitializeConfig();
            Console.WriteLine("Launching Bot");
            await DarkAgeBot.LaunchBotAsync();

            _tokenSource = new CancellationTokenSource();


            if (!_tokenSource.IsCancellationRequested)
            {
                await DarkAgeBot.RunBotAsync(_tokenSource.Token);
            }  
        }


        public static async Task RestartBot()
        {
            try
            {
                _tokenSource.Cancel();
                _tokenSource = null;
                await Main();
            }
            catch (Exception ex)
            {
                await CommandWindowLogger.LogMessageAsync(
                       new LogMessage(LogSeverity.Critical, DiscordLogger.GetAsyncMethodName(), ex.Message, ex));
            }

        }
    }
}
