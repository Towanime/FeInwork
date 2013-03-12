using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.entities;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;

namespace FeInwork.FeInwork.util
{
    /// <summary>
    /// Hace zoom a la camara mientras se encuentra dentro del rango de un trigger.
    /// </summary>
    public class CameraZoomTrigger : TriggerListener
    {
        /// <summary>
        /// Trigger que activara estos efectos.
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
