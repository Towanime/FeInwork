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
    /* Evento que se disparará al añadir una entidad al mundo.
     * Util para hacer cosas asegurandose de que se ejecutaran al final */
    public class EntityAddedEvent : AbstractEvent
    {
        IEntity addedEntity;

        private EntityAddedEvent(object origin, IEntity addedEntity) : base(origin, EventType.ENTITY_ADDED_EVENT)
        {
            this.addedEntity = addedEntity;
        }

        public static EntityAddedEvent Create(object origin, IEntity addedEntity)
        {
            EntityAddedEvent returningEvent = EventManager.Instance.GetEventFromType<EntityAddedEvent>(EventType.ENTITY_ADDED_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new EntityAddedEvent(origin, addedEntity));
            }
            else
            {
                returningEvent.origin = origin;
                returningEvent.addedEntity = addedEntity;
            }

            return returningEvent;
        }

        public IEntity AddedEntity
        {
            get { return this.addedEntity; }
        }
    }
}
