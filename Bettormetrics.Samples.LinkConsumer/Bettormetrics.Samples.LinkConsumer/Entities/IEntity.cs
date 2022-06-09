using System;

namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public interface IEntity
    {
        bool IsDeleted { get; set; }
        DateTime UtcDeleted { get; set; }
    }
}
