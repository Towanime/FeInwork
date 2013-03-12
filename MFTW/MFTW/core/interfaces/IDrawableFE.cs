using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.Core.Interfaces
{
    public interface IDrawableFE
    {
        void Draw(GameTime gameTime);

        bool Visible { get; set; }
    }
}
