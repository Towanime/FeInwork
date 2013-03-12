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
    public class SurfaceContactEvent : AbstractEvent
    {
        private SurfaceContactEvent(object origin) :
            base(origin, EventType.SURFACE_CONTACT_EVENT)
        {
        }

        public static SurfaceContactEvent Create(object origin)
        {
            SurfaceContactEvent returningEvent = EventManager.Instance.GetEventFromType<SurfaceContactEvent>(EventType.SURFACE_CONTACT_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new SurfaceContactEvent(origin));
            }
            else
            {
                returningEvent.origin = origin;
            }

            return returningEvent;
        }
    }
}