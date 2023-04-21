using System.Text.Json;
using AlexGoodsChecker;

List<Product> products = new();
using (StreamReader reader = new StreamReader("appsettings.json"))  
{  
    string json = reader.ReadToEnd();  
    products = JsonSerializer.Deserialize<List<Product>>(json);  
}

foreach (Product product in products)
{
    Console.WriteLine("--------------------------------------------------");
    Console.WriteLine(product.Id);
    Console.WriteLine(product.MarketPlace);
    Console.WriteLine(product.Url);
}
Console.WriteLine("result");
//File.WriteAllText("C:\\E\\Програмирование\\Amazon1.txt", result);
Console.ReadKey();


