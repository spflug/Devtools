using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            for (int i = 0; i < 20; ++i)
                using (var client = new HttpClient())
                {
                    await client.GetAsync(new Uri("https://www.google.de"));
                }

            Console.WriteLine("Hello World!");
            Console.ReadKey();
            var f = Foo.A;

            var x = f switch
            {
                Foo.A => "",
                Foo.B => "SD<f",
                //_ => throw new Exception()
            };

            var foo = args switch
            {
                { Length: 0 }  => throw new Exception(),
                _ => 1
            };
        }

        enum Foo
        {
            A,B
        }
    }

    


}
