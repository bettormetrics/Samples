using System;

namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public class Participant
    {
        public Guid Id { get; set; }
        public byte? GenderId { get; set; }
        public byte? UnderAge { get; set; }
        public string Name { get; set; }
    }
}
