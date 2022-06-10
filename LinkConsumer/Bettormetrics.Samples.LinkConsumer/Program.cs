using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Bettormetrics.Samples.LinkConsumer
{
    class Program
    {
        static async Task Main()
        {
            var configFile = await File.ReadAllTextAsync("config.json");
            var config = JsonConvert.DeserializeObject<Config>(configFile);

            var model = new Model();
            var consumer = new Consumer(config, model);

            await consumer.StartAsync();

            await Task.Delay(int.MaxValue);

            await consumer.StopAsync();
        }
    }
}
