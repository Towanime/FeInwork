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
    public class PerformSpecialActionEvent : AbstractEvent
    {
        private IEntity requestingEntity;
        private ActionType actionType;

        private PerformSpecialActionEvent(object origin, IEntity requestingEntity, ActionType actionType)
            : base(origin, EventType.PERFORM_SPECIAL_ACTION_EVENT)
        {
            this.requestingEntity = requestingEntity;
            this.actionType = actionType;
        }

        public static PerformSpecialActionEvent Create(object origin, IEntity requestingEntity, ActionType actionType)
        {
            PerformSpecialActionEvent returningEvent = EventManager.Instance.GetEventFromType<PerformSpecialActionEvent>(EventType.PERFORM_SPECIAL_ACTION_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new PerformSpecialActionEvent(origin, requestingEntity, actionType));
            }
            else
            {
                returningEvent.requestingEntity = requestingEntity;
                returningEvent.actionType = actionType;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public IEntity RequestingEntity
        {
            get { return this.requestingEntity; }
        }

        public ActionType ActionType
        {
            get { return this.actionType; }
        }
    }
}
