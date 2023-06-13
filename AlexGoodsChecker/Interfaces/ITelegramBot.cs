namespace AlexGoodsChecker.Interfaces;

public interface ITelegramBot
{
    void Start();
    void NotifyAboutCorrectWork(int count);
    void NotifyAboutProductAvailability(Product product);
    void NotifyAboutInvalidPage(Product product);
}