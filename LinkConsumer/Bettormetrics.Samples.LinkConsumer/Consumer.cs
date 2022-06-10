using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bettormetrics.Samples.LinkConsumer
{
    class Consumer
    {
        private static readonly TimeSpan StatOutputWindow = TimeSpan.FromSeconds(1);

        private readonly BlobContainerClient checkpointClient;
        private readonly EventProcessorClient processorClient;
        private readonly SemaphoreSlim processSemaphore;
        private readonly Model model;
        private readonly Dictionary<string, int> messageTypeCounts;
        private DateTime utcLastStatOutputTime;

        public Consumer(Config config, Model model)
        {
            this.model = model;
            checkpointClient = new BlobContainerClient(config.BlobConnectionString, config.BlobContainerName);
            processorClient = new EventProcessorClient(checkpointClient, config.EventHubConsumerGroup, config.EventHubConnectionString, config.EventHubName);
            processorClient.ProcessEventAsync += ProcessEventHandler;
            processorClient.ProcessErrorAsync += ProcessErrorHandler;
            processSemaphore = new SemaphoreSlim(1, 1);
            messageTypeCounts = new Dictionary<string, int>();
            utcLastStatOutputTime = DateTime.MinValue;
        }

        public async Task StartAsync()
        {
            await checkpointClient.CreateIfNotExistsAsync();
            await processorClient.StartProcessingAsync();
        }

        public async Task StopAsync()
        {
            await processorClient.StopProcessingAsync();
        }

        private Task ProcessErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine($"Error: {arg}");
            return Task.CompletedTask;
        }

        private Task ProcessEventHandler(ProcessEventArgs arg)
        {
            processSemaphore.Wait();

            try
            {
                ProcessEvent(arg);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                processSemaphore.Release();
            }

            return Task.CompletedTask;
        }

        private void ProcessEvent(ProcessEventArgs arg)
        {
            var messageType = model.Update(arg.Data.EventBody.ToArray());

            if (messageType != null)
            {
                UpdateCount(messageType);
            }

            TryOutputStats();
        }

        private void UpdateCount(string messageType)
        {
            if (messageTypeCounts.ContainsKey(messageType))
            {
                messageTypeCounts[messageType]++;
            }
            else
            {
                messageTypeCounts[messageType] = 1;
            }
        }

        private void TryOutputStats()
        {
            if (DateTime.UtcNow - utcLastStatOutputTime < StatOutputWindow)
            {
                return;
            }

            OutputStats();
            utcLastStatOutputTime = DateTime.UtcNow;
        }

        private void OutputStats()
        {
            var numFixtures = 0;
            var numMarkets = 0;
            var numSelections = 0;

            foreach (var f in model.Root.Children)
            {
                numFixtures++;
                foreach (var m in f.Value.Children)
                {
                    numMarkets++;
                    foreach (var s in m.Value.Children)
                    {
                        numSelections++;
                    }
                }
            }

            var messageCounts = messageTypeCounts.Select(x => $"{x.Key}: {x.Value}");
            Console.WriteLine($"Stats Held: {numFixtures} fixtures, {numMarkets} markets, {numSelections} selections");
            Console.WriteLine($"Stats Messages: {string.Join(", ", messageCounts)}");
            Console.WriteLine();
        }
    }
}
