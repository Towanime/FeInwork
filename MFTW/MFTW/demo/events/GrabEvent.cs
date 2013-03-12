using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.Core.Interfaces;
using AlchemistDemo.core.collision.bodies;

namespace AlchemistDemo.alchemist.events
{
    public class GrabEvent : AbstractEvent
    {
        public IEntity grabbingEntity;
        public IEntity grabbedEntity;
        public List<CollisionBody> grabbedBodies;
        public bool begin;

        public GrabEvent(object origin, IEntity grabbingEntity, IEntity grabbedEntity, List<CollisionBody> grabbedBodies, bool begin)
            : base(origin, EventType.TYPE.GRAB_EVENT)
        {
            this.grabbingEntity = grabbingEntity;
            this.grabbedEntity = grabbedEntity;
            this.grabbedBodies = grabbedBodies;
            this.begin = begin;
        }
    }
}
