using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    public class EntityOnSurfaceEvent : AbstractEvent
    {
        private IEntity entity;

        private EntityOnSurfaceEvent(object origin, IEntity entity)
            : base(origin, EventType.ENTITY_ON_SURFACE_EVENT)
        {
            this.entity = entity;
        }

        public static EntityOnSurfaceEvent Create(object origin, IEntity entity)
        {
            EntityOnSurfaceEvent returningEvent = EventManager.Instance.GetEventFromType<EntityOnSurfaceEvent>(EventType.ENTITY_ON_SURFACE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new EntityOnSurfaceEvent(origin, entity));
            }
            else
            {
                returningEvent.entity = entity;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public IEntity Entity
        {
            get
            {
                return this.entity;
            }
        }
    }
}
