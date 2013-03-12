using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.core.collision;

namespace FeInwork.Core.Base.Animation
{
    /// <summary>
    /// Estructura de cuadro de animación que se muestra en pantalla
    /// </summary>
    public struct AnimationFrame
    {
        public Rectangle rectangle;
        public int intervalFrames;
        //Contiene la info de los bodies, separado por espacio segun orden predefinido
        //punto y coma para separar distintos bodies
        public String collisionInfo;
        public int offsetx;
        public int offsety;
        public Vector2 scale;

        /// <summary>
        /// Un frame de animación
        /// </summary>
        /// <example>new AnimationFrame(new Rectangle(220, 200, 55, 38), 5,
        /// "Rectangle TestBody False 180 70 0 -20 False;AlchemistDemo.collision.responses.SingleHitCollisionResponse!TestBody;10",5.0f, 50, -30)</example>
        /// <param name="rect">Indica posición, ancho y alto</param>
        /// <param name="totalIntervalFrames">Total tiempo que permanece vivo</param>
        /// <param name="collInfo">[Parametros de creación ShapeFactory.Create(...)]*,[NamespaceResponse[!Target]*,[TimeAlive]*;</param>
        public AnimationFrame(Rectangle rect, int totalIntervalFrames, string collInfo, Vector2 currentScale, int offx, int offy)
        {
            rectangle = rect;
            intervalFrames = totalIntervalFrames;
            collisionInfo = collInfo;
            scale = currentScale;
            offsetx = offx;
            offsety = offy;
        }
    }
}
