using Bettormetrics.Samples.LinkConsumer.Entities;
using System;
using System.Collections.Generic;

namespace Bettormetrics.Samples.LinkConsumer.Nodes
{
    abstract class Node<TEntity, TLinkEntity, TChildEntity, TChildLinkEntity, TChildNode> : NodeB<TEntity, TLinkEntity, TChildEntity, TChildNode>
        where TLinkEntity : ILinkEntity
        where TChildLinkEntity : ILinkEntity
        where TChildNode : Node<TChildEntity, TChildLinkEntity>
    {
    }

    abstract class NodeB<TEntity, TLinkEntity, TChildEntity, TChildNode> : Node<TEntity, TLinkEntity>
        where TLinkEntity : ILinkEntity
        where TChildNode : Node<TChildEntity>
    {
        public Dictionary<Guid, TChildNode> Children { get; }

        public NodeB()
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

    abstract class Node<TEntity, TChildEntity, TChildLinkEntity, TChildNode> : Node<TEntity>
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

    abstract class Node<TEntity, TLinkEntity> : Node<TEntity>
        where TLinkEntity : ILinkEntity
    {
        public Dictionary<int, TLinkEntity> Links { get; set; }

        public Node()
        {
            Links = new Dictionary<int, TLinkEntity>();
        }

        public bool UpdateLink(Message<TLinkEntity> message)
        {
            Links[message.Entity.BookmakerId] = message.Entity;
            return true;
        }
    }

    abstract class Node<TEntity>
    {
        public TEntity Entity { get; set; }

        public bool Update(Message<TEntity> message)
        {
            Entity = message.Entity;
            return true;
        }
    }
}
