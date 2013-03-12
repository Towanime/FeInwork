using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlchemistDemo.core.collision.bodies;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using AlchemistDemo.core.managers;
using Microsoft.Xna.Framework;

namespace AlchemistDemo.alchemist.components.interfaces
{
    public abstract class AbstractCollisionComponent : BaseComponent
    {
        private List<CollisionBody> bodyList;

        public List<CollisionBody> BodyList
        {
            get
            {
                return this.bodyList;
            }
            set
            {
                this.bodyList = value;
            }
        }

        public AbstractCollisionComponent(IEntity owner, List<CollisionBody> bodyList)
            : base(owner)
        {
            this.bodyList = bodyList;
        }

        public AbstractCollisionComponent(IEntity owner, CollisionBody body)
            : base(owner)
        {
            this.bodyList = new List<CollisionBody>();
            this.bodyList.Add(body);
        }

        public void reSubscribe()
        {
            for (int i = 0; i < this.bodyList.Count; i++)
            {
                CollisionManager.Instance.removeContainer(this.bodyList[i]);
                CollisionManager.Instance.addContainer(this.bodyList[i]);
            }
        }

        public abstract void addBody(CollisionBody body);

        public abstract void addBodies(List<CollisionBody> bodyList);

        public abstract void removeBody(CollisionBody body);

        public abstract void removeBodies(List<CollisionBody> bodyList);

        public virtual void move(Vector2 oldPos, Vector2 newPos)
        {
            for (int i = 0; i < this.BodyList.Count; i++)
            {
                this.BodyList[i].OffsetRelativeTo(oldPos, newPos);
            }
        }

        internal void subscribe()
        {
            throw new NotImplementedException();
        }

        internal void deSubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
