using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using System.Collections.Generic;

namespace Alpha
{
    class Program
    {
        public static DiscordSocketClient Client;
        private CommandService Commands;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();
          
        

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
           {
               LogLevel = LogSeverity.Debug,
           });

            //help help HELP heLp
            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            await Client.LoginAsync(TokenType.Bot, "NTAwNDM5Njk3MjQ0NDIyMTU1.DqTgpg.OX4pbbKON4UhaoSrd6ONxSrEBYQ");
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync(".eventhelp[V5.1.5]", "https://twitch.tv/scratchybeats", StreamType.Twitch);
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;
            if (Context.Message == null || Context.Message.Content == "agree")
            {
                IReadOnlyCollection<SocketRole> Roles = Context.Guild.GetUser(Context.User.Id).Roles;
                var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Member");
                List<SocketRole> userroles = Roles.ToList();
                if (userroles.Contains(role) == false)
                {
                    SocketGuildChannel roleadd = Context.Guild.TextChannels.FirstOrDefault(x => x.Name == "roleadd");
                    await ((SocketGuildUser)Context.User).AddRoleAsync(role);
                }
            }

                int ArgPos = 0;
            if (!(Message.HasStringPrefix("+", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;

            var Result = await Commands.ExecuteAsync(Context, ArgPos);
            if (!Result.IsSuccess)
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong with executing a command. Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
        }


    }
}
