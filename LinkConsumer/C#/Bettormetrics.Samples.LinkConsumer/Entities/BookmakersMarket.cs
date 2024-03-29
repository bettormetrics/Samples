﻿using System;
using System.Collections.Generic;

namespace Bettormetrics.Samples.LinkConsumer.Entities
{
    public class BookmakersMarket : ILinkEntity
    {
        public int BookmakerId { get; set; }
        public Guid MarketId { get; set; }
        public string BookmakerMarketId { get; set; }
        public Guid FixtureId { get; set; }
        public int SportId { get; set; }
        public Guid CompetitionId { get; set; }
        public int MarketTypeId { get; set; }
        public int? MarketSubTypeId { get; set; }
        public string MarketOptions { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? UtcDeleted { get; set; }
        public bool IsOutright { get; set; }
        public List<string> AlternativeBookmakerIds { get; set; }
        public List<DateTime> UtcAlternativeBookmakerIdsUpdated { get; set; }
        public List<MarketLink> MarketLinks { get; set; }
        public DateTime? UtcLinkUpdated { get; set; }
        public string GetId() => $"{MarketId}_{BookmakerId}";
    }
}
