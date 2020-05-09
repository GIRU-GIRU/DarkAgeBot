using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DarkAgeBot.Logging
{
    public static class CommandWindowLogger
    {

        public async static Task LogMessageAsync(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
        }

        public async static Task WriteMessageAsync(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
