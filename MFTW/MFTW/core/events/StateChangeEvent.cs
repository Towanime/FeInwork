using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.util;
using FeInwork.Core.Util;
using FeInwork.Core.Base;
using FeInwork.Core.Managers;
using FeInwork.Core.Interfaces;

namespace FeInwork.core.events
{
    public class StateChangeEvent : AbstractEvent
    {
        // estado que cambio
        private int state;
        // quizas esto no tenga sentido coz si es true el neuvo valor se sobreentiende que el anterior valor es false :D
        // antiguo valor 
        private bool oldValue;
        // el nuevo valor que se le puso
        private bool newValue;

        private StateChangeEvent(object origin, int state, bool oldValue, bool newValue) :
            base(origin, EventType.STATE_CHANGE_EVENT)
        {
            this.state = state;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public static StateChangeEvent Create(object origin, int state, bool oldValue, bool newValue)
        {
            StateChangeEvent returningEvent = EventManager.Instance.GetEventFromType<StateChangeEvent>(EventType.STATE_CHANGE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new StateChangeEvent(origin, state, oldValue, newValue));
            }
            else
            {
                returningEvent.state = state;
                returningEvent.oldValue = oldValue;
                returningEvent.newValue = newValue;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }

        public int State
        {
            get { return this.state; }
        }

        public bool OldValue
        {
            get { return this.oldValue; }
        }

        public bool NewValue
        {
            get { return this.newValue; }
        }
    }
}
