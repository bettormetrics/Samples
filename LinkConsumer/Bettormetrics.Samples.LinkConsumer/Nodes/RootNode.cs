using Bettormetrics.Samples.LinkConsumer.Entities;
using System;

namespace Bettormetrics.Samples.LinkConsumer.Nodes
{
    class RootNode : NodeB<Root, Root, Competition, CompetitionNode>
    {
    }

    class Root : ILinkEntity
    {
        public int BookmakerId { get; set; }
        public int SportId { get; set; }
        public Guid CompetitionId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UtcDeleted { get; set; }
        public string GetId() => null;
    }
}
