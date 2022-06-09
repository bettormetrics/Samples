namespace Bettormetrics.Samples.LinkConsumer
{
    class Config
    {
        public string BlobConnectionString { get; set; }
        public string BlobContainerName { get; set; }
        public string EventHubConnectionString { get; set; }
        public string EventHubConsumerGroup { get; set; }
        public string EventHubName { get; set; }
    }
}
