using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System.Threading.Tasks;

namespace MyWindowsFileService
{
    class Program
    {

        static async Task<int> Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(configureLogging => configureLogging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<MoveFileWorker>()
                        .Configure<EventLogSettings>(config =>
                        {
                            config.LogName = "My Windows File Service";
                            config.SourceName = "My Windows File Service";
                        });
                }).UseWindowsService();
    }
}
