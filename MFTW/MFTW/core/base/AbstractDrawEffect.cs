using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.entities;

namespace FeInwork.Core.Base
{
    /// <summary>
    /// Clase base para efectos de dibujado que se vayan a aplicar sobre una
    /// entidad. Estos efectos pueden depender de eventos o estados y son
    /// llamados con un pase de DrawParameters pre-configurados.
    /// </summary>
    public abstract class AbstractDrawEffect
    {
        protected DrawableEntity entityToApply;
        protected bool effectInPlace;

        public AbstractDrawEffect(DrawableEntity entityToApply)
        {
            this.entityToApply = entityToApply;
        }

        public abstract void applyEffect(ref DrawParameters drawParameters);

        /// <summary>
        /// Entidad a la cual aplicarle el efecto
        /// </summary>
        public DrawableEntity EntityToApply
        {
            get { return this.entityToApply; }
        }

        /// <summary>
        /// Indica si el efecto esta sucediendo en este momento
        /// </summary>
        public bool EffectInPlace
        {
            get { return this.effectInPlace; }
        }
    }
}
