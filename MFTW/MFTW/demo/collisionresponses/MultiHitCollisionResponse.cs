using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.events;
using FeInwork.core.collision;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using FeInwork.FeInwork.components;
using FeInwork.FeInwork.util;

namespace FeInwork.collision.responses
{
    /// <summary>
    /// Respuesta que debe aplicarse al momento de colisionar 
    /// un ataque de multiples goles con otra entidad. 
    /// Por defecto solo responde a bodies de colortag 
    /// Color.Red a menos que se le especifique otro.
    /// </summary>
    public class MultiHitCollisionResponse : AbstractCollisionResponse
    {
        public MultiHitCollisionResponse(IEntity owner)
            : base(owner, GameConstants.TANGIBLE_BODY_TAG)
        {
            // Se inicializa a mano mientras se le da soporte al AnimationManager
            // para pasar parametros a los listeners
            this.initialize();
        }

        public MultiHitCollisionResponse(IEntity owner, Color colorTag)
            : base(owner, colorTag)
        {
            this.initialize();
        }

        public override void initialize()
        {
        }

        public override void invoke(CollisionEvent eventObject)
        {
            if (eventObject.CollisionResult.triggeringBody.Owner == this.owner)
            {
                // Si no es multihit entonces guarda la entidad a la que atacará
                // y luego realiza el ataque
                // Hace cierto daño a la entidad
                EventManager.Instance.fireEvent(DamageEvent.Create(eventObject.AffectedEntity, owner.find<PhysicalAttackComponent>().Damage, this.owner, ElementType.NONE));
            }
        }
    }
}
