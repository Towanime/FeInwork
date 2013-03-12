using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.core.collision.bodies;
using FeInwork.Core.Util;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    /// <summary>
    /// Evento para manejar cuando una entidad entra o sale del rango de un trigger.
    /// </summary>
    public class TriggerRangeEvent : AbstractEvent
    {
        /// <summary>
        /// CollisionBody del trigger.
        /// De este mismo objeto se puede obtener el Entity.
        /// </summary>
        private CollisionBody collisionBody;
        /// <summary>
        /// Entidad que activa el trigger
        /// </summary>
        private IEntity triggerEntity;
        /// <summary>
        /// True si la entidad esta en el rango del trigger.
        /// False para indicar que la "triggerEntity" ha salido del
        /// rango del trigger.
        /// </summary>
        private bool isInRange;

        private TriggerRangeEvent(CollisionBody collisionBody, IEntity triggerEntity, bool isInRange)
            : base(collisionBody.Owner, isInRange ? EventType.TRIGGER_IN_RANGE_EVENT : EventType.TRIGGER_OUT_RANGE_EVENT)
        {
            this.triggerEntity = triggerEntity;
            this.collisionBody = collisionBody;
            this.isInRange = isInRange;
        }

        public static TriggerRangeEvent Create(CollisionBody collisionBody, IEntity triggerEntity, bool isInRange)
        {
            TriggerRangeEvent returningEvent = EventManager.Instance.GetEventFromType<TriggerRangeEvent>(isInRange ? EventType.TRIGGER_IN_RANGE_EVENT : EventType.TRIGGER_OUT_RANGE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new TriggerRangeEvent(collisionBody, triggerEntity, isInRange));
            }
            else
            {
                returningEvent.collisionBody = collisionBody;
                returningEvent.triggerEntity = triggerEntity;
                returningEvent.isInRange = isInRange;
                returningEvent.origin = collisionBody.Owner;
            }

            return returningEvent;
        }

        public bool IsInRange
        {
            get { return this.isInRange; }
        }

        public CollisionBody CollisionBody
        {
            get { return this.collisionBody; }
        }

        public IEntity TriggerEntity
        {
            get { return this.triggerEntity; }
        }
    }
}
