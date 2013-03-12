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
using FeInwork.FeInwork.world;

namespace FeInwork.FeInwork.components
{
    /// <summary>
    /// Componente de colisión hecho para cosas que pueden moverse y voltearse
    /// </summary>
    public class EarthCollisionComponent : AbstractCollisionComponent, PositionChangeRequestListener, DeadListener
    {
        public EarthCollisionComponent(IEntity owner, List<CollisionBody> shapeList)
            : base(owner, shapeList)
        {
            this.initialize();
        }

        public EarthCollisionComponent(IEntity owner, CollisionBody shape)
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
            for (int i = shapeList.Count - 1; i >= 0; i--)
            {
                CollisionBody shape = shapeList[i];
                if (this.BodyList.Remove(shape))
                {
                    CollisionManager.Instance.removeContainer(shape);
                }
            }
        }

        public override void move(Vector2 oldPos, Vector2 newPos)
        {
            for (int i = 0; i < this.BodyList.Count; i++)
            {
                this.BodyList[i].OffsetRelativeTo(oldPos, newPos);
            }
        }

        public override void initialize()
        {
            for (int shapeIndex = 0; shapeIndex < BodyList.Count; shapeIndex++)
            {
                CollisionManager.Instance.addContainer(BodyList[shapeIndex]);
            }
            EventManager.Instance.addListener(EventType.POSITION_CHANGE_REQUEST_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.DEAD_EVENT, this.owner, this);
            Program.GAME.ComponentManager.addComponent(this);
        }

        public void invoke(PositionChangeRequestEvent eventObject)
        {
            Vector2 movementDistance = eventObject.ProjectedDistance;
            Vector2 totalMovementDistance = Vector2.Zero;

            if (BodyList.Count == 0)
            {
                totalMovementDistance = eventObject.ProjectedDistance;
            }

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
                        if (currentShape.Solid && secondShape.Solid && secondShape.Owner.Equals(WorldManager.Instance.ActualArea))
                        {
                            movementDistance += result.minimumTranslationVector;
                        }
                    }
                }

                if (movementDistance.X != 0.0f || movementDistance.Y != 0.0f)
                {
                    for (int i = 0; i < BodyList.Count; i++)
                    {
                        CollisionBody shape = this.BodyList[i];
                        CollisionManager.Instance.removeContainer(shape);
                        shape.Offset(movementDistance.X, movementDistance.Y);
                        CollisionManager.Instance.addContainer(shape);
                    }
                }

                totalMovementDistance += movementDistance;
                movementDistance = Vector2.Zero;
            }

            Vector2 currentPosition = eventObject.CurrentPosition;
            EventManager.Instance.fireEvent(PositionChangedEvent.Create(this.owner, ref currentPosition, ref totalMovementDistance));
        }

        public void invoke(DeadEvent eventArgs)
        {
            this.removeBodies(this.BodyList);
        }
    }
}
