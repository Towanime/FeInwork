using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.core.collision.bodies;
using FeInwork.Core.Util;
using FeInwork.Core.Interfaces;
using FeInwork.core.Base;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    /// <summary>
    /// Evento para informar que un controller ha sido deshabilitado o habilitado.
    /// </summary>
    public class ControlEnableStateChangeEvent : AbstractEvent
    {
        /// <summary>
        /// Control que cuyo estado cambio
        /// </summary>
        private BaseControlComponent controller;
        /// <summary>
        /// True si el controller ha sido habilitado.
        /// False para indicar que ha sido deshabilitado.
        /// </summary>
        private bool isEnabled;

        private ControlEnableStateChangeEvent(BaseControlComponent controller, bool isEnabled)
            : base(controller.Owner, EventType.CONTROL_ENABLE_STATE_CHANGE_EVENT)
        {
            this.controller = controller;
            this.isEnabled = isEnabled;
        }

        public static ControlEnableStateChangeEvent Create(BaseControlComponent controller, bool isEnabled)
        {
            ControlEnableStateChangeEvent returningEvent = EventManager.Instance.GetEventFromType<ControlEnableStateChangeEvent>(EventType.CONTROL_ENABLE_STATE_CHANGE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new ControlEnableStateChangeEvent(controller, isEnabled));
            }
            else
            {
                returningEvent.controller = controller;
                returningEvent.isEnabled = isEnabled;
                returningEvent.origin = controller.Owner;
            }

            return returningEvent;
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
        }

        public BaseControlComponent Controller
        {
            get
            {
                return this.controller;
            }
        }

    }
}
