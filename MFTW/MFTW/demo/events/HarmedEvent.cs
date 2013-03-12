using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.Core.Base;

namespace FeInwork.FeInwork.events
{
    /// <summary>
    /// Evento que se envia cuando se le hace daño a una entidad (la vida disminuye)
    /// </summary>
    public class HarmedEvent : AbstractEvent
    {
        private HarmedEvent(object origin) :
            base(origin, EventType.HARMED_EVENT)
        {
        }

        public static HarmedEvent Create(object origin)
        {
            HarmedEvent returningEvent = EventManager.Instance.GetEventFromType<HarmedEvent>(EventType.HARMED_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new HarmedEvent(origin));
            }
            else
            {
                returningEvent.origin = origin;
            }

            return returningEvent;
        }
    }
}
