using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.listeners;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;
using FeInwork.core.managers;
using FeInwork.FeInwork.events;

namespace FeInwork.FeInwork.components
{
    /// <summary>
    /// Componente hecho para objetos que pueden morir y deben realizar acciónes especificas
    /// despues de haber muerto para ser liberados de memoria al final
    /// </summary>
    public class DeadComponent : BaseComponent, IUpdateableFE, DeadListener, DeadActionListener
    {
        private bool isEnabled;
        /// <summary>
        /// Objetos que aún tienen algo por hacer luego de haber muerto el objeto
        /// </summary>
        private List<object> deadActionObjects;

        public DeadComponent(IEntity owner)
            : base(owner)
        {
            this.initialize();
        }

        public override void initialize()
        {
            this.isEnabled = false;
            EventManager.Instance.addListener(EventType.DEAD_EVENT, this.owner, this);
            this.owner.addState(util.EntityState.Dead, false);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Si ya no hay objetos que tengan que realizar una acción
            // despues de haber muerto el objeto, se manda a remover todas las
            // referencias a esta entidad
            if (deadActionObjects.Count == 0)
            {
                EntityManager.Instance.requestRemoveEntity(this.owner);
                this.Enabled = false;
            }
        }

        public bool Enabled
        {
            get { return this.isEnabled; }
            set { this.isEnabled = value; }
        }

        public void invoke(DeadEvent eventArgs)
        {
            if (!this.Enabled)
            {
                // Se activa y se registra el componente una vez
                // la entidad "muere"
                this.Enabled = true;
                this.deadActionObjects = new List<object>();
                Program.GAME.ComponentManager.addComponent(this);
                // Se registra a DEAD_ACTION_EVENT para saber cuando otros objetos
                // empiecen y terminen acciones de muerte
                EventManager.Instance.addListener(EventType.DEAD_ACTION_EVENT, this.owner, this);
            }
        }

        public void invoke(DeadActionEvent eventArgs)
        {
            if (this.Enabled)
            {
                if (!eventArgs.IsEndOfAction)
                {
                    if (!deadActionObjects.Contains(eventArgs.ActionObject))
                    {
                        deadActionObjects.Add(eventArgs.ActionObject);
                    }
                }
                else
                {
                    deadActionObjects.Remove(eventArgs.ActionObject);
                }
            }            
        }
    }
}
