using System;

namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public class Competition
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public short? GenderId { get; set; }
        public int? AgeUnder { get; set; }
        public string CountryId { get; set; }
        public string CountrySubdivisionId { get; set; }
    }
}
