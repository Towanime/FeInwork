using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    public class DeadEvent : AbstractEvent
    {
        IEntity deadEntity;

        private DeadEvent(IEntity origin) :
            base(origin, EventType.DEAD_EVENT)
        {
            this.deadEntity = origin;
        }

        public static DeadEvent Create(IEntity origin)
        {
            DeadEvent returningEvent = EventManager.Instance.GetEventFromType<DeadEvent>(EventType.DEAD_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new DeadEvent(origin));
            }
            else
            {
                returningEvent.origin = origin;
                returningEvent.deadEntity = origin;
            }

            return returningEvent;
        }

        public IEntity DeadEntity
        {
            get
            {
                return this.deadEntity;
            }
        }
    }
}
