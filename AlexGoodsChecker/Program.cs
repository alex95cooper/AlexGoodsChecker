using System.Text.Json;
using AlexGoodsChecker;

using StreamReader reader = new StreamReader("appsettings.json");
string json = reader.ReadToEnd();
List<Product> products = JsonSerializer.Deserialize<List<Product>>(json);

int count = 0;
using HttpClient client = new();
while (true)
{
    count++;
    if (products != null)
        foreach (Product product in products)
        {
            if (product != null)
            {
                if (GetRequester.CheckProductAvailability(product, client))
                {
                    Console.WriteLine($"Item with code {product.Id} appeared in the store {product.MarketPlace}");
                }
            }
        }

    Console.WriteLine("");
    Console.WriteLine($"The application is working correctly. A total of {count} requests were made per day.");
    Thread.Sleep(500000);
}
