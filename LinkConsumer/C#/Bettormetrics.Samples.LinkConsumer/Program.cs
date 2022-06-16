using Newtonsoft.Json;
using System;
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

            Console.WriteLine("Config loaded.");

            var model = new Model();
            var consumer = new Consumer(config, model);

            Console.WriteLine("Model and Consumer created.");

            await consumer.StartAsync();

            Console.WriteLine("Consumer started.");

            await Task.Delay(int.MaxValue);

            await consumer.StopAsync();
        }
    }
}
