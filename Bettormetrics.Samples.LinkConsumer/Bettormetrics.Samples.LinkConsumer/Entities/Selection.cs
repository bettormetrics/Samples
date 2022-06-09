﻿using System;

namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public class Selection : IEntity
    {
        public Guid Id { get; set; }
        public Guid MarketId { get; set; }
        public string Name { get; set; }
        public Guid FixtureId { get; set; }
        public int SportId { get; set; }
        public Guid CompetitionId { get; set; }
        public int MarketTypeId { get; set; }
        public int? MarketSubTypeId { get; set; }
        public string MarketOptions { get; set; }
        public bool IsOutright { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UtcDeleted { get; set; }
        public string GetId() => Id.ToString();
    }
}
