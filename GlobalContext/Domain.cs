using DarkAgeBot.Config;
using DarkAgeBot.Logging;
using DarkAgeBot.Reliability;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DarkAgeBot
{
    public static class Domain
    {
        private static SocketGuild _guild;
        private static ITextChannel _logChannel;
        private static ConcurrentDictionary<ulong, SocketRole> _guildRoles = new ConcurrentDictionary<ulong, SocketRole>();

        public async static Task InitializeDomain(DiscordSocketClient client)
        {
            try
            {
               await Retrier.Attempt(() => AssignDomainFields(client), TimeSpan.FromSeconds(3), 5);   
            }
            catch (Exception ex)
            {
                await CommandWindowLogger.LogMessageAsync(
                    new LogMessage(LogSeverity.Critical, MethodBase.GetCurrentMethod().Name, ex.Message, ex));
            }
           
        }

        private static async Task AssignDomainFields(DiscordSocketClient client)
        {
            _guild = client.GetGuild(BotConfig.BotSettings.GuildID);
            _logChannel = _guild.GetChannel(706966247862173710) as ITextChannel;
        }
   
        public class Roles
        {
            public static async Task InitializeRoles()
            {
                if (_guild.Roles != null && _guild.Roles.Count > 0)
                {
                    foreach (var role in _guild.Roles)
                    {
                        _guildRoles.TryAdd(role.Id, role);
                    }
                }

            }

            public async static Task SyncDeletedRole(SocketRole role)
            {
                _guildRoles.TryRemove(role.Id, out _);
            }

            public async static Task SyncCreatedRole(SocketRole role)
            {
                _guildRoles.TryAdd(role.Id, role);
            }


            public async static Task SyncUpdatedRole(SocketRole oldRole, SocketRole newRole)
            {             
                _guildRoles.AddOrUpdate(oldRole.Id, newRole, (x, oldRole) => newRole);
            }

            //public static Task<IRole> GrabRole(string roleName)
            //{
            //    try
            //    {
            //        throw new NotImplementedException();
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}

        }
        public static ITextChannel GetLogChannel()
        {
            return _logChannel;
        }

        public static SocketGuild GetGuild()
        {
            return _guild;
        }



        //public static async Task<ITextChannel> GetChannel(ulong id)
        //{
        //    try
        //    {
        //        throw new NotImplementedException();

        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //}



        //public static Task<ITextChannel> GetDeletedMessageLog()
        //{
        //    try
        //    {
        //        throw new NotImplementedException();



        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

    }
}
