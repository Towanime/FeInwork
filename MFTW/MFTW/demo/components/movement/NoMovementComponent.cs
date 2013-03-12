using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using Microsoft.Xna.Framework;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.util;

namespace FeInwork.FeInwork.components
{
    public class NoMovementComponent : BaseComponent, IUpdateableFE
    {
        private bool isEnabled;

        public NoMovementComponent(IEntity owner)
            : base(owner)
        {
            initialize();
        }

        public bool Enabled
        {
            get { return isEnabled; }
            set { this.isEnabled = value; }
        }

        public override void initialize()
        {
            isEnabled = true;
            Program.GAME.ComponentManager.addComponent(this);
        }

        public void Update(GameTime gameTime)
        {
            Vector2 position = owner.getVectorProperty(EntityProperty.Position);
            EventManager.Instance.fireEvent(PositionChangeRequestEvent.Create(this.owner, position, Vector2.Zero));
        }
    }
}
