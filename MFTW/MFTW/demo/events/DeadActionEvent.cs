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
    public class DeadActionEvent : AbstractEvent
    {
        private bool isEndOfAction;
        private object actionObject;

        public bool IsEndOfAction
        {
            get
            {
                return this.isEndOfAction;
            }
        }

        public object ActionObject
        {
            get
            {
                return this.actionObject;
            }
        }

        private DeadActionEvent(object origin, bool isEndOfAction, object actionObject) :
            base(origin, EventType.DEAD_ACTION_EVENT)
        {
            this.isEndOfAction = isEndOfAction;
            this.actionObject = actionObject;
        }

        public static DeadActionEvent Create(object origin, bool isEndOfAction, object actionObject)
        {
            DeadActionEvent returningEvent = EventManager.Instance.GetEventFromType<DeadActionEvent>(EventType.DEAD_ACTION_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new DeadActionEvent(origin, isEndOfAction, actionObject));
            }
            else
            {
                returningEvent.isEndOfAction = isEndOfAction;
                returningEvent.actionObject = actionObject;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }
    }
}
