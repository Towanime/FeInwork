using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.collision;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.events;
using Microsoft.Xna.Framework;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.util;

namespace FeInwork.collision.responses
{
    /// <summary>
    /// Respuesta que se aplica para entidades que no sean completamente rigidas en movimiento
    /// </summary>
    public class SoftBodyCollisionResponse : AbstractCollisionResponse
    {
        public SoftBodyCollisionResponse(IEntity owner)
            : base(owner)
        {

        }

        public override void invoke(CollisionEvent eventObject)
        {
            // se asegura que ambos cuerpos sean solidos
            if (eventObject.CollisionResult.triggeringBody.Solid && eventObject.CollisionResult.affectedBody.Solid)
            {
                // se calcula una fuerza de impacto basado en que tan profundo fue la colisión
                float force = eventObject.CollisionResult.minimumTranslationVector.Length() * 10000;
                double angle = 0;
                // Luego se verifica si es la entidad causante o afectada para determinar la direccion en la cual se aplicara la fuerza
                Vector2 direction = eventObject.CollisionResult.translationAxis;
                if (eventObject.AffectedEntity == this.owner)
                {
                    Vector2.Multiply(ref direction, -1, out direction);
                }

                angle = UtilMethods.directionToAngle(ref direction);
                EventManager.Instance.fireEvent(ForceAppliedEvent.Create(this.owner, angle, force));
            }
        }
    }
}
