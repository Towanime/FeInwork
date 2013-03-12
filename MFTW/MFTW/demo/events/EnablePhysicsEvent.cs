using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    public class EnablePhysicsEvent : AbstractEvent
    {
        private bool enablePhysics;

        private EnablePhysicsEvent(object origin, bool enablePhysics)
            : base(origin, EventType.ENABLE_PHYSICS_EVENT)
        {
            this.enablePhysics = enablePhysics;
        }

        public static EnablePhysicsEvent Create(object origin, bool enablePhysics)
        {
            EnablePhysicsEvent returningEvent = EventManager.Instance.GetEventFromType<EnablePhysicsEvent>(EventType.ENABLE_PHYSICS_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new EnablePhysicsEvent(origin, enablePhysics));
            }
            else
            {
                returningEvent.enablePhysics = enablePhysics;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public bool EnablePhysics
        {
            get
            {
                return this.enablePhysics;
            }
        }
    }
}
