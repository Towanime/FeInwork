using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.util;

namespace FeInwork.FeInwork.components
{
    public class SpecialActionManagerComponent : BaseComponent, IUpdateableFE, AlertSpecialActionListener, MoveExecutedListener
    {
        private bool isEnabled;
        /// <summary>
        /// Indica si el jugador desea realizar una acción
        /// </summary>
        private bool performAction;
        /// <summary>
        /// Cola de acciones clasificadas por tipo de acción y entidad que está
        /// haciendo la petición. NOTA: sujeto a cambio
        /// </summary>
        private Dictionary<ActionType, IEntity> actionQueue = new Dictionary<ActionType, IEntity>();

        public SpecialActionManagerComponent(IEntity owner)
            : base(owner)
        {
            this.initialize();
        }

        public override void initialize()
        {
            this.isEnabled = true;
            // Indica si la entidad esta disponible para realizar una acción
            this.owner.addState(EntityState.IsAvailable, true);
            Program.GAME.ComponentManager.addComponent(this);
            EventManager.Instance.addListener(EventType.MOVE_EVENT, this.owner, this);
            EventManager.Instance.addListener(EventType.ALERT_ACTION_EVENT, this.owner, this);
        }

        public void Update(GameTime gameTime)
        {            
            if (actionQueue.Count > 0)
            {
                bool isAvailable = this.owner.getState(EntityState.IsAvailable);
                // Se realiza la acción si estas dos condiciones se cumplen
                if (isAvailable && performAction)
                {
                    EventManager.Instance.fireEvent(PerformSpecialActionEvent.Create(this.owner, actionQueue.First().Value, actionQueue.First().Key));
                }
                actionQueue.Clear();
            }
            // Luego de cada frame se resetea el boolean de querer realizar acción
            this.performAction = false;
        }

        /// <summary>
        /// Agrega el tipo de acción especifico a la cola junto a la entidad
        /// que realizó la petición
        /// </summary>
        /// <param name="eventObject"></param>
        public void invoke(AlertSpecialActionEvent eventObject)
        {
            // si no existe se agrega una nueva lista
            if (!actionQueue.ContainsKey(eventObject.ActionType))
            {
                actionQueue.Add(eventObject.ActionType, eventObject.RequestingEntity);
            }
            else
            {
                actionQueue[eventObject.ActionType] = eventObject.RequestingEntity;
            }
        }

        public void invoke(MoveEvent eventObject)
        {
            if (eventObject.MoveType == MoveEvent.MOVE_TYPE.ACTION)
            {
                this.performAction = true;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
            }
        }
    }
}
