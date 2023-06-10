using System.Runtime;

namespace AlexGoodsChecker;

public class RetriesItem
{
    public Product Product { get; set; }
    public int RetriesCount { get; set; }
    public HttpResponseMessage FirstInvalidResponse { get; set; }
}