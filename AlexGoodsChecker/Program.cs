using AlexGoodsChecker;

using HttpClient client = new();
AlexTelegramBot bot = new(client);
bot.Start();
Console.ReadKey();