using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;

namespace FeInwork.core.Base
{
    /// <summary>
    /// Clase base de la que deberian heredar todos los componentes que controlen input!
    /// </summary>
    public abstract class BaseControlComponent : IComponent, IUpdateableFE 
	{
        /// <summary>
        /// Entidad principal de este componente y que debe controlar.
        /// </summary>
        protected IEntity owner;
        /// <summary>
        /// Si este control debe ser updeteable
        /// </summary>
        protected bool isEnabled;

        public virtual void initialize()
        {
            // heredar y override
        }

        public virtual void Update(GameTime gameTime)
        {
            // heredar y override
        }

        #region Properties

        public IEntity Owner
        {
            get { return this.owner; }
        }

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

        #endregion Properties
    }
}
