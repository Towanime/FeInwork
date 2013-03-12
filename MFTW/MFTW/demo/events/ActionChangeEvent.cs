using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.core.interfaces;
using FeInwork.Core.Managers;

namespace FeInwork.FeInwork.events
{
    public class ActionChangeEvent: AbstractEvent
    {
        private int actionId;
        private bool repeat;

        public int Action
        {
            get { return actionId; }
        }

        public bool doRepeat
        {
            get { return repeat; }
        }

        /// <summary>
        /// Evento que indica el cambio de una acción en un objeto
        /// </summary>
        /// <param name="origin">El objeto origen</param>
        /// <param name="actionId">Indica la acción a realizar</param>
        /// <param name="repeat">Si al terminar repite la acción</param>
        private ActionChangeEvent(object origin, int actionId, bool repeat) :
            base(origin, EventType.ACTION_CHANGE_EVENT)
        {
            this.actionId = actionId;
            this.repeat = repeat;
        }

        /// <summary>
        /// Crea un evento que indica el cambio de una acción en un objeto
        /// </summary>
        /// <param name="origin">El objeto origen</param>
        /// <param name="actionId">Indica la acción a realizar</param>
        /// <param name="repeat">Si al terminar repite la acción</param>
        /// <returns>Evento creado</returns>
        public static ActionChangeEvent Create(object origin, int actionId, bool repeat)
        {
            ActionChangeEvent returningEvent = EventManager.Instance.GetEventFromType<ActionChangeEvent>(EventType.ACTION_CHANGE_EVENT);
            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new ActionChangeEvent(origin, actionId, repeat));
            }
            else
            {
                returningEvent.actionId = actionId;
                returningEvent.repeat = repeat;
                returningEvent.origin = origin;
            }

            return returningEvent;
        }
    }
}
