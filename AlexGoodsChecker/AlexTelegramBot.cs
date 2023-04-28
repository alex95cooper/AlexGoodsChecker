using System.Text.Json;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Timer = System.Timers.Timer;

namespace AlexGoodsChecker;

public class AlexTelegramBot
{
    private const string TelegramBotToken = "6065625282:AAEjYaSxmKLgq3GgWiwyNI2PnFFal8B6NPY";

    private const string AppActivityMessage =
        "The application is working correctly. A total of {0} requests were made today.";

    private const string GoodsAvailabilityMessage = "Item with code {0} appeared in the store {1}.";
    private const string StartMessage = "Goods monitoring started";
    private const double MonitoringInterval = 500000;
    private const long ChatId = -872421566;

    private readonly List<Product> _products;
    private readonly TelegramBotClient _botClient;
    private readonly List<long> _chatIdList;
    private readonly HttpClient _client;

    private int _count;

    public AlexTelegramBot(HttpClient client)
    {
        _products = Deserialize() ?? new List<Product>();
        _botClient = new TelegramBotClient(TelegramBotToken);
        _chatIdList = new List<long>();
        _client = client;
    }

    public void Start()
    {
        _botClient?.SendTextMessageAsync(ChatId, StartMessage);
        Timer timer = new(MonitoringInterval);
        timer.Elapsed += Start_Monitoring;
        timer.Start();
    }
    
    private void Start_Monitoring(object sender, ElapsedEventArgs e)
    {
        if (_products != null)
        {
            _count++;
            foreach (var product in _products)
            {
                if (GetRequester.CheckProductAvailability(product, _client))
                {
                    _botClient?.SendTextMessageAsync(ChatId, string.Format(
                        GoodsAvailabilityMessage, product?.Id, product?.MarketPlace));
                }
            }
        }

        _botClient?.SendTextMessageAsync(ChatId, string.Format(AppActivityMessage, _count));
    }

    private static List<Product> Deserialize()
    {
        using StreamReader reader = new StreamReader("appsettings.json");
        string json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<List<Product>>(json);
    }

    private void MakeMessageSending(string message)
    {
        if (_chatIdList == null) return;
        foreach (long chatId in _chatIdList)
        {
            _botClient?.SendTextMessageAsync(chatId, message ?? string.Empty);
        }
    }
}