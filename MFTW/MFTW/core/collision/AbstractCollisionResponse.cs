using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.listeners;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using Microsoft.Xna.Framework;

namespace FeInwork.core.collision
{
    public abstract class AbstractCollisionResponse : CollisionListener
    {
        protected IEntity owner;
        protected Color? colorTag = null;

        protected AbstractCollisionResponse(IEntity owner)
        {
            this.owner = owner;
            this.initialize();
        }

        protected AbstractCollisionResponse(IEntity owner, Color colorTag)
        {
            this.owner = owner;
            this.colorTag = colorTag;
            this.initialize();
        }

        public Color? ColorTag
        {
            get
            {
                return this.colorTag;
            }
        }

        public virtual void initialize()
        {
         //   EventManager.Instance.addListener(EventType.COLLISION_EVENT, this.owner, this);
            // not anymore
           // Program.GAME.ComponentManager.addComponent(this);
        }

        public abstract void invoke(CollisionEvent eventObject);
    }
}
