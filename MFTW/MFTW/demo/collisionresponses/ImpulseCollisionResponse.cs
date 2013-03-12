using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.events;
using FeInwork.core.collision;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.util;

namespace FeInwork.collision.responses
{
    /// <summary>
    /// Respuesta que debe aplicarse al momento de colisionar 
    /// una bola de fuego con otra entidad
    /// </summary>
    public class ImpulseCollisionResponse : AbstractCollisionResponse
    {
        private double angle;
        private float magnitude;

        public ImpulseCollisionResponse(IEntity owner)
            : base(owner)
        {
            this.angle = GameAngles.RIGHT_ANGLE;
            this.magnitude = 10000;
        }

        public ImpulseCollisionResponse(IEntity owner, float magnitude)
            : base(owner)
        {
            this.angle = GameAngles.RIGHT_ANGLE;
            this.magnitude = magnitude;
        }

        public ImpulseCollisionResponse(IEntity owner, float magnitude, double angle)
            : base(owner)
        {
            this.angle = angle;
            this.magnitude = magnitude;
        }

        public override void invoke(CollisionEvent eventObject)
        {
            if (eventObject.TriggeringEntity == this.owner 
                && eventObject.CollisionResult.triggeringBody.Solid 
                && eventObject.CollisionResult.affectedBody.Solid)
            {
                // Le aplica una fuerza de impacto
                EventManager.Instance.fireEvent(ForceAppliedEvent.Create(eventObject.AffectedEntity, this.angle, this.magnitude));
            }
        }
    }
}
