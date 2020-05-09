using DarkAgeBot.Logging;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DarkAgeBot.Reliability
{
    class ConnectionReliability
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        private static bool _attemptReset;
        private static int _attemptRetries;

        private readonly DiscordSocketClient _client;
        private CancellationTokenSource _cts;


        public ConnectionReliability(DiscordSocketClient client, bool attemptReset, int attemptRetries)
        {
            _cts = new CancellationTokenSource();
            _client = client;
        
            _attemptReset = attemptReset;
            _attemptRetries = attemptRetries;

            _client.Connected += ConnectedAsync;
            _client.Disconnected += DisconnectedAsync;
        }

        public async Task ConnectedAsync()
        {
            await CommandWindowLogger.WriteMessageAsync("Client successfully reconnected");
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }

        public async Task DisconnectedAsync(Exception _e)
        {
            await CommandWindowLogger.WriteMessageAsync("Client disconnected, checking to reconnect");

            if (_attemptReset)
            {
                _ = Task.Delay(_timeout, _cts.Token).ContinueWith(async _ =>
                {
                    await CommandWindowLogger.WriteMessageAsync("Timeout expired, continuing to check client state...");
                    bool success = await CheckStateAsync();

                    if (success)
                    {
                        await CommandWindowLogger.WriteMessageAsync("Client reconnected succesfully!");
                    }
                });
            }
        }

        private async Task<bool> CheckStateAsync()
        {
            if (_client.ConnectionState == ConnectionState.Connected) return true;

            if (_attemptReset)
            {
                for (int i = 0; i < _attemptRetries; i++)
                {
                    await CommandWindowLogger.WriteMessageAsync("Attempting to restart DarkAge bot");

                    var timeout = Task.Delay(_timeout);
                    var connect = Program.RestartBot();

                    var task = await Task.WhenAny(timeout, connect);

                    if (task != timeout)
                    {
                        if (connect.IsFaulted)
                        {
                            await CommandWindowLogger.WriteMessageAsync("CRITICAL ERROR - " + connect.Exception);
                        }
                        else if (connect.IsCompletedSuccessfully)
                        {                           
                            return true;
                        }
                    }
                }
            }

            await CommandWindowLogger.WriteMessageAsync("Client has been disconnected from Discord");
            _cts.Cancel();
            return false;
        }
    }
}
