using System.Text.RegularExpressions;

namespace AlexGoodsChecker;

public static class PageValidator
{
    private const string ValidPagePattern = "(<div id=\"buybox\">|<rz-product-main-info>|<div class=\"product-item\">)";

    public static bool CheckPageIsValid(HttpResponseMessage response)
    {
        if (response != null && response.IsSuccessStatusCode)
        {
            Regex regex = new(ValidPagePattern);
            string htmlText = response.Content.ReadAsStringAsync().Result;
            return regex.IsMatch(htmlText);
        }
        
        return false;
    }
}