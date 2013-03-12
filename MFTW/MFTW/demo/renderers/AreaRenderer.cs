using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.entities;
using FeInwork.FeInwork.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.core.managers;

namespace FeInwork.FeInwork.renderers
{
    /// <summary>
    /// Clase usada para renderizar todas las texturas del 
    /// cuarto independientes de entidades
    /// </summary>
    public class AreaRenderer : IComponent, IDrawUpdateableFE
    {
        private bool isVisible;
        private AreaEntity roomOwner;
        /// <summary>
        /// Lista de objetos a renderizar
        /// </summary>
        private List<DrawParameters> renderList;
        private Color backgroundColor = Color.White;
        private Texture2D backgroundTexture;
        private Rectangle viewRectangle;

        public AreaRenderer(AreaEntity roomOwner, List<DrawParameters> renderList)
        {
            this.roomOwner = roomOwner;
            this.renderList = renderList;
            initialize();
        }

        public AreaRenderer(AreaEntity roomOwner, List<DrawParameters> renderList, Color backgroundColor)
        {
            this.roomOwner = roomOwner;
            this.renderList = renderList;
            this.backgroundColor = backgroundColor;
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
                    renderList[i] = objectParameters;
                }
            }
            backgroundTexture = Program.GAME.Content.Load<Texture2D>("teststage/blank_pixel");
        }

        public void DrawUpdate(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for (int i = 0; i < renderList.Count; i++)
            {
                DrawParameters objectParameters = renderList[i];

                viewRectangle.X = (int)objectParameters.Position.X;
                viewRectangle.Y = (int)objectParameters.Position.Y;
                viewRectangle.Width = (int)objectParameters.SourceRectangle.Width;
                viewRectangle.Height = (int)objectParameters.SourceRectangle.Height;

                objectParameters.Draw = Program.GAME.Camera.IsInView(viewRectangle);
            }
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchWithMatrix();

            // Se dibuja el color de fondo por defecto del nivel
            /*SpriteBatchManager.Instance.getSpriteBatchBackground().Draw(backgroundTexture, new Vector2(0, 0), null,
                backgroundColor, 0, Vector2.Zero, new Vector2(Program.GAME.ResolutionWidth, Program.GAME.ResolutionHeight),
                SpriteEffects.None, GameLayers.BACK_BACKGROUND);*/

            for (int i = 0; i < renderList.Count; i++)
            {
                DrawParameters objectParameters = renderList[i];
                if (objectParameters.Draw == true)
                {
                    sb.Draw(objectParameters.Texture, objectParameters.Position, objectParameters.SourceRectangle,
                        objectParameters.Color, objectParameters.Rotation, objectParameters.Origin, objectParameters.Scale,
                        objectParameters.Effects, objectParameters.LayerDepth);
                }
            }            
        }

        public IEntity Owner
        {
            get { return roomOwner; }
        }

        public bool Visible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
    }
}
