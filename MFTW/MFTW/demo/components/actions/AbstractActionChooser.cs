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
    /// <summary>
    /// Componente para el manejo de acciones por defecto desencadenadas luego de terminar una animacion.
    /// </summary>
    public class BasicActionChooser : BaseComponent
    {
        public BasicActionChooser(IEntity owner)
            : base(owner)
        {
            this.initialize();
        }

        public override void initialize()
        {
            Program.GAME.ComponentManager.addComponent(this);
        }

        /// <summary>
        /// Metodo para obtener una acción por defecto y establecer cualquier propiedad
        /// UTILIZAR ACTIONSLIST PARA ESTOS INT!
        /// </summary>
        /// <returns>La nueva acción a realizar</returns>
        public virtual int getNewAction()
        {
            return ActionsList.Default;
        }

    }
}
