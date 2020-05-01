using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VoiroTalk.Discord
{
    public class DiscordCommands : ModuleBase
    {
        [Command("echo")]
        public async Task Echo(string msg)
        {
            await ReplyAsync(msg);
        }
    }
}
