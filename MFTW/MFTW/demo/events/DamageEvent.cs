using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.components;

namespace FeInwork.FeInwork.events
{
    public class DamageEvent: AbstractEvent
    {
        private int damage;
        private IEntity attacker;
        private bool isForcingResolution;
        private ElementType elementType;

        /// <summary>
        /// Si está activo fuerza aplicar el daño al personaje sin aplicar Halo
        /// </summary>
        public bool IsForcingResolution
        {
            get { return isForcingResolution; }
            set { isForcingResolution = value; }
        }        

        public ElementType ElementType
        {
            get { return elementType; }
            set { elementType = value; }
        }

        public int Damage
        {
            get { return damage; }
        }

        public IEntity Attacker
        {
            get { return this.attacker; }
        }

        private DamageEvent(object origin, int damage, IEntity attacker, ElementType elementType) :
            base(origin, EventType.DAMAGE_EVENT)
        {
            this.damage = -Math.Abs(damage);
            this.attacker = attacker;
            this.elementType = elementType;
        }

        public static DamageEvent Create(object origin, int damage, IEntity attacker, ElementType elementType)
        {
            DamageEvent returningEvent = EventManager.Instance.GetEventFromType<DamageEvent>(EventType.DAMAGE_EVENT);

            if (attacker == Program.GAME.WorldManager.MainEntity)
            {
                //damage = Program.GAME.WorldManager.MainEntity.find<AffinityComponent>().calcDamageByAffinity(elementType, damage);
            }

            if (returningEvent == null)
            {
                returningEvent = EventManager.Instance.AddEventToPool(new DamageEvent(origin, damage, attacker, elementType));
            }
            else
            {
                returningEvent.damage = -Math.Abs(damage);
                returningEvent.attacker = attacker;
                returningEvent.origin = origin;
                returningEvent.elementType = elementType;
            }

            return returningEvent;
        }
    }
}
