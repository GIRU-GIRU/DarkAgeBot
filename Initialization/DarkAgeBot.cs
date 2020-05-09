using DarkAgeBot.Config;
using DarkAgeBot.Logging;
using DarkAgeBot.Reliability;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DarkAgeBot
{
    internal static class DarkAgeBot
    {

        private static DiscordSocketClient _client;
        private static CommandService _commands;
        private static IServiceProvider _services;
        public static async Task LaunchBotAsync()
        {
            DiscordSocketConfig botConfig = new DiscordSocketConfig()
            {
                MessageCacheSize = BotConfig.BotSettings.MessageCacheSize,
                AlwaysDownloadUsers = true,
                DefaultRetryMode = RetryMode.RetryTimeouts,
                LogLevel = LogSeverity.Verbose,
                ExclusiveBulkDelete = true,
            };



            var CommandServiceConfig = new CommandServiceConfig()
            {
                DefaultRunMode = RunMode.Async
            };

            _client = new DiscordSocketClient(botConfig);
            _commands = new CommandService(CommandServiceConfig);
            _services = new ServiceCollection()
                .AddSingleton(_commands)
                .AddSingleton(_client)
                .BuildServiceProvider();

            var botReady = new BotReady(_client);

            _client.Ready += botReady.ReadyProcedures;
            _client.RoleCreated += Domain.Roles.SyncCreatedRole;
            _client.RoleDeleted += Domain.Roles.SyncDeletedRole;
            _client.RoleUpdated += Domain.Roles.SyncUpdatedRole;
            _client.Log += CommandWindowLogger.LogMessageAsync;


            _client.UserJoined += UserSanitizer.Sanitize;
            // _client.UserLeft += 

            _client.MessageDeleted += UserMessageLogging.LogDeletedMessage;
            _client.MessageUpdated += UserMessageLogging.LogEditedMessage;
            _client.MessagesBulkDeleted += UserMessageLogging.LogBulkDelete; ;

            _commands.CommandExecuted += PostCommand.OnExecutedCommand;

            var reliability = new ConnectionReliability(_client, true, 5);


            await RegisterCommandAsync();
        }



        internal async static Task RunBotAsync(CancellationToken token)
        {

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await _client.LoginAsync(TokenType.Bot, BotConfig.BotSettings.BotToken);
                    await _client.StartAsync();

                    await Task.Delay(-1, token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED TO START BOT! - ", ex.Message);
            }

            await Task.Delay(-1);
        }


        public static async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private static async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message.Author.IsBot) return;
            var context = new SocketCommandContext(_client, message);

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                await _commands.ExecuteAsync(context, argPos, _services);

            }
        }
    }
}
