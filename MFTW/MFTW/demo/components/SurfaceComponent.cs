using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.listeners;
using Microsoft.Xna.Framework;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;
using FeInwork.FeInwork.events;

namespace FeInwork.FeInwork.components
{
    public class SurfaceComponent : BaseComponent, IUpdateableFE, PositionChangedListener, SurfaceListener
    {
        private bool isEnabled;
        private List<IEntity> entitiesOnSurface;
        private List<IEntity> tempEntitiesOnSurface;
        private Vector2 distance;

        public SurfaceComponent(IEntity owner)
            : base(owner)
        {
            initialize();
        }

        public override void initialize()
        {
            this.isEnabled = true;
            this.entitiesOnSurface = new List<IEntity>();
            this.tempEntitiesOnSurface = new List<IEntity>();
            Program.GAME.ComponentManager.addComponent(this);
            EventManager.Instance.addListener(EventType.POSITION_CHANGED_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.ENTITY_ON_SURFACE_EVENT, this.owner, this);
        }

        public void Update(GameTime gameTime)
        {
            if (Math.Abs(distance.X) > 0 || Math.Abs(distance.Y) > 0)
            {
                for (int i = 0; i < this.entitiesOnSurface.Count; i++)
                {
                    this.tempEntitiesOnSurface.Add(this.entitiesOnSurface[i]);
                }
                entitiesOnSurface.Clear();

                for (int i = 0; i < this.tempEntitiesOnSurface.Count; i++)
                {
                    IEntity entity = this.tempEntitiesOnSurface[i];
                    Vector2 entityPosition = entity.getVectorProperty(EntityProperty.Position);
                    EventManager.Instance.fireEvent(PositionChangeRequestEvent.Create(entity, entityPosition, distance));
                }
                tempEntitiesOnSurface.Clear();

                distance = Vector2.Zero;
            }
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
            }
        }

        public void invoke(PositionChangedEvent eventObject)
        {
            this.distance += eventObject.ProjectedDistance;
        }

        public void invoke(EntityOnSurfaceEvent eventObject)
        {
            this.addEntityOnSurface(eventObject.Entity);
        }

        public void addEntityOnSurface(IEntity entity)
        {
            if (!this.entitiesOnSurface.Contains(entity))
            {
                this.entitiesOnSurface.Add(entity);
            }
        }
    }
}
