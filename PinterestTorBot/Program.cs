using System;
using System.Text;
using System.Threading.Tasks;

namespace PinterestTorBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Console.WriteLine("Kaç hesap oluşturulacak?");
            // var count = Convert.ToInt32(Console.ReadLine());

            // Console.WriteLine("Kaçar api oluşturulacak? (Boş bırakılabilir)");
            // var apiCount = int.TryParse(Console.ReadLine(), out var readedCount) ? readedCount : 0;

            // new Tor().Run(count, apiCount);

            await new ArticleFinder().Run2();
        }
    }
}