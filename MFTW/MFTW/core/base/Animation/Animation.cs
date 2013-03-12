using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FeInwork.Core.Base.Animation
{
    public class Animation
    {
        private int id;
        /// <summary>
        /// Es el frame da animiación que se está mostrando
        /// </summary>
        private int currentAnimationFrame;
        /// <summary>
        /// Representa los frames de vida que le quedan al frame de animación
        /// </summary>
        private int currentRemainingFrames;
        private int frameHeight;
        private int frameWidth;
        private int intervalFrames;
        private int originx;
        private int originy;
        private int offsetx;
        private int offsety;
        private Vector2 scale;
        private bool isGoingFordward;
        private List<AnimationFrame> animationFrames;
        private string collisionInfo;

        /// <summary>
        /// Clase para generar una animación
        /// </summary>
        /// <param name="id">El nombre de la animación</param>
        public Animation(int id)
        {
            this.id = id;
            animationFrames = new List<AnimationFrame>();
            this.isGoingFordward = true;
            this.currentAnimationFrame = 0;
            this.scale = Vector2.One;
        }

        /// <summary>
        /// Clase para generar una animación
        /// </summary>
        /// <param name="id">El nombre de la animación</param>
        /// <param name="totalFrames">El total de cuadros de animación que contiene</param>
        /// <param name="frameHeight">El alto a recortar por cuadro</param>
        /// <param name="frameWidth">El ancho a recortar por cuadro</param>
        /// <param name="intervalFrames">El tiempo que durará vivo cada cuadro recortado</param>
        /// <param name="originx">Origen en X donde empieza la línea de animación X</param>
        /// <param name="originy">Origen en Y donde empieza la línea de animación Y</param>
        public Animation(int id, int frameWidth, int frameHeight, int intervalFrames, int originx, int originy)
        {
            this.id = id;
            animationFrames = new List<AnimationFrame>();
            this.isGoingFordward = true;
            this.currentAnimationFrame = 0;
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            this.intervalFrames = intervalFrames;
            this.originx = originx;
            this.originy = originy;
            this.scale = Vector2.One;
        }

        /// <summary>
        /// Clase para generar una animación
        /// </summary>
        /// <param name="id">El nombre de la animación</param>
        /// <param name="totalFrames">El total de cuadros de animación que contiene</param>
        /// <param name="frameHeight">El alto a recortar por cuadro</param>
        /// <param name="frameWidth">El ancho a recortar por cuadro</param>
        /// <param name="intervalFrames">El tiempo que durará vivo cada cuadro recortado</param>
        /// <param name="originx">Origen en X donde empieza la línea de animación X</param>
        /// <param name="originy">Origen en Y donde empieza la línea de animación Y</param>
        /// <param name="scale">Escala por defecto para todos los frames a crear</param>
        public Animation(int id, int frameWidth, int frameHeight, int intervalFrames, int originx, int originy, Vector2 scale)
        {
            this.id = id;
            animationFrames = new List<AnimationFrame>();
            this.isGoingFordward = true;
            this.currentAnimationFrame = 0;
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            this.intervalFrames = intervalFrames;
            this.originx = originx;
            this.originy = originy;
            this.scale = scale;
        }

        /// <summary>
        /// Clase para generar una animación
        /// </summary>
        /// <param name="id">El nombre de la animación</param>
        /// <param name="totalFrames">El total de cuadros de animación que contiene</param>
        /// <param name="frameHeight">El alto a recortar por cuadro</param>
        /// <param name="frameWidth">El ancho a recortar por cuadro</param>
        /// <param name="intervalFrames">El tiempo que durará vivo cada cuadro recortado</param>
        /// <param name="originx">Origen en X donde empieza la línea de animación X</param>
        /// <param name="originy">Origen en Y donde empieza la línea de animación Y</param>
        /// <param name="scale">Escala por defecto para todos los frames a crear</param>
        public Animation(int id, int frameWidth, int frameHeight, int intervalFrames, int originx, int originy, Vector2 scale,
            int offsetx, int offsety)
        {
            this.id = id;
            animationFrames = new List<AnimationFrame>();
            this.isGoingFordward = true;
            this.currentAnimationFrame = 0;
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            this.intervalFrames = intervalFrames;
            this.originx = originx;
            this.originy = originy;
            this.scale = scale;
            this.offsetx = offsetx;
            this.offsety = offsety;
        }

        /// <summary>
        /// Clase para generar una animación
        /// </summary>
        /// <param name="id">El nombre de la animación</param>
        /// <param name="totalFrames">El total de cuadros de animación que contiene</param>
        /// <param name="frameHeight">El alto a recortar por cuadro</param>
        /// <param name="frameWidth">El ancho a recortar por cuadro</param>
        /// <param name="intervalFrames">El tiempo que durará vivo cada cuadro recortado</param>
        /// <param name="originx">Origen en X donde empieza la línea de animación X</param>
        /// <param name="originy">Origen en Y donde empieza la línea de animación Y</param>
        /// <param name="scale">Escala por defecto para todos los frames a crear</param>
        /// <param name="scale">CollisionInfo por defecto para todos los frames</param>
        public Animation(int id, int frameWidth, int frameHeight, int intervalFrames, int originx, int originy, Vector2 scale,
            int offsetx, int offsety, string collisionInfo)
        {
            this.id = id;
            animationFrames = new List<AnimationFrame>();
            this.isGoingFordward = true;
            this.currentAnimationFrame = 0;
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            this.intervalFrames = intervalFrames;
            this.originx = originx;
            this.originy = originy;
            this.scale = scale;
            this.offsetx = offsetx;
            this.offsety = offsety;
            this.collisionInfo = collisionInfo;
        }

        public Vector2 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        public bool IsGoingFordwad
        {
            get
            {
                return isGoingFordward;
            }
            set
            {
                isGoingFordward = false;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Agrega n cantidad de frames construidos por defecto
        /// </summary>
        /// <param name="n">La cantidad de frames a agregar</param>
        public void addAllDefaultFrames(int n)
        {
            for (int i = 0; i < n; i++)
            {
                int x = animationFrames.Count * frameWidth + originx;
                animationFrames.Add(new AnimationFrame(new Rectangle(x, originy, frameWidth, frameHeight), intervalFrames, collisionInfo, scale, offsetx, offsety));
            }
        }

        public void addFrame(AnimationFrame frame)
        {
            animationFrames.Add(frame);
        }

        /// <summary>
        /// Avanza un cuadro en la animación de acuerdo a la dirección
        /// </summary>
        public void changeAnimationFrame()
        {
            if (isGoingFordward)
            {
                currentAnimationFrame = (currentAnimationFrame + 1 < animationFrames.Count) ? currentAnimationFrame + 1 : 0;
            }
            else
            {
                currentAnimationFrame = (currentAnimationFrame - 1 > 0) ? currentAnimationFrame - 1 : animationFrames.Count - 1;
            }

            currentRemainingFrames = animationFrames[currentAnimationFrame].intervalFrames;
        }

        /// <summary>
        /// Es consumido un frame gráfico, si se acabó el frame de animación actual
        /// se pasa al siguiente
        /// </summary>
        public String consumeFrame(bool isRepeating)
        {
            if (--currentRemainingFrames <= 0)
            {
                //Return End Of Frame
                changeAnimationFrame();
                if (!isRepeating && this.currentAnimationFrame == 0) return "EOF";

                return animationFrames[currentAnimationFrame].collisionInfo;
            }
            else return null;
        }

        public AnimationFrame getCurrentFrame()
        {
            return animationFrames[currentAnimationFrame];
        }

        public void resetFrames()
        {
            currentAnimationFrame = 0;
        }

        public string refillRemainingFrames()
        {
            currentRemainingFrames = animationFrames[currentAnimationFrame].intervalFrames;
            return animationFrames[currentAnimationFrame].collisionInfo;
        }
    }
}
