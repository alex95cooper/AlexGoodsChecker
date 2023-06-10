namespace AlexGoodsChecker.Interfaces;

public interface ITelegramBot
{
    void Start();
    public void NotifyAboutCorrectWork(int count);
    public void NotifyAboutProductAvailability(Product product);
    public void NotifyAboutInvalidPage(Product product);
}