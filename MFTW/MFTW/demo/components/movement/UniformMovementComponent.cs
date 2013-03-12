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
    public class UniformMovementComponent : BaseComponent, IUpdateableFE, PositionChangedListener
    {
        private bool isEnabled;
        private Vector2 position;
        private double angle;
        private Vector2 velocity;
        private int iterationValue = 1;
        private int currentIteration = 0;

        public UniformMovementComponent(IEntity owner)
            : base(owner)
        {
            this.angle = 0;
            this.velocity = new Vector2(100, 100);
            initialize();
        }

        public UniformMovementComponent(IEntity owner, float velocity)
            : base(owner)
        {
            this.angle = 0;
            this.velocity = new Vector2(velocity, velocity);
            initialize();
        }

        public UniformMovementComponent(IEntity owner, float velocity, double angle)
            : base(owner)
        {
            this.angle = angle;
            this.velocity = new Vector2(velocity, velocity);
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
            this.position = owner.getVectorProperty(EntityProperty.Position);
            EventManager.Instance.addListener(EventType.POSITION_CHANGED_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.ENABLE_VANQUISH_MODE_EVENT, this);
        }

        public void Update(GameTime gameTime)
        {
            this.position = owner.getVectorProperty(EntityProperty.Position);

            Vector2 projection = Vector2.Zero;
            Vector2 angleDirection = UtilMethods.angleToDirection(this.angle);
            projection.X = velocity.X * angleDirection.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            projection.Y = velocity.Y * angleDirection.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            EventManager.Instance.fireEvent(PositionChangeRequestEvent.Create(this.owner, this.position, projection / iterationValue));
        }

        public void invoke(PositionChangedEvent eventObject)
        {
            this.position = eventObject.CurrentPosition;
            this.position += eventObject.ProjectedDistance;
            owner.changeVectorProperty(EntityProperty.Position, this.position, true);
        }

        public int IterationValue
        {
            get { return this.iterationValue; }
            set { this.iterationValue = value; }
        }

        public int CurrentIteration
        {
            get { return this.currentIteration; }
            set { this.currentIteration = value; }
        }
    }
}
