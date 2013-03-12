using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FeInwork.core.managers
{
    /// <summary>
    /// BUSCAR mejor manera para tener menos spritebatchs :S 
    /// organizando cosas a dibujar por orden de capas quizas...
    /// in the meanwhile spritesheets para todos.
    /// </summary>
    public class SpriteBatchManager
    {
        private static SpriteBatchManager instance;
        /// <summary>
        ///  device para sacar los sprite batches
        /// </summary>
        private GraphicsDevice graphicDevice;
        /// <summary>
        /// Spritebatch para las cosas que se dibujaran y sean afectadas por la matriz de desplazamiento de la camara
        /// </summary>
        private SpriteBatch spriteBatchWithMatrix;        
        /// <summary>
        /// Este es para dibujar cosas que se encuentren entre el el hud y la zona de juego.
        /// Principalmente es para parallax effect delante del area de juego, esto da un buen effecto.
        /// </summary>
        private SpriteBatch spriteBatchBetween;
        /// <summary>
        /// Para hud principalmente, tambien esta ordenado por capas.
        /// </summary>
        private SpriteBatch spriteBatchHud;
                /// <summary>
        /// SpriteBatch para los fondos de niveles. No es afectado por matriz de desplazamiento.
        /// </summary>
        private SpriteBatch spriteBatchBackground;
        /// <summary>
        /// Spritebatch para las cosas que se dibujaran y sean afectadas por la matriz de desplazamiento de la camara
        /// </summary>
        private SpriteBatch spriteBatchDebugWithMatrix;
        /// <summary>
        /// Para hud principalmente, tambien esta ordenado por capas.
        /// </summary>
        private SpriteBatch spriteBatchDebugHud;
        /// <summary>
        /// True para especificar si ya se le hizo begin a los sprites batches y no se pueda hacer luehgo again.
        /// </summary>
        private bool hasBegun;

        private bool hasBegunDebug;

        private SpriteBatchManager()
        {
        }

        public void initialize(GraphicsDevice graphicDevice)
        {
            spriteBatchWithMatrix = new SpriteBatch(graphicDevice);
            spriteBatchBetween = new SpriteBatch(graphicDevice);
            spriteBatchHud = new SpriteBatch(graphicDevice);
            spriteBatchBackground = new SpriteBatch(graphicDevice);
            spriteBatchDebugWithMatrix = new SpriteBatch(graphicDevice);
            spriteBatchDebugHud = new SpriteBatch(graphicDevice);
        }

        public void begin()
        {
            if (!hasBegun)
            {
                spriteBatchBackground.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                spriteBatchWithMatrix.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Program.GAME.Camera.Transform);
                spriteBatchBetween.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                spriteBatchHud.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                hasBegun = true;
            }
        }

        public void end()
        {
            spriteBatchBackground.End();
            spriteBatchWithMatrix.End();
            spriteBatchBetween.End();
            spriteBatchHud.End();
            hasBegun = false;
        }

        public void beginDebug()
        {
            if (!hasBegunDebug)
            {
                spriteBatchDebugWithMatrix.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Program.GAME.Camera.Transform);
                spriteBatchDebugHud.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                hasBegunDebug = true;
            }
        }

        public void endDebug()
        {
            spriteBatchDebugWithMatrix.End();
            spriteBatchDebugHud.End();
            hasBegunDebug = false;
        }

        public SpriteBatch getSpriteBatchBackground()
        {
            return spriteBatchBackground;
        }

        public SpriteBatch getSpriteBatchWithMatrix()
        {
            return spriteBatchWithMatrix;
        }

        public SpriteBatch getSpriteBatchBetween()
        {
            return spriteBatchBetween;
        }

        public SpriteBatch getSpriteBatchHud()
        {
            return spriteBatchHud;
        }

        public SpriteBatch getSpriteBatchDebugWithMatrix()
        {
            return spriteBatchDebugWithMatrix;
        }

        public SpriteBatch getSpriteBatchDebugHud()
        {
            return spriteBatchDebugHud;
        }

        public GraphicsDevice GraphicDevice
        {
            get { return graphicDevice; }
            set { this.graphicDevice = value; }
        }

        public static SpriteBatchManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpriteBatchManager();
                }
                return instance;
            }
        }
    }
}
