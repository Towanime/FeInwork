using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.entities;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;
using FeInwork.FeInwork.draweffects;

namespace FeInwork.FeInwork.util
{
    /// <summary>
    /// Trigger que aplica un efecto de fade ("desaparecer")
    /// sobre una entidad que tenga FadeEffect
    /// </summary>
    public class FadeInTrigger : TriggerListener
    {
        /// <summary>
        /// Trigger que activara estos efectos.
        /// </summary>
        private BaseTrigger trigger;
        /// <summary>
        /// Entidad a la cual se le hará el FadeIn una
        /// vez se entre a este trigger'. En caso de que
        /// esta entidad sea null se le aplicará el fade
        /// a la entidad que haya provocado el trigger
        /// </summary>
        private DrawableEntity entityToFade;

        public FadeInTrigger(BaseTrigger trigger)
        {
            this.trigger = trigger;
            initialize();
        }

        public FadeInTrigger(BaseTrigger trigger, DrawableEntity entityToFade)
        {
            this.trigger = trigger;
            this.entityToFade = entityToFade;
            initialize();
        }

        private void initialize()
        {
            EventManager.Instance.addListener(EventType.TRIGGER_IN_RANGE_EVENT, trigger, this);
            EventManager.Instance.addListener(EventType.TRIGGER_OUT_RANGE_EVENT, trigger, this);
        }

        public void invoke(TriggerRangeEvent eventArgs)
        {
            DrawableEntity entity;
            if ((entity = entityToFade) != null || (entity = eventArgs.TriggerEntity as DrawableEntity) != null)
            {
                if (eventArgs.IsInRange)
                {
                    FadeEffect effect = entity.findDrawEffect<FadeEffect>();
                    if(effect != null)
                    {
                        effect.fade(FadeEffect.FADE_TYPE.IN);
                    }
                }
                else
                {
                    FadeEffect effect = entity.findDrawEffect<FadeEffect>();
                    if (effect != null)
                    {
                        effect.fade(FadeEffect.FADE_TYPE.OUT);
                    }
                }
            }

        }
    }
}
