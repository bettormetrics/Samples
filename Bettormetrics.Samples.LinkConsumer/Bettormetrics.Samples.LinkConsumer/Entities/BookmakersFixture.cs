using System;
using System.Collections.Generic;

namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public class BookmakersFixture : IEntity
    {
        public int BookmakerId { get; set; }
        public Guid? FixtureId { get; set; }
        public string BookmakerFixtureId { get; set; }
        public int SportId { get; set; }
        public Guid CompetitionId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UtcDeleted { get; set; }
        public List<string> AlternativeBookmakerIds { get; set; }
        public List<DateTime> UtcAlternativeBookmakerIdsUpdated { get; set; }
        public DateTime? UtcLinkUpdated { get; set; }
    }
}
