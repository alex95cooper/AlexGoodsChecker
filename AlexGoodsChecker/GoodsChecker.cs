using System.Text.RegularExpressions;
using System.Timers;
using Timer = System.Timers.Timer;
using AlexGoodsChecker.Interfaces;

namespace AlexGoodsChecker;

public class GoodsChecker : IGoodsChecker
{
    private const string AmazonProductUnavailablePattern =
        "<span class=\"a-color-price a-text-bold\">Currently unavailable.</span>";

    private const string MakeUpProductUnavailablePattern =
        "<div class=\"product-item__status red\" id=\"product_enabled\">Nu este disponibil</div>";

    private const string RozetkaProductUnavailablePattern =
        "type=\"button\" class=\"button button--medium button--navy ng-star-inserted\">";

    private const double MonitoringInterval = 500000;

    private readonly ITelegramBot _bot;
    private readonly List<Product> _products;
    private readonly List<RetriesItem> _retriesItems;

    private int _count;

    public GoodsChecker(List<Product> products, ITelegramBot bot)
    {
        _bot = EnsureBotNotNull(bot);
        _retriesItems = new List<RetriesItem>();
        _products = products ?? new List<Product>();
    }

    public void Start()
    {
        _bot.Start();
        Timer timer = new(MonitoringInterval);
        timer.Elapsed += MonitorGoods;
        timer.Elapsed += MakeRetries;
        timer.Start();
    }

    private void MakeRetries(object sender, ElapsedEventArgs e)
    {
        if (_retriesItems.Count > 0)
        {
            using HttpClient client = new();
            foreach (var item in _retriesItems)
            {
                item.RetriesCount++;
                if (item.RetriesCount < 6)
                {
                    MakeRetry(item, client);
                }
                else
                {
                    _bot.NotifyAboutInvalidPage(item.Product);
                    _retriesItems.Remove(item);
                }
            }
        }
    }

    private void MonitorGoods(object sender, ElapsedEventArgs e)
    {
        if (_products.Count > 0)
        {
            using HttpClient client = new();
            foreach (var product in _products)
            {
                MonitorGood(product, client);
            }
        }
        
        _count++;
        _bot.NotifyAboutCorrectWork(_count);
    }

    private bool CheckProductIsValid(Product product, HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode == false)
        {
            _retriesItems.Add(new RetriesItem {Product = product, RetriesCount = 0, FirstInvalidResponse = response});
            _products.Remove(product);
            return false;
        }

        return true;
    }

    private static bool CheckProductAvailability(Product product, string htmlText)
    {
        Regex regex = new Regex(SelectRegexPattern(product));
        return !regex.IsMatch(htmlText);
    }

    private static string SelectRegexPattern(Product product)
    {
        return product.MarketPlace switch
        {
            "Amazon" => AmazonProductUnavailablePattern,
            "Rozetka" => MakeUpProductUnavailablePattern,
            "MakeUp" => RozetkaProductUnavailablePattern,
            _ => string.Empty
        };
    }

    private void MonitorGood(Product product, HttpClient client)
    {
        HttpResponseMessage response = client.GetAsync(product.Url).Result;
        if (CheckProductIsValid(product, response))
        {
            string htmlText = response.Content.ReadAsStringAsync().Result;
            if (CheckProductAvailability(product, htmlText))
            {
                _bot.NotifyAboutProductAvailability(product);
            }
        }
    }

    private void MakeRetry(RetriesItem item, HttpClient client)
    {
        HttpResponseMessage response = item.RetriesCount == 1
            ? item.FirstInvalidResponse
            : client.GetAsync(item.Product.Url).Result;
        if (PageValidator.CheckPageIsValid(response))
        {
            _products.Add(item.Product);
            _retriesItems.Remove(item);
        }
    }
    
    private static ITelegramBot EnsureBotNotNull(ITelegramBot bot)
    {
        return bot ?? throw new ArgumentNullException(nameof(bot));
    }
}