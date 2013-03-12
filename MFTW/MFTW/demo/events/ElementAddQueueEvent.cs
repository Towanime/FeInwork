using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.util;

namespace FeInwork.FeInwork.events
{
    public class ElementAddQueueEvent : AbstractEvent
    {
        private ElementType element;
        private double angle;

        private ElementAddQueueEvent(object origin, ElementType element, double angle)
            : base(origin, EventType.ELEMENT_QUEUE_EVENT)
        {
            this.element = element;
            this.angle = angle;
        }

        public static ElementAddQueueEvent Create(object origin, ElementType element, double angle)
        {
            ElementAddQueueEvent returningEvent = EventManager.Instance.GetEventFromType<ElementAddQueueEvent>(EventType.ELEMENT_QUEUE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new ElementAddQueueEvent(origin, element, angle));
            }
            else
            {
                returningEvent.element = element;
                returningEvent.origin = origin;
                returningEvent.angle = angle;
            }

            return returningEvent;
        }

        public ElementType Element
        {
            get { return this.element; }
        }

        public double Angle
        {
            get { return this.angle; }
        }
    }
}
