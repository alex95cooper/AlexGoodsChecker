using Telegram.Bot;
using AlexGoodsChecker.Interfaces;

namespace AlexGoodsChecker;

public class AlexTelegramBot : ITelegramBot
{
    private const string StartMessage = "Goods monitoring started";
    private const string AppActivityMessage = "The application is working correctly. A total of {0} requests were made today.";
    private const string GoodsAvailabilityMessage = "Item with code {0} appeared in the store {1}.";
    private const string ErrorMessage = "Failed to get data from {0} website for product {1}";
    
    private readonly TelegramBotClient _botClient;
    private readonly long _chatId;

    public AlexTelegramBot(string token, long chatId)
    {
        if (token != null) _botClient = new TelegramBotClient(token);
        _chatId = chatId;
    }

    public void Start()
    {
        _botClient?.SendTextMessageAsync(_chatId, StartMessage);
    }

    public void NotifyAboutCorrectWork(int count)
    {
        _botClient?.SendTextMessageAsync(_chatId, string.Format(AppActivityMessage, count));
    }

    public void NotifyAboutProductAvailability(Product product)
    {
        _botClient?.SendTextMessageAsync(_chatId, string.Format(
            GoodsAvailabilityMessage, product?.Id, product?.MarketPlace));
    }

    public void NotifyAboutInvalidPage(Product product)
    {
        _botClient?.SendTextMessageAsync(_chatId, string.Format(
            ErrorMessage, product?.MarketPlace, product?.Id));
    }
}