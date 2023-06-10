using System.Text.RegularExpressions;
using System.Timers;
using Timer = System.Timers.Timer;
using AlexGoodsChecker.Interfaces;

namespace AlexGoodsChecker;

public class GoodsChecker : IGoodsChecker
{
    private const string AmazonHtmlTextPattern =
        "<span class=\"a-color-price a-text-bold\">Currently unavailable.</span>";

    private const string MakeUpHtmlTextPattern =
        "<div class=\"product-item__status red\" id=\"product_enabled\">Nu este disponibil</div>";

    private const string RozetkaHtmlTextPattern =
        "type=\"button\" class=\"button button--medium button--navy ng-star-inserted\">";

    private const double MonitoringInterval = 500000;

    private readonly ITelegramBot _bot;
    private readonly List<Product> _products;
    private List<RetriesItem> _retriesItems;
    private readonly HttpClient _client;

    private int _count;

    public GoodsChecker(List<Product> products, HttpClient client, ITelegramBot bot)
    {
        _products = products ?? new List<Product>();
        _client = client;
        _bot = bot;
    }

    public void Start()
    {
        _bot?.Start();
        Timer timer = new(MonitoringInterval);
        timer.Elapsed += Start_Monitoring;
        timer.Elapsed += Start_Retries;
        timer.Start();
    }

    private void Start_Retries(object sender, ElapsedEventArgs e)
    {
        if (_retriesItems != null && _retriesItems.Count > 0)
        {
            foreach (var item in _retriesItems)
            {
                item!.RetriesCount++;
                if (item.RetriesCount < 6)
                {
                    MakeRetry(item);
                }
                else
                {
                    _bot?.NotifyAboutInvalidPage(item.Product);
                    _retriesItems?.Remove(item);
                }
            }
        }
    }

    private void Start_Monitoring(object sender, ElapsedEventArgs e)
    {
        _count++;
        foreach (var product in _products!)
        {
            HttpResponseMessage response = _client?.GetAsync(product?.Url).Result;
            if (CheckProductIsValid(product, response))
            {
                string htmlText = response?.Content.ReadAsStringAsync().Result;
                if (CheckProductAvailability(product, htmlText))
                {
                    _bot?.NotifyAboutProductAvailability(product);
                }
            }
        }

        _bot?.NotifyAboutCorrectWork(_count);
    }

    private bool CheckProductIsValid(Product product, HttpResponseMessage response)
    {
        if (response is {IsSuccessStatusCode: false})
        {
            _retriesItems = new();
            _retriesItems?.Add(new() {Product = product, RetriesCount = 0, FirstInvalidResponse = response});
            _products?.Remove(product);
            return false;
        }

        return true;
    }

    private static bool CheckProductAvailability(Product product, string htmlText)
    {
        Regex regex = new Regex(SelectRegexPattern(product) ?? string.Empty);
        return htmlText != null && !regex.IsMatch(htmlText);
    }

    private static string SelectRegexPattern(Product product)
    {
        if (product == null)
        {
            return string.Empty;
        }

        return product.MarketPlace switch
        {
            "Amazon" => AmazonHtmlTextPattern,
            "Rozetka" => RozetkaHtmlTextPattern,
            "MakeUp" => MakeUpHtmlTextPattern,
            _ => string.Empty
        };
    }

    private void MakeRetry(RetriesItem item)
    {
        HttpResponseMessage response = item?.RetriesCount == 1
            ? item.FirstInvalidResponse
            : _client?.GetAsync(item?.Product?.Url).Result;
        if (PageValidator.CheckPageIsValid(response))
        {
            _products?.Add(item?.Product);
            _retriesItems?.Remove(item);
        }
    }
}