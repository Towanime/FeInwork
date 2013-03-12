using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.util;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using FeInwork.FeInwork.entities;

namespace FeInwork.FeInwork.draweffects
{
    /// <summary>
    /// Hace efecto de fade sobre lo que se va a dibujar
    /// dependiendo de un intervalo y dirección
    /// </summary>
    public class FadeEffect : AbstractDrawEffect
    {
        /// <summary>
        /// Cantidad que se va a utilizar para modificar
        /// la transparencia cada frame
        /// </summary>
        private float fadeAmount;
        /// <summary>
        /// Dirección de la transparencia.
        /// positivo si se va a aumentar y negativo
        /// si se va a disminuir
        /// </summary>
        private int fadeDirection;
        /// <summary>
        /// Minimo indice de alpha
        /// </summary>
        private float fadeMin = 0.0f;
        /// <summary>
        /// Maximo indice de alpha
        /// </summary>
        private float fadeMax = 1.0f;
        /// <summary>
        /// Tipo de fade que se va a realizar
        /// </summary>
        private FADE_TYPE fadeType = FADE_TYPE.IN;

        public enum FADE_TYPE
        {
            IN,
            OUT,
            INOUT
        };

        public FadeEffect(DrawableEntity entityToApply)
            : base(entityToApply)
        {
            this.fadeAmount = 0.01f;
            this.fadeMin = 0.0f;
            this.fadeMax = 1.0f;
        }

        public FadeEffect(DrawableEntity entityToApply, float fadeAmount)
            : base(entityToApply)
        {
            this.fadeAmount = fadeAmount;
            this.fadeMin = 0.0f;
            this.fadeMax = 1.0f;
        }

        public FadeEffect(DrawableEntity entityToApply, float fadeAmount, float fadeMin, float fadeMax)
            : base(entityToApply)
        {
            this.fadeAmount = fadeAmount;
            this.fadeMin = fadeMin;
            this.fadeMax = fadeMax;
        }

        public override void applyEffect(ref DrawParameters drawParameters)
        {
            if(effectInPlace)
            {
                float alpha = MathHelper.Clamp(drawParameters.Alpha + (fadeAmount * fadeDirection), fadeMin, fadeMax);
                drawParameters.Alpha = alpha;

                if (this.fadeType == FADE_TYPE.IN && alpha == fadeMin)
                {
                    effectInPlace = false;
                    return;
                }

                if (this.fadeType == FADE_TYPE.OUT && alpha == fadeMax)
                {
                    effectInPlace = false;
                    return;
                }

                if (this.fadeType == FADE_TYPE.INOUT)
                {
                    if (alpha == fadeMin)
                    {
                        fadeDirection = 1;
                    }
                    else if (alpha == fadeMax)
                    {
                        effectInPlace = false;
                    }
                }
            }
        }

        public bool fade(FADE_TYPE fadeType)
        {
            effectInPlace = true;
            this.fadeType = fadeType;
            if (this.fadeType == FADE_TYPE.OUT)
            {
                fadeDirection = 1;
            }
            else
            {
                fadeDirection = -1;
            }
            return true;
        }
    }
}
