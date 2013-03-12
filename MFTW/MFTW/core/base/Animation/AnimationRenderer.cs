using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Util;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.renderers.animation;
using FeInwork.FeInwork.listeners;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.world;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.entities;
using FeInwork.core.managers;
using FeInwork.FeInwork.events;
using FeInwork;

namespace FeInwork.Core.Base.Animation
{
    public class AnimationRenderer : IComponent, IDrawUpdateableFE, ActionChangeListener
    {
        private DrawableEntity owner;
        // nombre del resource a cargar en este caso grafica
        private string asset;
        private Texture2D texture;
        private AnimationManagerComponent animationManager;
        private DrawParameters drawParameters;
        private DrawParameters[] pastFrames;
        private int fadeFramesNumber;
        private int intervalFadeFrames;
        private int currentFadeFrame;
        private Rectangle spriteCameraRectangle;

        public AnimationRenderer(DrawableEntity owner, string asset, AnimationManagerComponent animationManager)
        {
            this.owner = owner;
            this.asset = asset;
            this.animationManager = animationManager;
            initialize();
        }

        public AnimationRenderer(DrawableEntity owner, string asset, AnimationManagerComponent animationManager, int fadeFramesNumber, int intervalFadeFrames)
        {
            this.owner = owner;
            this.asset = asset;
            this.animationManager = animationManager;
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
            // se registra al componente manager activo!
            // asi no se debe hacer... pero solo por ejemplo
            Program.GAME.ComponentManager.addComponent(this);
            // registra las propiedades obligatorias para que este comp funcione
            owner.addState(EntityState.Visible, true);
            owner.addVectorProperty(EntityProperty.Position, Vector2.Zero);
            owner.addState(EntityState.FacingRight, true);
            texture = Program.GAME.Content.Load<Texture2D>(asset);
            drawParameters = new DrawParameters(texture);
            EventManager.Instance.addListener(EventType.ACTION_CHANGE_EVENT, this);
        }

        public void DrawUpdate(GameTime gameTime)
        {
            if ((drawParameters.Draw = owner.getState(EntityState.Visible)) == true)
            {
                AnimationFrame currentFrame = animationManager.getCurrentAnimationFrame();
                Vector2 scale = currentFrame.scale;
                Rectangle sourceRectangle = currentFrame.rectangle;

                bool facingRight = owner.getState(EntityState.FacingRight);
                int orientation = facingRight == false ? -1 : 1;

                Vector2 ownerPosition = owner.getVectorProperty(EntityProperty.Position);

                Vector2 origin = drawParameters.Origin;
                origin.X = sourceRectangle.Width / 2 + currentFrame.offsetx * orientation;
                origin.Y = sourceRectangle.Height / 2 + currentFrame.offsety;

                int x1 = (int)(ownerPosition.X - (sourceRectangle.Width * scale.X - (
                    (facingRight) ? origin.X * scale.X : sourceRectangle.Width * scale.X - origin.X * scale.X)));
                int y1 = (int)(ownerPosition.Y - (sourceRectangle.Height - origin.Y) * scale.Y);
                int x2 = (int)(x1 + (sourceRectangle.Width * scale.X));
                int y2 = (int)(y1 + (sourceRectangle.Height * scale.Y));

                spriteCameraRectangle.X = x1;
                spriteCameraRectangle.Y = y1;
                spriteCameraRectangle.Width = x2 - x1;
                spriteCameraRectangle.Height = y2 - y1;

                drawParameters.Texture = texture;
                drawParameters.Position = ownerPosition;
                drawParameters.SourceRectangle = sourceRectangle;
                drawParameters.Color = Color.White;
                drawParameters.Rotation = owner.getFloatProperty(EntityProperty.Rotation);
                drawParameters.Origin = origin;
                drawParameters.Scale = scale;
                drawParameters.Effects = facingRight == false ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                drawParameters.LayerDepth = GameLayers.MIDDLE_PLAY_AREA;

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
                drawParameters.Color * drawParameters.Alpha, drawParameters.Rotation, drawParameters.Origin,
                drawParameters.Scale, drawParameters.Effects, drawParameters.LayerDepth);

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

        public void invoke(ActionChangeEvent eventObject)
        {
            if (eventObject.Origin.Equals(this.owner))
            {
                animationManager.setAction(eventObject.Action, eventObject.doRepeat);
            }
        }

        public IEntity Owner
        {
            get { return this.owner; }
        }
    }
}
