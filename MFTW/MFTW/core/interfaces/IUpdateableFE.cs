using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.Core.Interfaces
{
    public interface IUpdateableFE
    {
        /// <summary>
        /// Metodo para aplicar logica de un componente en cada update.
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);

        /// <summary>
        /// True si el componente puede ser actualizado ( llamar a update() )
        /// </summary>
        bool Enabled { get; set; }
    }
}
