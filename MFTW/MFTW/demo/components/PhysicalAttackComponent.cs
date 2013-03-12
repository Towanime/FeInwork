using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework.Input;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.events;
using FeInwork.core.collision;
using FeInwork.collision.responses;
using FeInwork.core.collision.bodies;
using FeInwork.FeInwork.components.interfaces;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.util;

namespace FeInwork.FeInwork.components
{
    public class PhysicalAttackComponent : IComponent, PhyiscalAttackListener
    {
        // guarda una referencia a la entidad dueña
        private IEntity owner;
        private bool isEnabled;

        public PhysicalAttackComponent(IEntity owner, int damage)
        {
            this.owner = owner;
            owner.changeIntProperty(EntityProperty.PhysicalAttack, damage, false);
            initialize();
        }

        // no es necesario llamar a este metodo pues BaseComponent lo llama en su 
        // unico constructor
        public void initialize()
        {
            //carga sus valores y crap
            Program.GAME.ComponentManager.addComponent(this);
            isEnabled = true;
            this.owner.addState(EntityState.Attacking);
            this.owner.addState(EntityState.IsAvailable, true);
            EventManager.Instance.addListener(EventType.PHYSICAL_ATTACK_EVENT, this);
        }

        public IEntity Owner
        {
            get { return owner; }
        }

        public int Damage
        {
            get { return calculateDamage(); }
        }

        public bool Enabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        /// <summary>
        /// Método para calcular el cantidad de daño a realizar
        /// </summary>
        /// <returns>El valor de daño con los bonus de ser necesario</returns>
        private int calculateDamage()
        {
            if(owner.containsIntProperty(EntityProperty.PhysicalAttackBonus))
            {
                return owner.getIntProperty(EntityProperty.PhysicalAttack)
                    + owner.getIntProperty(EntityProperty.PhysicalAttackBonus);
            }
            else
	        {
                return owner.getIntProperty(EntityProperty.PhysicalAttack);
	        }
        }

        public void invoke(events.PhysicalAttackEvent eventObject)
        {
            if (eventObject.Origin.Equals(this.owner))
            {
                //If on air shit or whatever & prend quelque merda
                owner.changeState(EntityState.Attacking, true, true);
                owner.changeState(EntityState.IsAvailable, false, false);
                owner.changeIntProperty(EntityProperty.Action, ActionsList.Attack, true);
            }
        }

    }

}
