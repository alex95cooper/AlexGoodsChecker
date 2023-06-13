using AlexGoodsChecker;
using AlexGoodsChecker.Interfaces;

IConfigurator configurator = new Configurator();
List<Product> products = configurator.Deserialize();
DotNetEnv.Load(Directory.GetCurrentDirectory() + "\\.env");
string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
long chatId = long.Parse(Environment.GetEnvironmentVariable("CHAT_ID")!);
ITelegramBot bot = new AlexTelegramBot(token, chatId);
IGoodsChecker checker = new GoodsChecker(products, bot);
checker.Start();
Console.ReadKey();