using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NodeWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                List<string> arguments = args.ToList();
                var enumerator = arguments.GetEnumerator();
                string currentArgument = enumerator.Current;
                IDictionary<string, string> settingDic = new Dictionary<string, string>(args.Length);
                while (enumerator.MoveNext())
                {
                    string next = enumerator.Current;
                    if (currentArgument.IndexOf("-") > 0)
                    {
                        settingDic[currentArgument] = next;
                        if (enumerator.MoveNext())
                        {
                            continue;
                        }
                        break;
                    }
                    currentArgument = next;
                }

                if (args.Length > 0 && args.Contains("-p"))
                {
                    int port = 5000;
                    var dic = args.ToDictionary(x => x);
                    var keys = dic.Keys.ToList();
                    var portIndex = keys.IndexOf("-p") + 1;
                    if (portIndex <= args.Length)
                    {
                        var portKey = keys[portIndex];
                        int.TryParse(dic[portKey], out port);
                        if (port > 65535 || port < 5000)
                        {
                            port = 5000;
                        }
                    }
                    webBuilder
                        .UseStartup<Startup>()
                        .UseUrls($"http://*:{port}");
                }
                else
                {
                    webBuilder.UseStartup<Startup>();
                }
            });
    }
}