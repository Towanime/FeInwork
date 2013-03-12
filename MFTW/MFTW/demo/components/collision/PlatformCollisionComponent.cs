using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.core.util;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using FeInwork.Core.Util;
using FeInwork.core.managers;
using FeInwork.FeInwork.events;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.listeners;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.core.collision.bodies;
using FeInwork.core.collision;
using FeInwork.Core.Listeners;
using FeInwork.Core.Events;
using FeInwork.FeInwork.components.interfaces;
using FeInwork.FeInwork.util;
using FeInwork.core.events;

namespace FeInwork.FeInwork.components
{
    public class PlatformCollisionComponent : AbstractCollisionComponent, PositionChangeRequestListener
    {
        public PlatformCollisionComponent(IEntity owner, List<CollisionBody> shapeList)
            : base(owner, shapeList)
        {
            this.initialize();
        }

        public PlatformCollisionComponent(IEntity owner, CollisionBody shape)
            : base(owner, shape)
        {
            this.initialize();
        }

        public override void addBody(CollisionBody shape)
        {
            if (!this.BodyList.Contains(shape))
            {
                this.BodyList.Add(shape);
                CollisionManager.Instance.addContainer(shape);
            }
        }

        public override void addBodies(List<CollisionBody> shapeList)
        {
            for (int i = 0; i < shapeList.Count; i++)
            {
                CollisionBody shape = shapeList[i];
                if (!this.BodyList.Contains(shape))
                {
                    this.BodyList.Add(shape);
                    CollisionManager.Instance.addContainer(shape);
                }
            }
        }

        public override void removeBody(CollisionBody shape)
        {
            if (this.BodyList.Remove(shape))
            {
                CollisionManager.Instance.removeContainer(shape);
            }
        }

        public override void removeBodies(List<CollisionBody> shapeList)
        {
            for (int i = 0; i < shapeList.Count; i++)
            {
                CollisionBody shape = shapeList[i];
                if (this.BodyList.Remove(shape))
                {
                    CollisionManager.Instance.removeContainer(shape);
                }
            }
        }

        public override void initialize()
        {
            for (int shapeIndex = 0; shapeIndex < BodyList.Count; shapeIndex++)
            {
                CollisionManager.Instance.addContainer(BodyList[shapeIndex]);
            }
            EventManager.Instance.addListener(EventType.POSITION_CHANGE_REQUEST_EVENT, this.owner, this);
            Program.GAME.ComponentManager.addComponent(this);
        }

        public void invoke(PositionChangeRequestEvent eventObject)
        {
            Vector2 movementDistance = eventObject.ProjectedDistance;

            for (int shapeIndex = 0; shapeIndex < BodyList.Count; shapeIndex++)
            {
                CollisionBody currentShape = this.BodyList[shapeIndex];
                XboxHashSet<CollisionBody> shapeGroup = CollisionManager.Instance.GetBodyGroup(currentShape);

                //for (int secondShapeIndex = 0; secondShapeIndex < shapeGroup.Count; secondShapeIndex++)
                //{
                foreach (CollisionBody secondShape in shapeGroup)
                {
                    //CollisionBody secondShape = shapeGroup[secondShapeIndex];

                    if (secondShape == currentShape || this.BodyList.Contains(secondShape))
                    {
                        continue;
                    }

                    CollisionResult result = currentShape.Intersect(secondShape, ref movementDistance);

                    if (result.willIntersect)
                    {
                        EventManager.Instance.fireEvent(CollisionEvent.Create(this, currentShape.Owner, secondShape.Owner, result));
                    }
                }

                if (movementDistance.X != 0.0f || movementDistance.Y != 0.0f)
                {
                    for (int i = 0; i < BodyList.Count; i++)
                    {
                        CollisionBody shape = this.BodyList[i];
                        shape.Offset(movementDistance.X, movementDistance.Y);
                        CollisionManager.Instance.removeContainer(shape);
                        CollisionManager.Instance.addContainer(shape);
                    }
                }
            }

            Vector2 currentPosition = eventObject.CurrentPosition;
            EventManager.Instance.fireEvent(PositionChangedEvent.Create(this.owner, ref currentPosition, ref movementDistance));
        }
    }
}
