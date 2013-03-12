using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.Core.Interfaces
{
    public interface IDrawUpdateableFE : IDrawableFE
    {
        /// <summary>
        /// Metodo para realizar operaciones pertinentes a
        /// lo que se debe dibujar en el Draw luego de
        /// realizar los Updates principales.
        /// </summary>
        /// <param name="gameTime"></param>
        void DrawUpdate(GameTime gameTime);
    }
}
