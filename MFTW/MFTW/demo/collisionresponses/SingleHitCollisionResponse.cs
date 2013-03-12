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
    /// un ataque de single hit con otra entidad. Por defecto solo responde a bodies
    /// de colortag Color.Red a menos que se le especifique otro.
    /// </summary>
    public class SingleHitCollisionResponse : AbstractCollisionResponse
    {
        private List<IEntity> hittedEntities;

        public SingleHitCollisionResponse(IEntity owner)
            : base(owner, GameConstants.TANGIBLE_BODY_TAG)
        {
            // Se inicializa a mano mientras se le da soporte al AnimationManager
            // para pasar parametros a los listeners
            this.initialize();
        }

        public SingleHitCollisionResponse(IEntity owner, Color colorTag)
            : base(owner, colorTag)
        {
            this.initialize();
        }

        public override void initialize()
        {
            // Si no es multihit crea una lista de entidades
            // para tener su referencia al momento de haberle pegado
            // y así no pegarle de nuevo
            hittedEntities = new List<IEntity>();
        }

        public override void invoke(CollisionEvent eventObject)
        {
            if (eventObject.CollisionResult.triggeringBody.Owner == this.owner)
            {
                IEntity otherEntity = eventObject.CollisionResult.affectedBody.Owner;
                // Si no es multihit entonces guarda la entidad a la que atacará
                // y luego realiza el ataque
                if (!hittedEntities.Contains(otherEntity))
                {
                    hittedEntities.Add(otherEntity);
                    // Hace cierto daño a la entidad
                    EventManager.Instance.fireEvent(DamageEvent.Create(otherEntity, owner.find<PhysicalAttackComponent>().Damage, this.owner, ElementType.NONE));
                }
            }
        }
    }
}
