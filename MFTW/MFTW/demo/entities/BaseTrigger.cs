using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.collision.bodies;
using FeInwork.core.collision;
using FeInwork.Core.Base;
using FeInwork.core.managers;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.listeners;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.components.interfaces;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;

namespace FeInwork.FeInwork.entities
{
    public class BaseTrigger : Entity, CollisionListener, IUpdateableFE, PositionChangedListener
    {
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

        public BaseTrigger()
        {
            // no zone yet! coz zone necesita owner que es esto mismo, acomodar!
            //this.zone = zone; 
            initialize();
        }

        private new void initialize()
        {
            /*if(zone == null)
            {
               zone = ShapeFactory.CreateCircle("trigger", this, false, false, 300, new Vector2(1200, 800), true);
            }*/
            // acomoda estoooooo
            //this.zone.addCollisionListener(this);
            //CollisionManager.Instance.addContainer(this.zone);
            EventManager.Instance.addCollisionListener(this, this);
            EventManager.Instance.addListener(EventType.POSITION_CHANGED_EVENT, this, this);
            //this.zone.addCollisionListener(this);
            //CollisionManager.Instance.addContainer(this.zone);
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
            if(isActive){
                // verifica si esta en rango todavia
                // se sobreentiende que debe haber uno solo :p
                //List<AbstractCollisionComponent> collisionComponents =
                  //  currentEntity.findByBaseClass<AbstractCollisionComponent>();
                // tiene?!
                //bool atLeastOne = false;
                
                /*foreach(CollisionBody body in collisionComponents[0].ShapeList){
                    // verifica si algun body de la entidad colisiona con el de este trigger
                    CollisionResult result = new CollisionResult();
                    body.Intersect(zone, Vector2.Zero, ref result);
                    if(result.intersect){
                        atLeastOne = true;
                        break;
                    }
                }*/
                //if (!atLeastOne)
                if (!zone.PointInBody(currentEntity.getVectorProperty(EntityProperty.Position)))
                {
                    // tira evento que salio de rango
                    isActive = false;
                    isEnabled = false;
                    EventManager.Instance.fireEvent(TriggerRangeEvent.Create(zone, currentEntity, false));
                }
            }
        }

        public void invoke(PositionChangedEvent eventObject)
        {
            // aqui se recibe nueva posicion para la entidad y se cambia directo.
            // ya le body estara reubicadocollision
            changeVectorProperty(EntityProperty.Position,
                getVectorProperty(EntityProperty.Position) + eventObject.ProjectedDistance, true);
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

        public Vector2 Position
        {
            get 
            { 
                return getVectorProperty(EntityProperty.Position); 
            }
            set
            {
                // tirar evento a ver si se puede
                EventManager.Instance.fireEvent(PositionChangeRequestEvent.Create(this, Position, value));
            }
        }
        #endregion properties
    }
}
