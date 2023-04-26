using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AlexGoodsChecker;

public class AlexTelegramBot
{
    private const string TelegramBotToken = "6065625282:AAEjYaSxmKLgq3GgWiwyNI2PnFFal8B6NPY";
    private const string AppActivityMessage = "The application is working correctly. A total of {0} requests were made per day.";
    private const string GoodsAvailabilityMessage = "Item with code {0} appeared in the store {1}";
    private const string StartMessage = "Goods monitoring started";
    private const int MonitoringInterval = 500000;
    
    private readonly TelegramBotClient _botClient;

    private int _count;

    public AlexTelegramBot()
    {
        _botClient = new TelegramBotClient(TelegramBotToken);
        _botClient.StartReceiving(Update, Error);
    }

    private Task Start(long chatId)
    {
        List<Product> products = Deserialize() ?? new List<Product>();
        using HttpClient client = new();
        while (true)
        {
            _count++;
            foreach (Product product in products)
            {
                if (product == null) continue;
                if (GetRequester.CheckProductAvailability(product, client))
                {
                    _botClient?.SendTextMessageAsync(chatId, string.Format(GoodsAvailabilityMessage, product.Id,
                        product.MarketPlace));
                }
            }

            _botClient?.SendTextMessageAsync(chatId, string.Format(AppActivityMessage, _count));
            Thread.Sleep(MonitoringInterval);
        }
    }

    private static List<Product> Deserialize()
    {
        using StreamReader reader = new StreamReader("appsettings.json");
        string json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<List<Product>>(json);
    }

    private async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var message = update?.Message ?? new Message();
        if (message.Text != null && message.Text.Contains("/start"))
        {
            _botClient?.SendTextMessageAsync(message.Chat.Id, StartMessage, cancellationToken: token);
            Thread.Sleep(MonitoringInterval);
            await Start(message.Chat.Id)!;
        }
    }

    private static async Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        Console.WriteLine("Something wrong...");
        await Task.Delay(MonitoringInterval, token);
    }
}