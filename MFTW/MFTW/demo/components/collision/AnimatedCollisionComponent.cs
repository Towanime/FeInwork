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
    /// <summary>
    /// Componente de colisión hecho para cosas que pueden moverse y voltearse
    /// </summary>
    public class AnimatedCollisionComponent : AbstractCollisionComponent, PositionChangeRequestListener, StateChangeListener, DeadListener
    {
        public AnimatedCollisionComponent(IEntity owner, List<CollisionBody> shapeList)
            : base(owner, shapeList)
        {
            this.initialize();
        }

        public AnimatedCollisionComponent(IEntity owner, CollisionBody shape)
            : base(owner, shape)
        {
            this.initialize();
        }

        public override void initialize()
        {
            for (int shapeIndex = 0; shapeIndex < BodyList.Count; shapeIndex++)
            {
                CollisionManager.Instance.addContainer(BodyList[shapeIndex]);
            }
            EventManager.Instance.addListener(EventType.POSITION_CHANGE_REQUEST_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.DEAD_EVENT, this.owner, this);
            EventManager.Instance.addStateListener(EntityState.FacingRight, this.owner, this);
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
                
            for (int shapeIndex = 0; shapeIndex < bodyList.Count; shapeIndex++)
            {
                CollisionBody currentShape = this.bodyList[shapeIndex];
                XboxHashSet<CollisionBody> shapeGroup = CollisionManager.Instance.GetBodyGroup(currentShape);

                //for (int secondShapeIndex = 0; secondShapeIndex < shapeGroup.Count; secondShapeIndex++)
                //{
                foreach (CollisionBody secondShape in shapeGroup)
                {
                    //CollisionBody secondShape = shapeGroup[secondShapeIndex];

                    if (secondShape == currentShape || this.bodyList.Contains(secondShape))
                    {
                        continue;
                    }

                    CollisionResult result = currentShape.Intersect(secondShape, ref movementDistance);

                    if (result.willIntersect)
                    {
                        EventManager.Instance.fireEvent(CollisionEvent.Create(this, currentShape.Owner, secondShape.Owner, result));
                        if (currentShape.Solid && secondShape.Solid)
                        {
                            Vector2.Add(ref movementDistance, ref result.minimumTranslationVector, out movementDistance);
                        }
                    }
                }

                if (movementDistance.X != 0.0f || movementDistance.Y != 0.0f)
                {
                    for (int i = 0; i < bodyList.Count; i++)
                    {
                        CollisionBody shape = this.bodyList[i];
                        CollisionManager.Instance.removeContainer(shape);
                        shape.Offset(movementDistance.X, movementDistance.Y);
                        CollisionManager.Instance.addContainer(shape);
                    }
                }

                Vector2.Add(ref totalMovementDistance, ref movementDistance, out totalMovementDistance);
                movementDistance = Vector2.Zero;
            }

            Vector2 currentPosition = eventObject.CurrentPosition;
            EventManager.Instance.fireEvent(PositionChangedEvent.Create(this.owner, ref currentPosition, ref totalMovementDistance));
        }

        public void invoke(StateChangeEvent eventObject)
        {
            if (eventObject.State == EntityState.FacingRight && !eventObject.NewValue.Equals(eventObject.OldValue))
            {
                for (int shapeIndex = 0; shapeIndex < bodyList.Count; shapeIndex++)
                {
                    CollisionBody body = bodyList[shapeIndex];
                    CollisionManager.Instance.removeContainer(body);
                    body.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                    CollisionManager.Instance.addContainer(body);
                }
            }
        }

        public void invoke(DeadEvent eventArgs)
        {
            this.removeBodies(this.BodyList);
        }
    }
}
