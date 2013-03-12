using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.entities;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;
using FeInwork.core.util;
using FeInwork.core.interfaces;
using FeInwork.Core.Interfaces;

namespace FeInwork.FeInwork.triggers
{
    public class YaeDialogTrigger : TriggerListener
    {
        /// <summary>
        /// Trigger que activara estos efectos.
        /// </summary>
        private BaseTrigger trigger;
        private bool activated;
        private IEntity yae;

        public YaeDialogTrigger(IEntity yae, BaseTrigger trigger)
        {
            this.yae = yae;
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
            // empieza el dialogo!
            if (!activated)
            {
                activated = true;
                //Program.GAME.Camera.Focus = (IFocusable)yae;
                //Program.GAME.Camera.resetCameraPosition();
                // begin dialog! a los coñazos again
                string text = "Yae: $n "
                    + "Sabia que vendrias... Goemon... $0 "
                    + "Goemon:  $n "
                    + "Yae... Se que las cosas no terminaron bien entre nosotros pero... "
                    + " $1 "
                    + "Silencio!!! "
                    + " $2 Goemon: $n "
                    + "Como podemos arreglar esto? $e "
                    + "Yae: $n "
                    + "Solo se podra resolver con la muerte... $e "
                    + "Yae: $n "
                    + "Es hora de la batalla!!!!";

                // parametros!!!
                DialogParameters[] pars = new DialogParameters[5];
                pars[0] = new DialogParameters(false);
                pars[0].BlockSeconds = 4;
                pars[0].IsAutoNextAfterBlock = true;

                pars[1] = new DialogParameters(false);
                pars[1].Scale = 4;

                pars[2] = new DialogParameters(false);
                DialogManager.Instance.beginDialog(text, pars);
            }
        }
    }
}
