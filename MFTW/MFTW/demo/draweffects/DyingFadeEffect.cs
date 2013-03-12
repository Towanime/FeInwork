using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.Core.Managers;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.entities;
using FeInwork.Core.Util;

namespace FeInwork.FeInwork.draweffects
{
    public class DyingFadeEffect: FadeEffect, DeadListener, IUpdateableFE
    {
        bool enabled;

        public DyingFadeEffect(DrawableEntity entityToApply)
            : base(entityToApply)
        {
            EventManager.Instance.addListener(EventType.DEAD_EVENT, entityToApply, this);
        }

        public DyingFadeEffect(DrawableEntity entityToApply, float fadeAmount)
            : base(entityToApply, fadeAmount)
        {
            EventManager.Instance.addListener(EventType.DEAD_EVENT, entityToApply, this);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (effectInPlace == false && enabled == true)
            {
                this.enabled = false;
                Program.GAME.ComponentManager.removeUpdateable(this);
                EventManager.Instance.fireEvent(DeadActionEvent.Create(this.entityToApply, true, this));
            }
        }

        public void invoke(DeadEvent eventArgs)
        {
            this.fade(FADE_TYPE.IN);
            this.enabled = true;
            Program.GAME.ComponentManager.addUpdateableOnly(this);
            EventManager.Instance.fireEvent(DeadActionEvent.Create(this.entityToApply, false, this));
        }

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }
    }
}
