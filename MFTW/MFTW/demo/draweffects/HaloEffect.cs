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
    /// Efecto para cuando una entidad tenga invincibilidad
    /// por haber recibido daño recientemente
    /// </summary>
    public class HaloEffect : AbstractDrawEffect
    {
        /// <summary>
        /// Intervalo de cada cuantos frames va a dibujar o no dibujar
        /// </summary>
        private int flickInterval = 10;
        /// <summary>
        /// Frame actual
        /// </summary>
        private int flickCounter = 0;
        /// <summary>
        /// Si dibuja o no dibuja
        /// </summary>
        private bool flick;

        public HaloEffect(DrawableEntity entityToApply)
            : base(entityToApply)
        {

        }

        public override void applyEffect(ref DrawParameters drawParameters)
        {
            bool haloState = entityToApply.getState(EntityState.Halo);

            // Si el estado esta activado y el efecto no ha comenzado
            // entonces se comienza el efecto y se resetean los campos
            if (haloState && !effectInPlace)
            {
                effectInPlace = true;
                flickCounter = flickInterval;
                flick = true;
            }
            // De lo contrario si el estado esta desactivado y el efecto esta
            // corriendo entonces se termina el efecto
            else if(!haloState && effectInPlace)
            {
                effectInPlace = false;
            }

            if (effectInPlace)
            {
                if (flickCounter <= 0)
                {
                    flick = !flick;
                    flickCounter = flickInterval;
                }
                flickCounter--;
                if (flick) drawParameters.Draw = false;
            }
        }
    }
}
