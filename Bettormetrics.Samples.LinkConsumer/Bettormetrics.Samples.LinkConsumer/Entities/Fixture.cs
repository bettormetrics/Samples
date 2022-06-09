using System;
using System.Collections.Generic;

namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public class Fixture : IEntity
    {
        public Guid Id { get; set; }
        public int SportId { get; set; }
        public Guid CompetitionId { get; set; }
        public Competition Competition { get; set; }
        public List<FixtureParticipant> Participants { get; set; }
        public string CountryId { get; set; }
        public string CountrySubdivisionId { get; set; }
        public DateTime UtcStart { get; set; }
        public DateTime UtcCreated { get; set; }
        public DateTime UtcUpdated { get; set; }
        public bool IsOutright { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UtcDeleted { get; set; }
        public string GetId() => Id.ToString();
    }
}
