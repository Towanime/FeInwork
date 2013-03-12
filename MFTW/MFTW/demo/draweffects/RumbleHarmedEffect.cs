using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.util;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using FeInwork.Core.Util;
using FeInwork.Core.Base;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.FeInwork.entities;

namespace FeInwork.FeInwork.draweffects
{
    /// <summary>
    /// Efecto de rumble para cuando una entidad reciba daño. La posicion de dibujo se cambia en 4 lados
    /// segun un intervalo de frames
    /// </summary>
    public class RumbleHarmedEffect : AbstractDrawEffect, HarmedListener
    {
        private bool isHarmed;
        private int framesPerRumbleSide = 3;
        private int currentFrame;
        private int currentSide;

        public RumbleHarmedEffect(DrawableEntity entityToApply)
            : base(entityToApply)
        {
            EventManager.Instance.addListener(EventType.HARMED_EVENT, this.entityToApply, this);
            EventManager.Instance.addListener(EventType.TRANCE_BEGIN_EVENT, this);
            EventManager.Instance.addListener(EventType.TRANCE_END_EVENT, this);
        }

        public override void applyEffect(ref DrawParameters drawParameters)
        {
            if (isHarmed && effectInPlace == false)
            {
                currentSide = 1;
                currentFrame = 1;
                effectInPlace = true;
            }
            else
            {
                isHarmed = false;
            }

            if (effectInPlace)
            {
                // Si ha hecho una iteracion de frames completa entonces se cambia
                // de lado
                if (currentFrame > framesPerRumbleSide)
                {
                    currentSide++;
                    currentFrame = 1;
                }

                // Si ya ha pasado por los 4 lados entonces termina el efecto
                if (currentSide > 4)
                {
                    effectInPlace = false;
                    return;
                }

                // Elige la posicion dependiendo del lado
                switch (currentSide)
                {
                    case 1:
                        drawParameters.Position += new Vector2(-5, 0);
                    break;
                    case 2:
                        drawParameters.Position += new Vector2(0, -5);
                    break;
                    case 3:
                        drawParameters.Position += new Vector2(5, 0);
                    break;
                    case 4:
                        drawParameters.Position += new Vector2(0, 5);
                    break;
                }

                currentFrame++;
            }
        }

        public void invoke(HarmedEvent eventArgs)
        {
            this.isHarmed = true;
        }

    }
}
