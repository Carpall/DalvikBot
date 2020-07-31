﻿using System;
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
            accessDenied,
            commandNotExists,
            userJoin,
        }
        private void InitDictionary()
        {
            Dictionary.Add(Catergories.waitingForCommand, new string[] { "Ma che cazzo volete sempre? :face_with_monocle:", "Ma chi è il solito rompi palle?", "Madonna, ti scanno! Devi dirmi un comando bineurone", "Ma cos'è sei nato ritardato? devi darmi un comando", "Oh si, schiavizzami signore mio, sono al tuo servizio", "$help per avere più info coglione", "mh, ok, sei simpatico", "tua madre ha culo corto", "ciao! che cazzo vuoi?", "We la!", "...", "... :smile: e cosa vorresti?"});
            Dictionary.Add(Catergories.sayHello, new string[] { "Sono intollerante al lattosio, anche ai coglioni", "🦻🏾 non ci sento, parla più forte 😉", "👋", "", "Dai basta, è la 40esima volta oggi", "ahah, ma quanto sei simpi? a no sei simp 👍", "😀", "ciao anche a te! :smile:", "si, ok... salve 😬", "mh..., buon giorno, desideri?", "Non mi rompere i coglioni, non sto in vena oggi!", "Sei un po' cringe", "buon salve", "Viva i comunisti", "Lasciami dormire coglione", "Non vedo perchè risponderti... 🤷", "🤦 non cercare di rimorchiarmi", "Vai in Brasile e non tornare mai più, tieni ti ho anche fatto le valige 🧳", "Shhhh", "Sto dormendo, è Domenica... a no forse no", "Sto dormendo... 💤", "Ho sonno 🥱" });
            Dictionary.Add(Catergories.accessDenied, new string[] { "Accesso negato bro ⛔", "mh, no mi dispiace!", "è inutile che continui a fare il comando, tanto lo può usare solo Carpal", "no!", "eh si ciao, mica so scemo", "puoi star qui fino a domani, tanto non te lo faccio usare", "if (signal.Author.Mention == \"<@!699146708466008115>\"){  <- vuol dire che solo carpal può...", "🖕🏻" });
            Dictionary.Add(Catergories.commandNotExists, new string[] {"Ma sai scrivere?", "Per me, non sai scrivere", "Non so che cazzo vuoi, ma quel comando non esiste...", "$help per info sui comandi🤦‍", "ufficio coglioni, di là", "no!, questo comando te lo sei letteralmente inventato!", "e secondo te io potevo avere un comando così brutto?", "mhh... non so dicosa parli", "non ho voglia di aiutarti", "se fai $help vedi quanto sei scemo a fare un comando che non esiste", "cos.. no coglione, non so cosa voglia dire", "che cazz... quel comando ha meno senso della tua nascina", "unieuro... bho, che cazzo volevi??", "💩", "🚗 bruumm...", "... c'è tuo padre che ti chiama, non lo senti, su vai che poi ti castiva", "❗ chi cazzo mi ha chiamato" });
            Dictionary.Add(Catergories.userJoin, new string[] { "Cavolo! una persona nuova dopo 8 lustri...", "Finalmente novità", "Ciao!", "we laaa, io sono il più faigo", "va che ogni tanto entra qualcuno in sto server di morti di fame :face_with_monocle:", "Bravo, che sei entrato", "Ok... credo che sia entrato qualcuno", "è inutile che te lo aspetti... non sei il benvenuto", "Benvenuto :smile:", "ma la gente ha pure il coraggio di entrare in sto server?", "{mention} che nome di merda che hai", "Con quel nome, sarai cringissimo {mention}", "aho jamm' ja, n'amoce a pigghia nu caffettino aobbar, ciruz {mention}", "no ok... è davvero entrato qualcuno? 😅", "{mention} fidati esci! esci prima che puoi 🤐", "dai! {mention} per sta volta sei il benvenuto", "mh... non sei il benvenuto! esci perfavore....", "ahhahahahhahaahahha, non hai capito niente, questo gruppo è acccessibile solo ai maschi...", "{mention} aspetta aspetta, famoce un serfi 🤳🏻"});
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
            Client.UserJoined += join;
            await Task.Delay(-1);
        }

        private async Task join(SocketGuildUser user)
        {
            await (user.Guild.DefaultChannel).SendMessageAsync(GetRandomAnswer(Catergories.userJoin).Replace("{mention}", user.Mention));
            return;
        }

        private async Task receiveMessages(SocketMessage signal)
        {
            // def simple class for easiest method using
            Message message = new Message();
            message.Value = signal.Content;
            channel = signal.Channel;


            if (!signal.Author.IsBot) {
                if (message.Pop(" ") == "$") await sendMessages(GetRandomAnswer(Catergories.waitingForCommand));
                else if (message.startWith('$')) {
                    if (message.AsKeyword("ciao")) await sendMessages(GetRandomAnswer(Catergories.sayHello));
                    else if (message.AsKeyword("say")) {
                        await sendMessages(message.Value.Remove(0, 5));
                        await signal.AddReactionAsync(new Emoji("✅"));
                    } else if (message.AsKeyword("exec")) {
                        if (signal.Author.Mention == "<@!699146708466008115>") {
                            System.CodeDom.Compiler.CompilerResults result = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string> {
                            { "CompilerVersion", "v3.5" }
                            }).CompileAssemblyFromSource(new System.CodeDom.Compiler.CompilerParameters { GenerateInMemory = true, GenerateExecutable = false }, message.Value.Remove(0, 6));
                            if (result.Errors.Count != 0) {
                                await sendMessages("Errori:");
                                for (int i = 0; i < result.Errors.Count; i++)
                                    await sendMessages(result.Errors[i].ToString().Replace(@"c:\Users\Mondelli", "."));
                            } else {
                                await sendMessages("Pulito! Puoi eseguirlo, è corretto");
                            }
                        } else {
                            await sendMessages(GetRandomAnswer(Catergories.accessDenied));
                        }
                    }
                    else if (message.AsKeyword("search.stack ")) await sendMessages($"Ecco la tua ricerca: https://stackoverflow.com/search?q={message.Value.Remove(0, message.Value.IndexOf(" ")).Replace(" ", "+")}");
                    else if (message.AsKeyword("search.github ")) await sendMessages($"Ecco la tua ricerca: https://github.com/search?q={message.Value.Remove(0, message.Value.IndexOf(" ")).Replace(" ", "+")}");
                    else if (message.AsKeyword("search.duck ")) await sendMessages($"Ecco la tua ricerca: https://duckduckgo.com/?q={message.Value.Remove(0, message.Value.IndexOf(" ")).Replace(" ", "+")}&ia=web");
                    else if (message.AsKeyword("search ")) await sendMessages($"Ecco la tua ricerca: https://google.com/search?q={message.Value.Remove(0, message.Value.IndexOf(" ")).Replace(" ", "+")}");
                    else if (message.AsKeyword("search.ph ")) await sendMessages($"Ecco la tua ricerca: https://pornhub.com/search?q={message.Value.Remove(0, message.Value.IndexOf(" ")).Replace(" ", "+")}");
                    else if (message.AsKeyword("search.yt ")) await sendMessages($"Ecco la tua ricerca: https://youtube.com/results?search_query={message.Value.Remove(0, message.Value.IndexOf(" ")).Replace(" ", "+")}");
                    else if (message.AsKeyword("source")) await sendMessages("Ecco il source: https://github.com/Carpall/DalvikBot");
                    else if (message.AsKeyword("author")) await sendMessages("Ecco la page dell'autore: https://github.com/Carpall");
                    //else if (message.AsKeyword("del")) await channel.DeleteMessageAsync();
                    else await sendMessages(GetRandomAnswer(Catergories.commandNotExists));
                    // if message does not contains '$' access symbol
                } else {
                    // sistema di ascolto messaggi, per machine learning
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
            keyword = '$' + keyword;
            if (Pop(" ").ToLower().Replace(keyword, "") == "") return true;
            else {
                if (Value.Replace(keyword, "") == Value && Value.Replace(keyword.ToUpper(), "") == Value) return false;
                else {
                    return true;
                }
            }
            //string mes = Value.ToLower();
            //if (Pop(" ").ToLower() == keyword) return true;
            //for (int i = 0; i < keyword.Count(); i++) {
            //    if (mes[i] == ' ')
            //        return (mes.Substring(0, i - 1).ToLower() == keyword) ? true : false;
            //    else if (mes[i] == keyword[i]) continue;
            //    else
            //        return (mes.Substring(0, i - 1) == keyword) ? true : false;
            //}
            //return true;
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
    }
}
