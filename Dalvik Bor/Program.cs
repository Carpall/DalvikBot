using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Net;
using Discord.API;
using Discord;

namespace Dalvik_Bot
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        // env var
        public DiscordSocketClient Client;

        // async methods
        public async Task MainAsync()
        {
            // declaring socket client
            Client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Info });

            // login + start async
            await Client.LoginAsync(TokenType.Bot, System.IO.File.ReadAllText(@"C:\Users\Mondelli\Desktop\accesstoken.at"));
            await Client.StartAsync();

            // managing events
            Client.Log += log;
            Client.MessageReceived += receiveMessages;

            await Task.Delay(-1);
        }

        private async Task receiveMessages(SocketMessage signal)
        {
            // def simple class for easiest method using
            Message message = new Dalvik_Bot.Message();
            message.Value = signal.Content;


            if (!signal.Author.IsBot) {
                if (message.Pop(" ")[0] == '$') await sendMessages("Cosa c'è? :face_with_monocle:", signal.Channel);


            }
        }

        private async Task sendMessages(string message, ISocketMessageChannel channel)
        {
            channel.SendMessageAsync(message);
        }

        private Task log(LogMessage message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Log: {message}");
            Console.ForegroundColor = ConsoleColor.Gray;
            return Task.CompletedTask;
        }

    }
    class Message
    {
        public string Value = "";
        public string GetEmptyString()
        {
            string toReturn = "";
            bool isInterpolate = false;
            foreach (char chr in Value) {
                if (chr == '"') {
                    toReturn += '"';
                    isInterpolate = (isInterpolate) ? false : true;
                } else if (!isInterpolate)
                    toReturn += chr;
                else
                    toReturn += ' ';

            }
            return toReturn;
        }
        public string Pop(string str)
        {
            return Value.Replace(str, string.Empty);
        }
        public bool AsKeyword(string keyword)
        {
            try {
                int FirstLetterIndex = 0;
                for (int i = 0; i < Value.Length; i++) {
                    if (Value[i] == ' ')
                        FirstLetterIndex++;
                    else
                        break;
                }
                if (FirstLetterIndex >= 1)
                    return (Value.Remove(0, FirstLetterIndex).Substring(0, keyword.Length) == keyword) ? true : false;
                else
                    return (Value.Substring(0, keyword.Length) == keyword) ? true : false;
            } catch (ArgumentOutOfRangeException) { return false; }
        }
        public string Select(int startIndex, int finishIndex)
        {
            return Value.Substring(startIndex, finishIndex - startIndex);
        }
        public string RemoveComments(string commentsChar)
        {
            string result = "";
            bool isInterpolate = false;
            for (int i = 0; i < Value.Length; i++) {
                if (Value[i] == '"') {
                    isInterpolate = (!isInterpolate) ? true : false;
                    result += '"';
                } else if (Value[i] == commentsChar[0] && Value[i + 1] == commentsChar[1] && !isInterpolate) {
                    break;
                } else {
                    result += Value[i];
                }
            }
            return result;
        }
        public List<string> GetWordWrapList(string PatternToSplit)
        {
            string result = "";
            bool isInterpolate = false;
            for (int j = 0; j < Value.Length; j++) {
                if (Value[j] == '"') {
                    isInterpolate = (!isInterpolate) ? true : false;
                    result += '"';
                    continue;
                } else if (isInterpolate) {
                    result += Value[j];
                    continue;
                } else if (PatternToSplit.Contains(Value[j])) {
                    result += '⳿';
                    continue;
                } else result += Value[j];
            }
            List<string> toReturn = new List<string>();
            string[] resultSplit = result.Split('⳿');
            for (int i = 0; i < resultSplit.Count(); i++) {
                if (!string.IsNullOrWhiteSpace(resultSplit[i])) toReturn.Add(resultSplit[i]);
            }
            return toReturn;
        }
    }
}
