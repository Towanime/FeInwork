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
    public class ProgressiveMovementComponent : BaseComponent, IUpdateableFE, PositionChangedListener
    {
        private bool isEnabled;
        private Vector2 position;
        private double angle;
        private Vector2 stepImpulse;
        private Vector2 maxVelocity;
        private Vector2 currentVelocity;
        private int iterationValue = 1;
        private int currentIteration = 0;
        private Vector2 projectionDistance;

        public ProgressiveMovementComponent(IEntity owner)
            : base(owner)
        {
            this.angle = 0;
            this.stepImpulse = new Vector2(20, 20);
            this.maxVelocity = new Vector2(100, 100);
            initialize();
        }

        public ProgressiveMovementComponent(IEntity owner, float stepImpulse, float maxVelocity)
            : base(owner)
        {
            this.angle = 0;
            this.stepImpulse = new Vector2(stepImpulse, stepImpulse);
            this.maxVelocity = new Vector2(maxVelocity, maxVelocity);
            initialize();
        }

        public ProgressiveMovementComponent(IEntity owner, float stepImpulse, float maxVelocity, double angle)
            : base(owner)
        {
            this.angle = angle;
            this.stepImpulse = new Vector2(stepImpulse, stepImpulse);
            this.maxVelocity = new Vector2(maxVelocity, maxVelocity);
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

            if (currentIteration == 0)
            {
                currentIteration = iterationValue;
                Vector2 angleDirection = UtilMethods.angleToDirection(this.angle);
                this.Velocity = this.Velocity + (this.stepImpulse * (float)gameTime.ElapsedGameTime.TotalSeconds);
                this.projectionDistance.X = this.Velocity.X * angleDirection.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                this.projectionDistance.Y = this.Velocity.Y * angleDirection.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;    
            }

            currentIteration--;
            EventManager.Instance.fireEvent(PositionChangeRequestEvent.Create(this.owner, this.position, this.projectionDistance / iterationValue));
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

        public Vector2 Velocity
        {
            get { return currentVelocity; }
            set 
            {
                currentVelocity.X = MathHelper.Clamp(value.X, 0, maxVelocity.X);
                currentVelocity.Y = MathHelper.Clamp(value.Y, 0, maxVelocity.Y);
            }
        }
    }
}
