using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.collision.bodies;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.core.managers;
using Microsoft.Xna.Framework;

namespace FeInwork.FeInwork.components.interfaces
{
    public abstract class AbstractCollisionComponent : BaseComponent
    {
        protected List<CollisionBody> bodyList;

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

        public virtual void addBody(CollisionBody shape)
        {
            if (!this.bodyList.Contains(shape))
            {
                this.bodyList.Add(shape);
                CollisionManager.Instance.addContainer(shape);
            }
        }

        public virtual void addBodies(List<CollisionBody> shapeList)
        {
            for (int i = 0; i < shapeList.Count; i++)
            {
                CollisionBody shape = shapeList[i];
                if (!this.bodyList.Contains(shape))
                {
                    this.bodyList.Add(shape);
                    CollisionManager.Instance.addContainer(shape);
                }
            }
        }

        public virtual void removeBody(CollisionBody shape)
        {
            if (this.bodyList.Remove(shape))
            {
                CollisionManager.Instance.removeContainer(shape);
            }
        }

        public virtual void removeBodies(List<CollisionBody> shapeList)
        {
            for (int i = shapeList.Count - 1; i >= 0; i--)
            {
                CollisionBody shape = shapeList[i];
                if (this.bodyList.Remove(shape))
                {
                    CollisionManager.Instance.removeContainer(shape);
                }
            }
        }

        public virtual void move(Vector2 oldPos, Vector2 newPos)
        {
            for (int i = 0; i < this.BodyList.Count; i++)
            {
                this.bodyList[i].OffsetRelativeTo(oldPos, newPos);
            }
        }

        internal void subscribe()
        {
            //throw new NotImplementedException();
            for (int i = 0; i < this.bodyList.Count; i++)
            {
                CollisionManager.Instance.addContainer(this.bodyList[i]);
            }
        }

        internal void deSubscribe()
        {
            //throw new NotImplementedException();
            for (int i = 0; i < this.bodyList.Count; i++)
            {
                CollisionManager.Instance.removeContainer(this.bodyList[i]);
            }
        }
    }
}
