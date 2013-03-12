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
    public class CureEvent: AbstractEvent
    {
        private int cure;

        public int Cure
        {
            get { return cure; }
        }

        private CureEvent(object origin, int cure) :
            base(origin, EventType.CURE_EVENT)
        {
            this.cure = cure;
        }

        public static CureEvent Create(object origin, int cure)
        {
            CureEvent returningEvent = EventManager.Instance.GetEventFromType<CureEvent>(EventType.CURE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new CureEvent(origin, cure));
            }
            else
            {
                returningEvent.cure = cure;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }
    }
}
