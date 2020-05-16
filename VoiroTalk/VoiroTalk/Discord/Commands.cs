using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using VoiroTalk.Const;

namespace VoiroTalk.Discord
{
    public class Commands : ModuleBase
    {
        /// <summary>
        /// ボイスコントローラメンバ
        /// </summary>
        private VoiceController _voiceController;


        /// <summary>
        /// 受け取った文字列をそのまま発言します
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [Command("echo")]
        public async Task Echo(string msg)
        {
            await ReplyAsync(msg);
        }

        /// <summary>
        /// VCチャンネルに接続します
        /// </summary>
        /// <param name="targetChannel">発言者がいるVCチャンネル</param>
        /// <returns></returns>
        [Command("connect", RunMode = RunMode.Async)]
        public async Task Connect(IVoiceChannel targetChannel = null)
        {
            _voiceController = new VoiceController(this);
            await _voiceController.ConnectVCChannel(targetChannel);
        }

        /// <summary>
        /// VCチャンネルから切断します
        /// </summary>
        /// <returns></returns>
        [Command("disconnect", RunMode = RunMode.Async)]
        public async Task Disconnect(IVoiceChannel targetChannel = null)
        {
            _voiceController = new VoiceController(this);
            await _voiceController?.DisconnectVCChannel(targetChannel);
        }
    }
}
