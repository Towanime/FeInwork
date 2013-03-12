using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.entities;
using FeInwork.FeInwork.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.Core.Base;
using FeInwork.core.managers;

namespace FeInwork.FeInwork.renderers
{
    /// <summary>
    /// Clase usada para renderizar todas las texturas del 
    /// cuarto independientes de entidades
    /// </summary>
    public class StaticObjectsRenderer : IComponent, IDrawUpdateableFE
    {
        private bool isVisible;
        private DrawableEntity owner;
        /// <summary>
        /// Lista de objetos a renderizar
        /// </summary>
        private List<DrawParameters> renderList;
        private DrawParameters drawParams;

        public StaticObjectsRenderer(DrawableEntity owner, List<DrawParameters> renderList)
        {
            this.owner = owner;
            this.renderList = renderList;
            initialize();
        }

        public void initialize()
        {
            Program.GAME.ComponentManager.addComponent(this);
            isVisible = true;
            for (int i = 0; i < renderList.Count; i++)
            {
                DrawParameters objectParameters = renderList[i];
                if (objectParameters.SourceRectangle.IsEmpty)
                {
                    objectParameters.SourceRectangle = new Rectangle(0, 0, objectParameters.Texture.Width, objectParameters.Texture.Height);
                }
            }

            drawParams = new DrawParameters();
            drawParams.Color = Color.White;
            drawParams.Alpha = 1.0f;
        }

        public void DrawUpdate(GameTime gameTime)
        {
            for (int i = 0; i < renderList.Count; i++)
            {
                DrawParameters objectParameters = renderList[i];

                objectParameters.Draw = Program.GAME.Camera.IsInView(
                    new Rectangle((int)objectParameters.Position.X,
                        (int)objectParameters.Position.Y,
                        objectParameters.SourceRectangle.Width,
                        objectParameters.SourceRectangle.Height));
            }

            if (owner.Effects != null)
            {
                for (int i = 0; i < owner.Effects.Count; i++)
                {
                    owner.Effects[i].applyEffect(ref drawParams);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchWithMatrix();

            for (int i = 0; i < renderList.Count; i++)
            {
                DrawParameters objectParameters = renderList[i];
                if (objectParameters.Draw == true)
                {
                    sb.Draw(objectParameters.Texture, objectParameters.Position, objectParameters.SourceRectangle,
                        objectParameters.Color * drawParams.Alpha, objectParameters.Rotation, objectParameters.Origin, objectParameters.Scale,
                        objectParameters.Effects, objectParameters.LayerDepth);
                }
            }
        }

        public bool Visible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        public IEntity Owner
        {
            get { return owner; }
        }
    }
}
