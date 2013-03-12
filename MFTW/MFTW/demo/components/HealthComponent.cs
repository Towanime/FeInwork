using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.renderers;
using FeInwork.Core.Managers;
using Microsoft.Xna.Framework.Input;
using FeInwork.FeInwork.events;
using FeInwork.Core.Util;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.util;
using FeInwork.Core.Listeners;
using FeInwork.core.events;

namespace FeInwork.FeInwork.components
{
    public class HealthComponent : BaseComponent, IUpdateableFE, CureListener, DamageListener
    {
        private bool isEnabled;
        /// <summary>
        /// Vida actual
        /// </summary>
        private int currentHealth;
        /// <summary>
        /// Vida maxima
        /// </summary>
        private int maxHealth;
        /// <summary>
        /// Contador de frames de halo actuales
        /// </summary>
        private int currentHaloFrames;
        /// <summary>
        /// Frames de halo
        /// </summary>
        private int maxHaloFrames;

        private Dictionary<ElementType, float> elementMultipliers;

        public HealthComponent(IEntity owner, Dictionary<ElementType, float> elementMultipliers, int maxHealth)
            : base(owner)
        {
            this.currentHealth = maxHealth;
            this.maxHealth = maxHealth;
            if (elementMultipliers == null) elementMultipliers = UtilMethods.getGenericDamageMultipliers();
            this.elementMultipliers = elementMultipliers;
            initialize();
        }

        public HealthComponent(IEntity owner, Dictionary<ElementType, float> elementMultipliers, int maxHealth, int currentHealth)
            : base(owner)
        {
            this.currentHealth = currentHealth;
            this.maxHealth = maxHealth;
            if (elementMultipliers == null) elementMultipliers = UtilMethods.getGenericDamageMultipliers();
            this.elementMultipliers = elementMultipliers;
            initialize();
        }

        public HealthComponent(IEntity owner, Dictionary<ElementType, float> elementMultipliers, int maxHealth, int currentHealth, int maxHaloFrames)
            : base(owner)
        {
            this.currentHealth = currentHealth;
            this.maxHealth = maxHealth;
            if (elementMultipliers == null) elementMultipliers = UtilMethods.getGenericDamageMultipliers();
            this.elementMultipliers = elementMultipliers;
            this.maxHaloFrames = Math.Abs(maxHaloFrames);
            initialize();
        }

        public override void initialize()
        {
            Program.GAME.ComponentManager.addComponent(this);
            EventManager.Instance.addListener(EventType.DAMAGE_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.CURE_EVENT, this.owner, this);
            this.owner.addState(EntityState.Invincible, true);
            this.owner.addState(EntityState.Halo, false);
        }

        public void Update(GameTime gameTime)
        {
            if (currentHaloFrames > 0)
            {
                this.currentHaloFrames--;
            }
            else
            {
                this.currentHaloFrames = 0;
                this.owner.changeState(EntityState.Halo, false, false);
                this.owner.changeState(EntityState.Invincible, false, false);
                this.isEnabled = false;
            }
        }

        public int Current
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        public int Max
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }


        public void modHealth(float value, bool isForcingResolution)
        {
            // Si esta en modo halo y va a curarse entonces lo deja
            int damageBonus = owner.containsFloatProperty(EntityProperty.DamageMultiplierBonus) ?
            (int)(owner.getFloatProperty(EntityProperty.DamageMultiplierBonus) * value) : 0;

            if (currentHaloFrames > 0)
            {
                if(value > 0) currentHealth = (int)MathHelper.Clamp(currentHealth + value, 0, maxHealth);
                else if (isForcingResolution)
                {
                    currentHealth = (int)MathHelper.Clamp(currentHealth + value + damageBonus, 0, maxHealth);
                    checkDead();
                }
            }            
            else if (currentHaloFrames == 0 )
            {
                // Si no hay halo entonces se realiza el cambio
                // indiferentemente si el valor es positivo o negativo
                currentHealth = (int)MathHelper.Clamp(currentHealth + value + damageBonus, 0, maxHealth);
                if (value < 0) hasBeenHarmed();
            }
        }

        private void hasBeenHarmed()
        {
            // Si el valor es negativo se envia un evento de daño
            EventManager.Instance.fireEvent(HarmedEvent.Create(this.owner));

            // Si existen halo frames que agregar y la entidad no ha muerto entonces
            // se agregan y se le pone invincibilidad a la entidad
            if (checkDead()) { }
            else if (maxHaloFrames > 0 && owner.getState(EntityState.Dead) != true)
            {
                currentHaloFrames = maxHaloFrames;
                this.owner.changeState(EntityState.Halo, true, false);
                this.owner.changeState(EntityState.Invincible, true, false);
                this.isEnabled = true;
            }
        }

        public bool checkDead()
        {
            // Se envia evento de muerte en caso de que la vida haya llegado a 0
            if (currentHealth <= 0 && owner.getState(EntityState.Dead) != true)
            {
                EventManager.Instance.fireEvent(DeadEvent.Create(owner));
                return true;
            }
            return false;
        }

        public void invoke(DamageEvent eventObject)
        {
            /*if (eventObject.Target == this.owner)
            {*/
                float damage = eventObject.Damage;
                if (this.elementMultipliers.ContainsKey(eventObject.ElementType))
                {
                    damage *= this.elementMultipliers[eventObject.ElementType];
                }
                modHealth(damage, eventObject.IsForcingResolution);
            //}
        }

        public void invoke(CureEvent eventObject)
        {
            //Sólo yo me curo
            if (eventObject.Origin == this.owner) modHealth(eventObject.Cure, false);
        }

        public bool Enabled
        {
            get { return this.isEnabled; }
            set { this.isEnabled = value; }
        }
    }
}