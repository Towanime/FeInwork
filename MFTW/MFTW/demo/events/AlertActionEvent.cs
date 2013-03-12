using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    public class AlertSpecialActionEvent : AbstractEvent
    {
        private IEntity requestingEntity;
        private ActionType actionType;

        private AlertSpecialActionEvent(object origin, IEntity requestingEntity, ActionType actionType)
            : base(origin, EventType.ALERT_ACTION_EVENT)
        {
            this.requestingEntity = requestingEntity;
            this.actionType = actionType;
        }

        public static AlertSpecialActionEvent Create(object origin, IEntity requestingEntity, ActionType actionType)
        {
            AlertSpecialActionEvent returningEvent = EventManager.Instance.GetEventFromType<AlertSpecialActionEvent>(EventType.ALERT_ACTION_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new AlertSpecialActionEvent(origin, requestingEntity, actionType));
            }
            else
            {
                returningEvent.actionType = actionType;
                returningEvent.requestingEntity = requestingEntity;
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
