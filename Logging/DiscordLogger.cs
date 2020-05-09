using Discord;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DarkAgeBot.Logging
{
    public static class DiscordLogger
    {
        public async static void HandleExceptionQuietly(string message, Exception ex)
        {
            try
            {
                var innermostExceptionMessage = GetInnermostException(ex).Message;

                await Domain.GetLogChannel().SendMessageAsync($"Exception thrown in {message} - {ex.Message}");
            }
            catch (Exception e)
            {
                await CommandWindowLogger.WriteMessageAsync($"--CRITICAL -- Error in Handle Exception!! - {e.Message}");
            }
        }

        public async static void HandleExceptionPublically(ITextChannel chnl, string message, Exception ex)
        {
            try
            {
                var innermostExceptionMessage = GetInnermostException(ex).Message;

                await chnl.SendMessageAsync($"Exception thrown in {message} - {ex.Message}");
            }
            catch (Exception e)
            {
                await CommandWindowLogger.WriteMessageAsync($"--CRITICAL -- Error in Handle Exception!! - {e.Message}");
            }
        }


        public static Exception GetInnermostException(Exception ex)
        {

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex;
        }
        public static string GetAsyncMethodName([CallerMemberName]string name = "unknown") => name;
    }
}
