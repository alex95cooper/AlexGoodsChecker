using System.Text.RegularExpressions;

namespace AlexGoodsChecker;

public static class GetRequester
{
    private const string AmazonHtmlTextPattern =
        "<span class=\"a-color-price a-text-bold\">Currently unavailable.</span>";

    private const string MakeUpHtmlTextPattern =
        "<div class=\"product-item__status red\" id=\"product_enabled\">Nu este disponibil</div>";

    private const string RozetkaHtmlTextPattern =
        "type=\"button\" class=\"button button--medium button--navy ng-star-inserted\">";

    public static bool CheckProductAvailability(Product product, string htmlText)
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
}