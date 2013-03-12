using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlchemistDemo.alchemist.listeners;
using AlchemistDemo.alchemist.events;
using AlchemistDemo.alchemist.entities;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;

namespace AlchemistDemo.alchemist.util
{
    /// <summary>
    /// Hace zoom a la camara mientras se encuentra dentro del rango de un trigger.
    /// </summary>
    public class CameraZoomTrigger : TriggerListener
    {
        /// <summary>
        /// Triger que activara estos efectos.
        /// </summary>
        private BaseTrigger trigger;

        public CameraZoomTrigger(BaseTrigger trigger)
        {
            this.trigger = trigger;
            initialize();
        }

        private void initialize()
        {
            EventManager.Instance.addListener(EventType.TRIGGER_IN_RANGE_EVENT, trigger, this);
            EventManager.Instance.addListener(EventType.TRIGGER_OUT_RANGE_EVENT, trigger, this);
        }

        public void invoke(TriggerRangeEvent eventArgs)
        {
            if (eventArgs.IsInRange)
            {
                Program.GAME.Camera.zoomIn();
            }
            else
            {
                Program.GAME.Camera.zoomOut();
            }
        }
    }
}
