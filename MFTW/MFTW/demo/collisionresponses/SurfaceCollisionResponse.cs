using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.listeners;
using Microsoft.Xna.Framework;
using FeInwork.core.collision;
using FeInwork.Core.Util;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.components;

namespace FeInwork.collision.responses
{
    /// <summary>
    /// Respuesta que debe darse cuando un cuerpo colisione con una superficie
    /// Ej: Indicar que la entidad ya no se encuentra en el aire
    /// </summary>
    public class SurfaceCollisionResponse : AbstractCollisionResponse
    {
        public SurfaceCollisionResponse(IEntity owner)
            : base(owner)
        {

        }

        public override void invoke(CollisionEvent eventObject)
        {
            if (eventObject.AffectedEntity == this.owner && eventObject.CollisionResult.triggeringBody.Solid && eventObject.CollisionResult.affectedBody.Solid)
            {
                // Se calcula el angulo entre un vector recto horizontalmente y la perpendicular del eje de transicion
                // para saber si el angulo es mayor o menor a 45 grados, en caso de que sea menor determinamos que la entidad que ha colisionado
                // con esta superficie se encuentra en tierra
                Vector2 perpendicular = new Vector2();
                perpendicular.X = -eventObject.CollisionResult.translationAxis.Y; 
                perpendicular.Y = eventObject.CollisionResult.translationAxis.X;

                double angle = (Math.Atan2(0, 1) - Math.Atan2(perpendicular.Y, perpendicular.X)) * (180 / Math.PI);
                if ((Math.Abs(angle) >= 125 || Math.Abs(angle) <= 55) 
                    && eventObject.CollisionResult.minimumTranslationVector.Y <= 0
                    && eventObject.CollisionResult.triggeringBody.Center.Y < eventObject.CollisionResult.affectedBody.Center.Y)
                {
                    EventManager.Instance.fireEvent(SurfaceContactEvent.Create(eventObject.TriggeringEntity));
                    EventManager.Instance.fireEvent(EntityOnSurfaceEvent.Create(this.owner, eventObject.TriggeringEntity));
                    /*SurfaceComponent comp = this.owner.find<SurfaceComponent>();
                    if (comp != null) { comp.addEntityOnSurface(eventObject.TriggeringEtity); }*/
                }
            }
        }
    }
}
