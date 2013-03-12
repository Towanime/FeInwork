using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.listeners;
using FeInwork.Core.Interfaces;
using FeInwork.core.collision.bodies;
using Microsoft.Xna.Framework;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;
using FeInwork.FeInwork.events;
using FeInwork.core.managers;

namespace FeInwork.FeInwork.triggers
{
    /// <summary>
    /// Otor tipo de trigger que adapta a cualquier entidad y cambia su posicion cuando
    /// la entidad cambia usando position change event
    /// </summary>
    public class AttachableTrigger : CollisionListener, IUpdateableFE
    {
        /// <summary>
        /// Entidad a la que se adapta.
        /// </summary>
        private IEntity owner;
        /// <summary>
        /// Zona del trigger, puede ser cualquier tipo de body
        /// </summary>
        private CollisionBody zone;
        /// <summary>
        /// Tag para filtrar por color si es necesario.
        /// Por defecto los triggers son de color Color.AliceBlue
        /// Si se quiere que una entidad active triggers entonces agregarle un collisionbody
        /// que tenga este color de tag.
        /// </summary>
        private Color? colorTag = Color.AliceBlue;
        /// <summary>
        /// si esta activo el trigger.
        /// </summary>
        private bool isActive;
        /// <summary>
        /// Entidad que activo el trigger.
        /// </summary>
        private IEntity currentEntity;
        //
        private bool isEnabled;

        /// <summary>
        /// Especifica la zona de una vez ya que el owner de sa zona sera la entidad 
        /// a la que este trigger se quiere adaptar y no el trigger como tal (a diferencia de BaseTrigger)
        /// </summary>
        /// <param name="zone"></param>
        public AttachableTrigger(CollisionBody zone)
        {
            // no zone yet! coz zone necesita owner que es esto mismo, acomodar!
            this.zone = zone; 
            initialize();
        }

        private void initialize()
        {
            owner = zone.Owner;
            EventManager.Instance.addCollisionListener(owner, this);
            //EventManager.Instance.addListener(EventType.POSITION_CHANGED_EVENT, owner, this);
            Program.GAME.ComponentManager.addUpdateableOnly(this);
        }

        public void invoke(CollisionEvent eventObject)
        {
            if (!isActive)
            {
                currentEntity = eventObject.TriggeringEntity;
                isActive = true;
                isEnabled = true;
                EventManager.Instance.fireEvent(TriggerRangeEvent.Create(zone, currentEntity, true));
            }
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (!zone.PointInBody(currentEntity.getVectorProperty(EntityProperty.Position)))
                {
                    // tira evento que salio de rango
                    isActive = false;
                    isEnabled = false;
                    EventManager.Instance.fireEvent(TriggerRangeEvent.Create(zone, currentEntity, false));
                }
            }
        }

        #region properties
        public bool Enabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }

        public CollisionBody Zone
        {
            set
            {
                if (this.zone != null)
                {
                    CollisionManager.Instance.removeContainer(this.zone);
                }
                this.zone = value;
                CollisionManager.Instance.addContainer(this.zone);
            }
        }

        public Color? ColorTag
        {
            get { return colorTag; }
        }
        #endregion
    }
}
