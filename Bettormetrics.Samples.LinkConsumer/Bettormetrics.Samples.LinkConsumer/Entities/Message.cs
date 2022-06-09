namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public class Message<T> : MessageRoot
    {
        public T Entity { get; set; }
    }
}
