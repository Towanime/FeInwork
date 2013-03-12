using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.managers;
using Microsoft.Xna.Framework;
using FeInwork.core.collision.bodies;
using FeInwork.FeInwork.components.interfaces;
using FeInwork.FeInwork.listeners;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.events;
using FeInwork.Core.Util;
using FeInwork.core.collision;

namespace FeInwork.FeInwork.components
{
    /// <summary>
    /// Componente de colisión hecho para entidades estaticas que no tienen ningún movimiento
    /// </summary>
    public class StaticCollisionComponent : AbstractCollisionComponent, PositionChangeRequestListener, DeadListener
    {
        public StaticCollisionComponent(IEntity owner, List<CollisionBody> shapeList)
            : base(owner, shapeList)
        {
            this.initialize();
        }

        public StaticCollisionComponent(IEntity owner, CollisionBody shape)
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
            Vector2 distance = Vector2.Zero;

            for (int shapeIndex = 0; shapeIndex < BodyList.Count; shapeIndex++)
            {
                CollisionBody currentShape = this.BodyList[shapeIndex];
                XboxHashSet<CollisionBody> shapeGroup = CollisionManager.Instance.GetBodyGroup(currentShape);

                //for (int secondShapeIndex = 0; secondShapeIndex < shapeGroup.Count; secondShapeIndex++)
                //{
                foreach (CollisionBody secondShape in shapeGroup)
                {
                    //CollisionBody secondShape = shapeGroup[secondShapeIndex];

                    if (secondShape == currentShape) continue;
                    if (this.BodyList.Contains(secondShape)) continue;

                    CollisionResult result = currentShape.Intersect(secondShape, ref distance);

                    if (!result.willIntersect) continue;
                    EventManager.Instance.fireEvent(CollisionEvent.Create(this, currentShape.Owner, secondShape.Owner, result));
                }
            }
        }

        public void invoke(DeadEvent eventArgs)
        {
            this.removeBodies(this.BodyList);
        }
    }
}
