using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Util;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.world;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.entities;
using FeInwork.core.managers;

namespace FeInwork.FeInwork.renderers
{
    public class BasicRenderer : IComponent, IDrawUpdateableFE
    {
        // nombre del resource a cargar en este caso grafica
        private DrawableEntity owner;
        private string asset;
        private Rectangle sourceRectangle;
        private DrawParameters drawParameters;
        private Texture2D texture;
        private DrawParameters[] pastFrames;
        private int fadeFramesNumber;
        private int intervalFadeFrames;
        private int currentFadeFrame;
        private Rectangle spriteCameraRectangle;


        public BasicRenderer(DrawableEntity owner, string asset)
        {
            this.owner = owner;
            this.asset = asset;
            initialize();
        }

        public BasicRenderer(DrawableEntity owner, string asset, Rectangle sourceRectangle)
        {
            this.owner = owner;
            this.asset = asset;
            this.sourceRectangle = sourceRectangle;
            initialize();
        }

        public BasicRenderer(DrawableEntity owner, string asset, Rectangle sourceRectangle, int fadeFramesNumber, int intervalFadeFrames)
        {
            this.owner = owner;
            this.asset = asset;
            this.sourceRectangle = sourceRectangle;
            if (fadeFramesNumber > 0 && intervalFadeFrames > 0)
            {
                this.pastFrames = new DrawParameters[fadeFramesNumber];
                this.fadeFramesNumber = fadeFramesNumber;
                this.intervalFadeFrames = intervalFadeFrames;
                this.currentFadeFrame = intervalFadeFrames;
            }
            initialize();
        }

        public void initialize()
        {
            Program.GAME.ComponentManager.addComponent(this);
            this.texture = Program.GAME.Content.Load<Texture2D>(asset);
            if (sourceRectangle.IsEmpty)
            {
                sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            }

            drawParameters = new DrawParameters(texture);
            drawParameters.SourceRectangle = sourceRectangle;
            drawParameters.Color = Color.White;
            drawParameters.LayerDepth = GameLayers.MIDDLE_PLAY_AREA;
        }

        public void DrawUpdate(GameTime gameTime)
        {
            if ((drawParameters.Draw = owner.Visible) == true)
            {
                bool facingRight = owner.IsFacingRight;
                int orientation = facingRight == false ? -1 : 1;
                Vector2 scale = owner.Scale;

                Vector2 ownerPosition = owner.Position;

                Vector2 origin = drawParameters.Origin;
                origin.X = sourceRectangle.Width / 2;
                origin.Y = sourceRectangle.Height / 2;

                int x1 = (int)(ownerPosition.X - (sourceRectangle.Width * scale.X - (
                    (facingRight) ? origin.X * scale.X : sourceRectangle.Width * scale.X - origin.X * scale.X)));
                int y1 = (int)(ownerPosition.Y - (sourceRectangle.Height - origin.Y) * scale.Y);
                int x2 = (int)(x1 + (sourceRectangle.Width * scale.X));
                int y2 = (int)(y1 + (sourceRectangle.Height * scale.Y));

                spriteCameraRectangle.X = x1;
                spriteCameraRectangle.Y = y1;
                spriteCameraRectangle.Width = x2 - x1;
                spriteCameraRectangle.Height = y2 - y1;

                drawParameters.Position = ownerPosition;
                drawParameters.Effects = (facingRight) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                drawParameters.Rotation = owner.Rotation;
                drawParameters.Origin = origin;
                drawParameters.Scale = owner.Scale;

                // Se aplican efectos en caso de que exista una lista de estos.
                if (owner.Effects != null)
                {
                    for (int i = 0; i < owner.Effects.Count; i++)
                    {
                        owner.Effects[i].applyEffect(ref drawParameters);
                    }
                }

                // Chequeo temporal mientras se disminuye la cantidad de 
                // begin/end para saber si la entidad a dibujar se encuentra
                // a la vista de la cámara
                drawParameters.Draw = Program.GAME.Camera.IsInView(spriteCameraRectangle);

                if (fadeFramesNumber > 0)
                {
                    if (this.currentFadeFrame <= 0)
                    {
                        for (int i = pastFrames.Length - 1; i >= 0; i--)
                        {
                            if (i == pastFrames.Length - 1)
                            {
                                pastFrames[i].Draw = false;
                            }
                            else
                            {
                                pastFrames[i + 1] = pastFrames[i];
                            }
                        }
                        pastFrames[0] = drawParameters;
                        this.currentFadeFrame = this.intervalFadeFrames;
                    }
                    else
                    {
                        this.currentFadeFrame -= 1;
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchWithMatrix();

            sb.Draw(drawParameters.Texture, drawParameters.Position, drawParameters.SourceRectangle,
                drawParameters.Color * drawParameters.Alpha, drawParameters.Rotation, drawParameters.Origin, drawParameters.Scale,
                drawParameters.Effects, drawParameters.LayerDepth);

            if (fadeFramesNumber > 0)
            {
                float alphaAmount = 1f / (fadeFramesNumber + 1);
                float depthAmount = 0.00001f;
                float currenAlpha = 1f;
                float currentDepth = drawParameters.LayerDepth;

                for (int i = 0; i < pastFrames.Length; i++)
                {
                    currenAlpha -= alphaAmount;
                    currentDepth += depthAmount;
                    if (pastFrames[i].Draw == false) continue;

                    sb.Draw(pastFrames[i].Texture, pastFrames[i].Position, pastFrames[i].SourceRectangle,
                        pastFrames[i].Color * (pastFrames[i].Alpha * currenAlpha), pastFrames[i].Rotation, pastFrames[i].Origin,
                        pastFrames[i].Scale, pastFrames[i].Effects, currentDepth);
                }
            }
        }

        public int FadeFramesNumber
        {
            get { return this.fadeFramesNumber; }
            set
            {
                if (value > 0)
                {
                    this.fadeFramesNumber = value;
                    if (this.pastFrames == null)
                    {
                        this.pastFrames = new DrawParameters[value];
                    }
                    else
                    {
                        if (this.pastFrames.Length != value)
                        {
                            this.pastFrames = new DrawParameters[value];
                        }
                        else
                        {
                            for (int i = 0; i < this.pastFrames.Length; i++)
                            {
                                this.pastFrames[i] = new DrawParameters();
                            }
                        }
                    }
                    this.currentFadeFrame = intervalFadeFrames;
                }
                else
                {
                    this.fadeFramesNumber = 0;
                    this.pastFrames = null;
                    this.currentFadeFrame = 0;
                }
            }
        }

        public bool Visible
        {
            get { return drawParameters.Draw; }
            set { drawParameters.Draw = value; }
        }

        public IEntity Owner
        {
            get { return this.owner; }
        }
    }
}
