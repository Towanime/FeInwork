using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Util;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.components;
using FeInwork.FeInwork.util;
using FeInwork.core.managers;

namespace FeInwork.FeInwork.renderers
{
    public class HealthRenderer : BaseComponent, IDrawUpdateableFE
    {
        private bool isVisible;
        private HealthComponent healthComponent;
        private Texture2D textureHealthBar;
        private SpriteFont font;
        private float posX;
        private float posY;
        private bool isRelativeToPosition;
        // DrawingParameters para las barras y strings
        private DrawParameters greenBarParams;
        private DrawParameters grayBarParams;
        private DrawParameters blackBarParams;
        private DrawStringParameters HPLabelParams;
        private DrawStringParameters HPCurrentParams;

        public HealthRenderer(IEntity owner, HealthComponent healthComponent, int posX, int posY)
            : base(owner)
        {
            this.healthComponent = healthComponent;
            this.posX = posX;
            this.posY = posY;
            this.isRelativeToPosition = false;
            initialize();
        }

        public HealthRenderer(IEntity owner, HealthComponent healthComponent)
            : base(owner)
        {
            this.healthComponent = healthComponent;
            this.isRelativeToPosition = true;
            initialize();
        }

        public override void initialize()
        {
            textureHealthBar = Program.GAME.Content.Load<Texture2D>("teststage/generic_bar");
            font = Program.GAME.Content.Load<SpriteFont>("MenuFont");
            Program.GAME.ComponentManager.addComponent(this);
            this.isVisible = true;

            greenBarParams = new DrawParameters(textureHealthBar);
            greenBarParams.Color = Color.PaleGreen;
            greenBarParams.Rotation = 0;
            greenBarParams.Origin = Vector2.Zero;
            greenBarParams.Effects = SpriteEffects.None;
            greenBarParams.LayerDepth = GameLayers.FRONT_HUD_AREA;

            grayBarParams = new DrawParameters(textureHealthBar);
            grayBarParams.Color = Color.Gray;
            grayBarParams.Rotation = 0;
            grayBarParams.Origin = Vector2.Zero;
            grayBarParams.Effects = SpriteEffects.None;
            grayBarParams.LayerDepth = GameLayers.FRONT_HUD_AREA;
            grayBarParams.SourceRectangle = new Rectangle(0, 45, textureHealthBar.Width, 44);

            blackBarParams = new DrawParameters(textureHealthBar);
            blackBarParams.Color = Color.Black;
            blackBarParams.Rotation = 0;
            blackBarParams.Origin = Vector2.Zero;
            blackBarParams.Effects = SpriteEffects.None;
            blackBarParams.LayerDepth = GameLayers.FRONT_HUD_AREA;
            blackBarParams.SourceRectangle = new Rectangle(0, 0, textureHealthBar.Width, 44);

            HPLabelParams = new DrawStringParameters();
            HPLabelParams.SpriteFont = font;
            HPLabelParams.Text = "HP:";
            HPLabelParams.Color = Color.White;

            HPCurrentParams = new DrawStringParameters();
            HPCurrentParams.SpriteFont = font;
            HPCurrentParams.SbText = new StringBuilder();
            HPCurrentParams.Color = Color.White;
        }

        public void DrawUpdate(GameTime gameTime)
        {
            Vector2 scale = new Vector2(1, 1);
            float tempX = posX;
            float tempY = posY;

            if (isRelativeToPosition)
            {
                // Si se dibuja relativo a la posición entonces se dibuja una barra mas pequeña
                // para efectos de debug
                scale = new Vector2(0.3f, 0.6f);
                // La posición sera 20px por debajo del X y Y de la entidad
                tempX = owner.getVectorProperty(EntityProperty.Position).X - ((textureHealthBar.Width * scale.X) / 2);
                tempY = owner.getVectorProperty(EntityProperty.Position).Y + 20;
                HPCurrentParams.Position = new Vector2(tempX, tempY + 50 * scale.Y);
            }
            else
            {   // Se mueve el X de dibujado para dar espacio al string del HUD
                tempX += 80;
                // Una escala un poco mas pequeña para no ocupar todo el tamaño de la textura
                scale = new Vector2(0.5f, 0.8f);
                HPLabelParams.Position = new Vector2(tempX - 80, tempY);
                HPCurrentParams.Position = new Vector2(tempX, tempY + 50 * scale.Y);
            }

            HPCurrentParams.SbText.Remove(0, HPCurrentParams.SbText.Length);
            HPCurrentParams.SbText.Append(healthComponent.Current);
            HPCurrentParams.SbText.Append(" / ");
            HPCurrentParams.SbText.Append(healthComponent.Max);

            greenBarParams.Position = new Vector2(tempX, tempY);
            greenBarParams.SourceRectangle = new Rectangle(0, 45, (int)((textureHealthBar.Width) * ((float)healthComponent.Current / (float)healthComponent.Max)), 44);
            greenBarParams.Scale = scale;
            greenBarParams.LayerDepth = GameLayers.FRONT_HUD_AREA;

            grayBarParams.Position = new Vector2(tempX, tempY);
            grayBarParams.Scale = scale;
            grayBarParams.LayerDepth = GameLayers.MIDDLE_HUD_AREA;

            blackBarParams.Position = new Vector2(tempX, tempY);
            blackBarParams.Scale = scale;
            blackBarParams.LayerDepth = GameLayers.BACK_HUD_AREA;
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchHud();

            if (isRelativeToPosition)
            {
             //   sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Program.GAME.Camera.Transform);
                sb.DrawString(HPCurrentParams.SpriteFont, HPCurrentParams.SbText,
                    HPCurrentParams.Position, HPCurrentParams.Color, 0f, Vector2.Zero,
                    1, SpriteEffects.None,
                    GameLayers.TEXT_HUD_AREA);
             //   sb.End();
                // Se le agrega la cámara al dibujado
              //  sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Program.GAME.Camera.Transform);
            }
            else
            {
                // Se dibujan los strings de HP
              //  sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                sb.DrawString(HPLabelParams.SpriteFont, HPLabelParams.Text, HPLabelParams.Position, HPLabelParams.Color,
                    0f, Vector2.Zero,
                    1, SpriteEffects.None,
                    GameLayers.TEXT_HUD_AREA);
                sb.DrawString(HPCurrentParams.SpriteFont, HPCurrentParams.SbText, HPCurrentParams.Position, HPCurrentParams.Color,
                    0f, Vector2.Zero,
                    1, SpriteEffects.None,
                    GameLayers.TEXT_HUD_AREA);
               // sb.End();
                // Se dibuja de manera estatica sin la cámara
             //   sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            }

            // Fondo negro 
            sb.Draw(blackBarParams.Texture, blackBarParams.Position, blackBarParams.SourceRectangle,
                blackBarParams.Color, blackBarParams.Rotation, blackBarParams.Origin,
                blackBarParams.Scale, blackBarParams.Effects, blackBarParams.LayerDepth);

            // Fondo gris
            sb.Draw(grayBarParams.Texture, grayBarParams.Position, grayBarParams.SourceRectangle,
                grayBarParams.Color, grayBarParams.Rotation, grayBarParams.Origin,
                grayBarParams.Scale, grayBarParams.Effects, grayBarParams.LayerDepth);

            // Barra de vida (verde)
            sb.Draw(greenBarParams.Texture, greenBarParams.Position, greenBarParams.SourceRectangle,
                greenBarParams.Color, greenBarParams.Rotation, greenBarParams.Origin,
                greenBarParams.Scale, greenBarParams.Effects, greenBarParams.LayerDepth);
           // sb.End();
        }

        public bool Visible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

    }
}
