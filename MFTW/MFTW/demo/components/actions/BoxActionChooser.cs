using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.components;

namespace FeInwork.FeInwork.actions
{
    public class BoxActionChooser : BasicActionChooser, DeadListener
    {
        public BoxActionChooser(IEntity owner)
            : base(owner)
        {
            this.initialize();
        }

        public override void initialize()
        {
            EventManager.Instance.addListener(EventType.DEAD_EVENT, this.owner, this);
            Program.GAME.ComponentManager.addComponent(this);
        }

        public override int getNewAction()
        {
            if (owner.containsState(EntityState.Dead) && owner.getState(EntityState.Dead))
            {
                EventManager.Instance.fireEvent(DeadActionEvent.Create(owner, true, this));
            }

            return ActionsList.Default;
        }

        public void invoke(DeadEvent eventArgs)
        {
            owner.changeState(EntityState.Dead, true, false);
            //Se cambia la propiedad para que se haga la animación
            owner.changeIntProperty(EntityProperty.Action, ActionsList.Dead, false);
            EventManager.Instance.fireEvent(DeadActionEvent.Create(owner, false, this));
        }

    }
}
