using AlexGoodsChecker;
using AlexGoodsChecker.Interfaces;

IConfigurator configurator = new Configurator();
List<Product> products = configurator.Deserialize();
DotNetEnv.Load(Directory.GetCurrentDirectory() + "\\.env");
string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
using HttpClient client = new();
ITelegramBot bot = new AlexTelegramBot(products, client, token);
bot.Start();
Console.ReadKey();