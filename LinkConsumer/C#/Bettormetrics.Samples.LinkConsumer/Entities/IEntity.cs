using System;

namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public interface IEntity
    {
        public int SportId { get; set; }
        public Guid CompetitionId { get; set; }
        bool IsDeleted { get; set; }
        DateTime? UtcDeleted { get; set; }
        string GetId();
    }
}
