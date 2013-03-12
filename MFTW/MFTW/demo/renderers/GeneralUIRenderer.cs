using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.components;
using FeInwork.FeInwork.util;
using FeInwork.core.managers;

namespace FeInwork.FeInwork.renderers
{
    public class GeneralUIRenderer : IDrawUpdateableFE
    {
        private bool isVisible;
        private Texture2D buttons;
        private Rectangle greenBtnRectangle;
        private Rectangle blueBtnRectangle;
        private Rectangle yellowBtnRectangle;
        private Rectangle redBtnRectangle;
        private Vector2 btnsPos;

        public GeneralUIRenderer()
        {
            initialize();
        }

        private void initialize()
        {
            this.isVisible = true;
            Program.GAME.ComponentManager.addDrawableOnly(this);
            buttons = Program.GAME.Content.Load<Texture2D>("teststage/buttons_layout");
            greenBtnRectangle = new Rectangle(0, 0, 50, 50);
            blueBtnRectangle = new Rectangle(50, 0, 50, 50);
            yellowBtnRectangle = new Rectangle(100, 0, 50, 50);
            redBtnRectangle = new Rectangle(150, 0, 50, 50);
            this.btnsPos = Vector2.Zero;
            this.btnsPos.X = Program.GAME.ResolutionWidth - 200;
            this.btnsPos.Y = Program.GAME.ResolutionHeight - 200;
        }

        public void DrawUpdate(GameTime gameTime)
        {

        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchHud();

            sb.Draw(buttons, new Vector2(btnsPos.X + 50, btnsPos.Y + 100), greenBtnRectangle, Color.White,
                0f, Vector2.Zero, 1, SpriteEffects.None,
                GameLayers.BACK_HUD_AREA);
            sb.Draw(buttons, new Vector2(btnsPos.X, btnsPos.Y + 50), blueBtnRectangle, Color.White,
                0f, Vector2.Zero, 1, SpriteEffects.None,
                GameLayers.BACK_HUD_AREA);
            sb.Draw(buttons, new Vector2(btnsPos.X + 50, btnsPos.Y), yellowBtnRectangle, Color.White,
                0f, Vector2.Zero, 1, SpriteEffects.None,
                GameLayers.BACK_HUD_AREA);
            sb.Draw(buttons, new Vector2(btnsPos.X + 100, btnsPos.Y + 50), redBtnRectangle, Color.White,
                0f, Vector2.Zero, 1, SpriteEffects.None,
                GameLayers.BACK_HUD_AREA);
        }

        public bool Visible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
    }
}
