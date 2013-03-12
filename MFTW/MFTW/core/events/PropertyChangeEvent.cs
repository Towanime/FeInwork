using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.Core.Interfaces;

namespace FeInwork.Core.Events
{
    public class PropertyChangeEvent : AbstractEvent
    {
        // propiedad que cambio
        private int property;
        // antiguo valor 
        private object oldValue;
        // el nuevo valor que se le puso
        private object newValue;

        private PropertyChangeEvent(object origin, int property, object oldValue, object newValue) :
            base(origin, EventType.PROPERTY_CHANGE_EVENT)
        {
            this.property = property;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public static PropertyChangeEvent Create(object origin, int property, object oldValue, object newValue)
        {
            PropertyChangeEvent returningEvent = EventManager.Instance.GetEventFromType<PropertyChangeEvent>(EventType.PROPERTY_CHANGE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new PropertyChangeEvent(origin, property, oldValue, newValue));
            }
            else
            {
                returningEvent.property = property;
                returningEvent.oldValue = oldValue;
                returningEvent.newValue = newValue;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public int PropertyName
        {
            get { return this.property; }
        }

        public object OldValue
        {
            get { return this.oldValue; }
        }

        public object NewValue
        {
            get { return this.newValue; }
        }
    }
}
