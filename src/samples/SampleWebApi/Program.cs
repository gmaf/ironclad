namespace SampleWebApi
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args) => WebHost.CreateDefaultBuilder(args).UseUrls("http://+:5007").UseStartup<Startup>().Build().Run();
    }
}
