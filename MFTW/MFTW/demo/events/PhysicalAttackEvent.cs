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
    public class PhysicalAttackEvent: AbstractEvent
    {
        private PhysicalAttackEvent(object origin) :
            base(origin, EventType.PHYSICAL_ATTACK_EVENT)
        {
        }

        public static PhysicalAttackEvent Create(object origin)
        {
            PhysicalAttackEvent returningEvent = EventManager.Instance.GetEventFromType<PhysicalAttackEvent>(EventType.PHYSICAL_ATTACK_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new PhysicalAttackEvent(origin));
            }
            else
            {
                returningEvent.origin = origin;
            }

            return returningEvent;
        }
    }
}
