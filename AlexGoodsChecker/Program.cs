// See https://aka.ms/new-console-template for more information

using AlexGoodsChecker;
string urlAddress = "https://www.amazon.com/Eric-Robertson-Christmas-Album-Vinyl/dp/B00J1197AI/ref=sr_1_167?crid=1JHK3PQ0HOSUG&keywords=metallica+black+album+vinyl&qid=1681921889&sprefix=metallica+black+%2Caps%2C225&sr=8-167";
bool result = GetRequester.CheckProductAvailability(urlAddress);
Console.WriteLine(result);
//File.WriteAllText("C:\\E\\Програмирование\\Amazon1.txt", result);
Console.ReadKey();