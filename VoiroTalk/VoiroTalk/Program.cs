using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using VoiroTalk.Const;
using VoiroTalk.Discord;

namespace VoiroTalk
{
    class Program
    {
        public static DiscordSocketClient _client;
        public static CommandService _commands;
        public static IServiceProvider _services;
        public static MessageRecived _recived;
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// 起動時処理
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection().BuildServiceProvider();
            _recived = new MessageRecived();
            
            _client.MessageReceived += CommandRecieved;
            _client.Log += Log;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _client.LoginAsync(TokenType.Bot, AuthConst.TOKEN);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// メッセージの受信処理を行います。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal async Task CommandRecieved(SocketMessage messageParam)
        {
            // nullなら処理しない
            if (!(messageParam is SocketUserMessage message)) return;

            // botは対象外
            if (message.Author.IsBot) return;

            // デバッグ用メッセージを出力
            Console.WriteLine("{0} {1}:{2}", message.Channel.Name, message.Author.Username);

            int argPos = 0;

            // 接頭辞でコマンドかどうかを識別
            if (!(message.HasCharPrefix(DiscordConst.MESSAGE_PREFIX, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            CommandContext context = new CommandContext(_client, message);

            IResult result = await _commands.ExecuteAsync(context, argPos, _services);

            if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
             
        }

        /// <summary>
        /// ログを出力します
        /// </summary>
        /// <param name="msg">出力するメッセージ</param>
        /// <returns></returns>
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
