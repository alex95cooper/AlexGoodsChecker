namespace AlexGoodsChecker.Extensions;

public static class HttpClientExtensions
{
    public static async Task<string> GetHttpText(this HttpClient client, Product product)
    {
        if (product == null || client == null) return default;
        string htmlText = product.MarketPlace == "Amazon"
            ? await File.ReadAllTextAsync("Amazon.txt")
            : await client.GetStringAsync(product.Url);
        return htmlText;
    }
}