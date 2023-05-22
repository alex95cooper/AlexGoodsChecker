using System.Text.Json;
using AlexGoodsChecker.Interfaces;

namespace AlexGoodsChecker;

public class Configurator : IConfigurator
{
    public List<Product> Deserialize()
    {
        using StreamReader reader = new StreamReader("appsettings.json");
        string json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<List<Product>>(json);
    }
}