using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Util;
using FeInwork.core.collision;
using Microsoft.Xna.Framework;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    public class CollisionEvent : AbstractEvent
    {
        private IEntity triggeringEntity;
        private IEntity affectedEntity;
        private CollisionResult collisionResult;

        private CollisionEvent(object origin, IEntity triggeringEntity, IEntity affectedEntity, CollisionResult collisionResult)
            : base(origin, EventType.COLLISION_EVENT)
        {
            this.triggeringEntity = triggeringEntity;
            this.affectedEntity = affectedEntity;
            this.collisionResult = collisionResult;
        }

        public static CollisionEvent Create(object origin, IEntity triggeringEntity, IEntity affectedEntity, CollisionResult collisionResult)
        {
            CollisionEvent returningEvent = EventManager.Instance.GetEventFromType<CollisionEvent>(EventType.COLLISION_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new CollisionEvent(origin, triggeringEntity, affectedEntity, collisionResult));
            }
            else
            {
                returningEvent.triggeringEntity = triggeringEntity;
                returningEvent.affectedEntity = affectedEntity;
                returningEvent.collisionResult = collisionResult;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public IEntity TriggeringEntity
        {
            get { return this.triggeringEntity; }
        }

        public IEntity AffectedEntity
        {
            get { return this.affectedEntity; }
        }

        public CollisionResult CollisionResult
        {
            get { return this.collisionResult; }
        }

    }
}
