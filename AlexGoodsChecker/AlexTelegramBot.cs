using System.Timers;
using Telegram.Bot;
using Timer = System.Timers.Timer;
using AlexGoodsChecker.Extensions;
using AlexGoodsChecker.Interfaces;

namespace AlexGoodsChecker;

public class AlexTelegramBot : ITelegramBot
{
    private const string AppActivityMessage = "The application is working correctly. A total of {0} requests were made today.";
    private const string GoodsAvailabilityMessage = "Item with code {0} appeared in the store {1}.";
    private const string StartMessage = "Goods monitoring started";
    private const double MonitoringInterval = 500000;
    private const long ChatId = -872421566;

    private readonly List<Product> _products;
    private readonly TelegramBotClient _botClient;
    private readonly HttpClient _client;

    private int _count;

    public AlexTelegramBot(List<Product> products, HttpClient client, string token)
    {
        _products = products ?? new List<Product>();
        if (token != null) _botClient = new TelegramBotClient(token);
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
                string htmlText = _client.GetHttpText(product).Result;
                if (GetRequester.CheckProductAvailability(product, htmlText))
                {
                    _botClient?.SendTextMessageAsync(ChatId, string.Format(
                        GoodsAvailabilityMessage, product?.Id, product?.MarketPlace));
                }
            }
        }

        _botClient?.SendTextMessageAsync(ChatId, string.Format(AppActivityMessage, _count));
    }
}