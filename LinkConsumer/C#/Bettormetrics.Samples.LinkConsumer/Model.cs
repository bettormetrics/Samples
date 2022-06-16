using Bettormetrics.Samples.LinkConsumer.Entities;
using Bettormetrics.Samples.LinkConsumer.Nodes;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Bettormetrics.Samples.LinkConsumer
{
    class Model
    {
        public RootNode Root { get; }

        public Model()
        {
            Root = new RootNode();
        }

        public string Update(byte[] bytes)
        {
            var raw = Encoding.UTF8.GetString(bytes);

            var root = JsonConvert.DeserializeObject<MessageRoot>(raw);
            
            var result = root.Type switch
            {
                nameof(Fixture) => Update(Deserialize<Fixture>(raw)),
                nameof(Market) => Update(Deserialize<Market>(raw)),
                nameof(Selection) => Update(Deserialize<Selection>(raw)),
                nameof(BookmakersFixture) => Update(Deserialize<BookmakersFixture>(raw)),
                nameof(BookmakersMarket) => Update(Deserialize<BookmakersMarket>(raw)),
                nameof(BookmakersSelection) => Update(Deserialize<BookmakersSelection>(raw)),
                _ => false
            };

            return root.Type;
        }

        private bool Update(Message<Fixture> message)
        {
            var competition = Root.GetOrCreateChild(message.Entity.CompetitionId);
            competition.Update(new Message<Competition> { Entity = message.Entity.Competition, Type = nameof(Competition) });
            return competition
                .GetOrCreateChild(message.Entity.Id)
                .Update(message);
        }

        private bool Update(Message<Market> message)
        {
            return Root.GetOrCreateChild(message.Entity.CompetitionId)
                .GetOrCreateChild(message.Entity.FixtureId)
                .GetOrCreateChild(message.Entity.Id)
                .Update(message);
        }

        private bool Update(Message<Selection> message)
        {
            return Root.GetOrCreateChild(message.Entity.CompetitionId)
                .GetOrCreateChild(message.Entity.FixtureId)
                .GetOrCreateChild(message.Entity.MarketId)
                .GetOrCreateChild(message.Entity.Id)
                .Update(message);
        }

        private bool Update(Message<BookmakersFixture> message)
        {
            return Root.GetOrCreateChild(message.Entity.CompetitionId)
                .GetOrCreateChild(message.Entity.FixtureId)
                .UpdateLink(message);
        }

        private bool Update(Message<BookmakersMarket> message)
        {
            return Root.GetOrCreateChild(message.Entity.CompetitionId)
                .GetOrCreateChild(message.Entity.FixtureId)
                .GetOrCreateChild(message.Entity.MarketId)
                .UpdateLink(message);
        }

        private bool Update(Message<BookmakersSelection> message)
        {
            return Root.GetOrCreateChild(message.Entity.CompetitionId)
                .GetOrCreateChild(message.Entity.FixtureId)
                .GetOrCreateChild(message.Entity.MarketId)
                .GetOrCreateChild(message.Entity.SelectionId)
                .UpdateLink(message);
        }

        private static Message<T> Deserialize<T>(string raw)
        {
            return JsonConvert.DeserializeObject<Message<T>>(raw);
        }
    }
}
