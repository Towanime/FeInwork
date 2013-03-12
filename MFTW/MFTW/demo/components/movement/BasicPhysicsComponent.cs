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
    public class BasicPhysicsComponent : BaseComponent, 
        IUpdateableFE, MoveExecutedListener,
        PositionChangedListener,
        ForceAppliedListener,
        StateChangeListener,
        EnablePhysicsListener,
        DeadListener,
        SurfaceContactListener
    {
        private bool isEnabled;
        private bool isPhysicsEnabled;
        private Vector2 position = Vector2.Zero;
        private Vector2 projectionDistance = Vector2.Zero;
        private Vector2 direction = new Vector2(1, -1);
        private Vector2 velocity = Vector2.Zero;
        private Vector2 acceleration = Vector2.Zero;
        private float gravity = 2000f;
        private float groundFriction = 20000f;
        private float airFriction = 5000f;
        private PhysicsData data;
        private Vector2 internalForces = Vector2.Zero;
        private Vector2 externalForces = Vector2.Zero;
        private bool onAir = false;
        private bool hasJumped = false;
        private bool hasMoved = false;

        private int iterationValue = 1;
        private int currentIteration = 0;

        public BasicPhysicsComponent(IEntity owner, PhysicsData data)
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
            int horizontalDirection = (!owner.containsState(EntityState.FacingRight)) ? 1 
                : (owner.getState(EntityState.FacingRight) ? 1 : -1);            
            owner.addState(EntityState.OnAir, true);
            owner.addState(EntityState.Running, false);
            owner.addIntProperty(EntityProperty.HorizontalDirection, horizontalDirection);
            direction = new Vector2(horizontalDirection, -1);
            EventManager.Instance.addListener(EventType.MOVE_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.POSITION_CHANGED_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.FORCE_APPLIED_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.ENABLE_PHYSICS_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.ENABLE_VANQUISH_MODE_EVENT, this);
            EventManager.Instance.addListener(EventType.TRANCE_BEGIN_EVENT, this);
            EventManager.Instance.addListener(EventType.TRANCE_END_EVENT, this);
            EventManager.Instance.addListener(EventType.DEAD_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.SURFACE_CONTACT_EVENT, this.owner, this);
            EventManager.Instance.addStateListener(EntityState.OnAir, this.owner, this);
            Program.GAME.ComponentManager.addComponent(this);
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
            if (onAir)
            {
                owner.changeState(EntityState.OnAir, onAir, true);
            }

            if (isPhysicsEnabled)
            {                
                int horizontalDirection = owner.getIntProperty(EntityProperty.HorizontalDirection);

                if (currentIteration == 0)
                {
                    currentIteration = iterationValue;

                    if (hasJumped)
                    {
                        this.internalForces.Y += this.data.JumpImpulse * direction.Y;
                        this.onAir = true;
                        this.velocity.Y = 0;
                        this.hasJumped = false;
                    }

                    if (hasMoved)
                    {
                        float moveImpulse = this.data.RunningImpulse;
                        if (onAir == true) { moveImpulse = this.data.AirImpulse; }

                        if (Math.Sign(velocity.X) != Math.Sign(direction.X))
                        {
                            this.internalForces.X += moveImpulse * direction.X;
                        }
                        else
                        {
                            if (horizontalDirection != direction.X)
                            {
                                this.internalForces.X += moveImpulse * -direction.X;
                            }
                            else
                            {
                                this.internalForces.X += moveImpulse * direction.X;
                            }
                        }
                        hasMoved = false;
                    }

                    this.position = owner.getVectorProperty(EntityProperty.Position);

                    acceleration = Vector2.Zero;
                    Vector2.Add(ref this.internalForces, ref this.externalForces, out acceleration);

                    if (this.data.Weight != 0)
                    {
                        Vector2.Divide(ref acceleration, this.data.Weight, out acceleration);
                        acceleration.Y += gravity;
                    }

                    this.Velocity = this.velocity + (this.acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    this.projectionDistance = this.velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    this.internalForces = Vector2.Zero;
                    this.externalForces = Vector2.Zero;

                    if (Math.Sign(velocity.X) != Math.Sign(direction.X))
                    {
                        this.velocity.X = 0;
                        this.direction.X *= -1;
                    }
                    else
                    {
                        this.internalForces.X += ((onAir == false) ? groundFriction : airFriction) * -direction.X;
                    }
                    if (onAir == false)
                    {
                        this.velocity.Y = 0;
                    }                                        
                }

                currentIteration--;
                onAir = true;

                Vector2.Divide(ref this.projectionDistance, iterationValue, out this.projectionDistance);
                EventManager.Instance.fireEvent(PositionChangeRequestEvent.Create(this.owner, this.position, this.projectionDistance));
            }
            else
            {
                this.resetValues();
                //EventManager.Instance.fireEvent(PositionEvent.Create(this.owner, this.position, new Vector2(0, 0), false));
            }
        }

        private void resetValues()
        {
            this.internalForces = Vector2.Zero;
            this.externalForces = Vector2.Zero;
            onAir = true;
        }

        public void invoke(MoveEvent eventObject)
        {
            if (isEnabled && isPhysicsEnabled)
            {
                if (eventObject.MoveType == MoveEvent.MOVE_TYPE.WALK)
                {
                    this.hasMoved = true;
                }

                if (eventObject.MoveType == MoveEvent.MOVE_TYPE.JUMP && owner.getState(EntityState.OnAir) == false)
                {
                    this.hasJumped = true;
                }
            }
        }

        public void invoke(PositionChangedEvent eventObject)
        {
            if (isEnabled)
            {
                this.position = eventObject.CurrentPosition;
                Vector2 projectedDistance = eventObject.ProjectedDistance;
                Vector2.Add(ref this.position, ref projectedDistance, out this.position);
                owner.changeVectorProperty(EntityProperty.Position, this.position, true);
            }
        }

        public void invoke(ForceAppliedEvent eventObject)
        {
            if (isEnabled && isPhysicsEnabled)
            {
                Vector2 angleDirection = UtilMethods.angleToDirection(eventObject.Angle);
                angleDirection.X = (float)Math.Round(angleDirection.X, 6);
                angleDirection.Y = (float)Math.Round(angleDirection.Y, 6);
                this.externalForces.X = this.externalForces.X + (angleDirection.X * eventObject.Magnitude);
                this.externalForces.Y = this.externalForces.Y + (angleDirection.Y * eventObject.Magnitude);
            }          
        }

        public void invoke(StateChangeEvent eventObject)
        {
            if (isEnabled)
            {
                if (eventObject.State == EntityState.OnAir)
                {
                    this.onAir = eventObject.NewValue;
                }
            }
        }

        public void invoke(SurfaceContactEvent eventObject)
        {
            if (isEnabled)
            {
                this.onAir = false;
                this.owner.changeState(EntityState.OnAir, false, true);
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
