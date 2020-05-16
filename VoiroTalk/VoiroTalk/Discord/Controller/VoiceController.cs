using Discord;
using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VoiroTalk.Const;

namespace VoiroTalk.Discord
{
    /// <summary>
    /// ボイスチャンネル接続/切断/利用を管理するコントローラクラスです
    /// </summary>
    public class VoiceController
    {
        private readonly Commands _commands;

        /// <summary>
        /// Discordオーディオ管理
        /// </summary>
        public IAudioClient AudioClient { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="commands"></param>
        public VoiceController(Commands commands)
        {
            _commands = commands;
        }

        /// <summary>
        /// 対象VCチャンネルに接続します。
        /// </summary>
        /// <param name="targetChannel"></param>
        /// <returns></returns>
        public async Task ConnectVCChannel(IVoiceChannel targetChannel)
        {
            // VCチャンネルの取得
            targetChannel = targetChannel ?? (_commands.Context.User as IGuildUser)?.VoiceChannel;

            if (targetChannel == null)
            {
                // 発言者がVCチャンネルに所属していない場合、エラーメッセージを返却し終了する
                await _commands.Context.Channel.SendMessageAsync(DiscordMessageConst.VOICE_CHANNEL_UNCONNECTED);
                Console.WriteLine("Connect:Not channel joined");
                return;
            }
            try
            {
                // 発言者がVCチャンネルに所属している場合、接続させる
                AudioClient = await targetChannel.ConnectAsync();
                Console.WriteLine("Connect:AudioClient connected by " + targetChannel.Name);
                await SendAsync(AudioClient);
                await _commands.Context.Channel.SendMessageAsync(string.Format(DiscordMessageConst.VOICE_CHANNEL_CONNECTED, targetChannel.Name));
            }
            catch (Exception ex)
            {
                // 何らかの例外が発生
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// 既に接続しているVCチャンネルから切断します
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectVCChannel(IVoiceChannel targetChannel)
        {
            targetChannel ??= (_commands.Context.User as IGuildUser)?.VoiceChannel;
            if (targetChannel != null) 
            {
                // 発言者がVCチャンネルに存在する場合、該当チャンネルから切断させる
                await targetChannel.DisconnectAsync();
                await _commands.Context.Channel.SendMessageAsync(string.Format(DiscordMessageConst.VOICE_CHANNEL_DISCONNECTED, targetChannel.Name));
                return;
            }

            // TODO:存在しない場合、現在接続しているチャンネルを検索し、切断させる
            
        }

        /// <summary>
        /// 音声送信ストリーム
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task SendAsync(IAudioClient client)
        {
            using (Process ffmpeg = CreateFFmpegStream())
            using (System.IO.Stream output = ffmpeg.StandardOutput.BaseStream)
            using (AudioOutStream discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try
                {
                    await output.CopyToAsync(discord);
                }
                finally
                {
                    await discord.FlushAsync();
                }
            }
        }
        private string path = AUDIO_CACHE + "niconico-sm22726795-_.mp4";
        private const string AUDIO_CACHE = "audio_cache\\";

        /// <summary>
        /// FFMpegを起動します
        /// </summary>
        /// <returns></returns>
        private Process CreateFFmpegStream()
        {
            Process ffmpegProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            Console.WriteLine("CreateFFmpegStream:" + ffmpegProcess.StartInfo);
            Console.WriteLine("CreateFFmpegStream:" + ffmpegProcess.ToString());
            return ffmpegProcess;
        }
    }
}
