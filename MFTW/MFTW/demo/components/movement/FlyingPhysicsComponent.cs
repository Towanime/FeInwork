using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;
using FeInwork.Core.Listeners;
using FeInwork.Core.Events;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.listeners;
using FeInwork.core.physics;
using FeInwork.FeInwork.util;
using FeInwork.core.events;

namespace FeInwork.FeInwork.components
{
    public class FlyingPhysicsComponent : BaseComponent, 
        IUpdateableFE, MoveExecutedListener,
        PositionChangedListener,
        ForceAppliedListener,
        EnablePhysicsListener,
        DeadListener
    {
        private bool isEnabled;
        private bool isPhysicsEnabled;
        private Vector2 position = Vector2.Zero;
        private Vector2 projectionDistance = Vector2.Zero;
        private Vector2 direction = new Vector2(1, -1);
        private Vector2 velocity = Vector2.Zero;
        private Vector2 acceleration = Vector2.Zero;
        private PhysicsData data;
        private Vector2 internalForces = Vector2.Zero;
        private Vector2 externalForces = Vector2.Zero;
        private bool hasFlew = false;
        private bool hasMoved = false;

        private int iterationValue = 1;

        private int currentIteration = 0;


        public FlyingPhysicsComponent(IEntity owner, PhysicsData data)
            : base(owner)
        {
            this.position = owner.getVectorProperty(EntityProperty.Position);
            this.data = data;
            initialize();
        }

        public override void initialize()
        {
            isEnabled = true;
            isPhysicsEnabled = true;
            Program.GAME.ComponentManager.addComponent(this);
            owner.addState(EntityState.OnAir, true);
            owner.addIntProperty(EntityProperty.HorizontalDirection, 1);
            EventManager.Instance.addListener(EventType.MOVE_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.POSITION_CHANGED_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.FORCE_APPLIED_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.ENABLE_PHYSICS_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.ENABLE_VANQUISH_MODE_EVENT, this);
            EventManager.Instance.addListener(EventType.DEAD_EVENT, this.owner, this);
        }

        public bool Enabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                this.velocity.X = 0f;
            }
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

        public bool IsPhysicsEnabled
        {
            get { return this.isPhysicsEnabled; }
            set { this.isPhysicsEnabled = value; }
        }

        private Vector2 Velocity
        {
            get
            {
                return this.velocity;
            }
            set
            {
                this.velocity.X = MathHelper.Clamp(value.X, this.data.MinimumVelocity.X, this.data.MaximumVelocity.X);
                this.velocity.Y = MathHelper.Clamp(value.Y, this.data.MinimumVelocity.Y, this.data.MaximumVelocity.Y);
            }
        }

        public void Update(GameTime gameTime)
        {

            if (isPhysicsEnabled)
            {
                if (currentIteration == 0)
                {
                    currentIteration = iterationValue;

                    if (hasFlew)
                    {
                        int verticalDirection = owner.getIntProperty(EntityProperty.VerticalDirection);
                        float flyImpulse = this.data.AirImpulse;
                        this.internalForces.Y += flyImpulse * verticalDirection;
                        this.hasFlew = false;
                    }

                    if (hasMoved)
                    {
                        int horizontalDirection = owner.getIntProperty(EntityProperty.HorizontalDirection);
                        float moveImpulse = this.data.RunningImpulse;
                        this.internalForces.X += moveImpulse * horizontalDirection;
                        hasMoved = false;
                    }

                    this.position = owner.getVectorProperty(EntityProperty.Position);

                    acceleration = Vector2.Zero;
                                    
                    acceleration = (this.internalForces + this.externalForces);

                    this.Velocity = this.acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    this.projectionDistance = this.velocity;

                    this.internalForces = Vector2.Zero;
                    this.externalForces = Vector2.Zero;

                }

                currentIteration--;


                EventManager.Instance.fireEvent(PositionChangeRequestEvent.Create(this.owner, this.position, this.projectionDistance / iterationValue));
            }
            else
            {
                this.resetValues();
            }
        }

        private void resetValues()
        {
            this.internalForces = Vector2.Zero;
            this.externalForces = Vector2.Zero;
        }

        public void invoke(MoveEvent eventObject)
        {
            if (isEnabled && isPhysicsEnabled)
            {
                if (eventObject.MoveType == MoveEvent.MOVE_TYPE.WALK)
                {
                    this.hasMoved = true;
                }

                if (eventObject.MoveType == MoveEvent.MOVE_TYPE.FLY)
                {
                    this.hasFlew = true;
                }
            }
        }

        public void invoke(PositionChangedEvent eventObject)
        {
            if (isEnabled)
            {
                this.position = eventObject.CurrentPosition;
                this.position += eventObject.ProjectedDistance;
                owner.changeVectorProperty(EntityProperty.Position, this.position, true);
            }
        }

        public void invoke(ForceAppliedEvent eventObject)
        {
            if (isEnabled && isPhysicsEnabled)
            {
                Vector2 angleDirection = UtilMethods.angleToDirection(eventObject.Angle);
                this.externalForces += new Vector2(angleDirection.X * eventObject.Magnitude,
                    angleDirection.Y * eventObject.Magnitude);
            }          
        }

        public void invoke(EnablePhysicsEvent eventArgs)
        {
            this.isPhysicsEnabled = eventArgs.EnablePhysics;
        }

        public void invoke(DeadEvent eventArgs)
        {
            this.isPhysicsEnabled = false;
        }
    }
}
