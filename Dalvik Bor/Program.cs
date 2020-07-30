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
        enum Catergories
        {
            waitingForCommand,
            sayHello,
        }
        private void InitDictionary()
        {
            Dictionary.Add(Catergories.waitingForCommand, new string[] { "Ma che cazzo volete sempre? :face_with_monocle:", "Ma chi è il solito rompi palle?", "Madonna, ti scanno! Devi dirmi un comando bineurone", "Ma cos'è sei nato ritardato? devi darmi un comando", "Oh si, schiavizzami signore mio, sono al tuo servizio", "$help per avere più info coglione", "mh, ok, sei simpatico", "tua madre ha culo corto", "ciao! che cazzo vuoi?", "We la!", "...", "... :smile: e cosa vorresti?"});
            Dictionary.Add(Catergories.sayHello, new string[] { "Sono intollerante al lattosio, anche ai coglioni", "🦻🏾 non ci sento, parla più forte 😉", "👋", "", "Dai basta, è la 40esima volta oggi", "ahah, ma quanto sei simpi? a no sei simp 👍", "😀", "ciao anche a te! :smile:", "si, ok... salve 😬", "mh..., buon giorno, desideri?", "Non mi rompere i coglioni, non sto in vena oggi!", "Sei un po' cringe", "buon salve", "Viva i comunisti", "Lasciami dormire coglione", "Non vedo perchè risponderti... 🤷", "🤦 non cercare di rimorchiarmi", "Vai in Brasile e non tornare mai più, tieni ti ho anche fatto le valige 🧳", "Shhhh", "Sto dormendo, è Domenica... a no forse no", "Sto dormendo... 💤", "Ho sonno 🥱" });
            MainAsync().GetAwaiter().GetResult();
        }

        // init words to use and start mainasync
        static void Main(string[] args) => new Program().InitDictionary();

        // env var
        public DiscordSocketClient Client;
        private Dictionary<Catergories, string[]> Dictionary = new Dictionary<Catergories, string[]>();
        public Random Random = new Random();
        ISocketMessageChannel channel;

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
            channel = signal.Channel;


            if (!signal.Author.IsBot) {
                if (message.Pop(" ") == "$") await sendMessages(GetRandomAnswer(Catergories.waitingForCommand));
                else if (message.startWith('$')) {
                    if (message.isCommand("ciao")) await sendMessages(GetRandomAnswer(Catergories.sayHello));
                    else if (message.isCommand("say")) {
                        try {
                            await sendMessages(message.Value.Remove(0, 5));
                            await signal.DeleteAsync();
                        } catch (Exception e) { Console.WriteLine(e); }
                    }


                }

            }
        }

        private async Task sendMessages(string message)
        {
            await channel.SendMessageAsync(message);
        }

        private Task log(LogMessage message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Log: {message}");
            Console.ForegroundColor = ConsoleColor.Gray;
            return Task.CompletedTask;
        }

        private string GetRandomAnswer(Catergories category)
        {
            return Dictionary[category][Random.Next(0, Dictionary[category].Count()-1)];
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
        public bool startWith(char preCommand)
        {
            return (Pop(" ")[0] == '$') ? true : false;
        }
        public bool isCommand(string command)
        {
            return (Pop(" ").Substring(1, command.Count()) == command) ? true : false;
        }
    }
}
