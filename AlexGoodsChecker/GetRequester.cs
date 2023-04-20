using System.Net;
using System.Text.RegularExpressions;

namespace AlexGoodsChecker;

public static class GetRequester
{
    private const string AmazonHtmlTextPattern = "<span class=\"a-color-price a-text-bold\">Currently unavailable.</span>";
    private const string RozetkaHtmlTextPattern = "Нет в наличии";
    
    public static bool CheckProductAvailability(string url)
    {
        string htmlText = GetHttpText(url).Result;
        Regex regex = new Regex(AmazonHtmlTextPattern);
        return !regex.IsMatch(htmlText);
    }
    
    private static async Task<string> GetHttpText(string url)
    {
        using HttpClient client = new();
        Regex regex = new Regex("www.amazon.com");
        string htmlText = regex.IsMatch(url)
            ? await File.ReadAllTextAsync("Amazon.txt")
            : await client.GetStringAsync(url);
        return htmlText;
    }
    
    
    
}