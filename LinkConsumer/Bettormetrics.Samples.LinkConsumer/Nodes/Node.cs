using Bettormetrics.Samples.LinkConsumer.Entities;
using System;
using System.Collections.Generic;

namespace Bettormetrics.Samples.LinkConsumer.Nodes
{
    abstract class Node<TEntity, TLinkEntity, TChildEntity, TChildLinkEntity, TChildNode> : Node<TEntity, TLinkEntity>
        where TEntity : IEntity
        where TLinkEntity : ILinkEntity
        where TChildEntity : IEntity
        where TChildLinkEntity : ILinkEntity
        where TChildNode : Node<TChildEntity, TChildLinkEntity>
    {
        public Dictionary<Guid, TChildNode> Children { get; }

        public Node()
        {
            Children = new Dictionary<Guid, TChildNode>();
        }

        public TChildNode GetOrCreateChild(Guid key)
        {
            if (!Children.ContainsKey(key))
            {
                Children[key] = Activator.CreateInstance<TChildNode>();
            }

            return Children[key];
        }
    }

    abstract class Node<TEntity, TLinkEntity>
        where TEntity : IEntity
        where TLinkEntity : ILinkEntity
    {
        public TEntity Entity { get; set; }
        public Dictionary<int, TLinkEntity> Links { get; set; }

        public Node()
        {
            Links = new Dictionary<int, TLinkEntity>();
        }

        public bool Update(Message<TEntity> message)
        {
            Entity = message.Entity;
            return true;
        }

        public bool UpdateLink(Message<TLinkEntity> message)
        {
            Links[message.Entity.BookmakerId] = message.Entity;
            return true;
        }
    }

    abstract class Node
    {
    }
}
