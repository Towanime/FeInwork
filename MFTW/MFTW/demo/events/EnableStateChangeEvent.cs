using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.core.interfaces;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    public class EnableStateChangeEvent : AbstractEvent
    {
        private EnableStateChangeEvent(object origin) :
            base(origin, EventType.ENABLE_STATE_CHANGE_EVENT)
        {
           // this.item = item;
        }

        public static EnableStateChangeEvent Create(object origin)
        {
            EnableStateChangeEvent returningEvent = EventManager.Instance.GetEventFromType<EnableStateChangeEvent>(EventType.ENABLE_STATE_CHANGE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new EnableStateChangeEvent(origin));
            }
            else
            {
                returningEvent.origin = origin;
            }

            return returningEvent;
        }
    }
}
