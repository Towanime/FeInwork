using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.entities;
using FeInwork.FeInwork.world;
using FeInwork.FeInwork.util;
using FeInwork.core.managers;

namespace FeInwork.FeInwork
{
    public class TestStage : IComponent, IDrawableFE
    {
        // aqui en vez de ponerla como IEntity la pongo como WorldEntity xq este componente es Unico para esa entidad
        AreaEntity owner;
        Texture2D background;
        Texture2D towers;
        Texture2D ground;
        Texture2D moon;
        Texture2D nube;
        Texture2D nube2;
        Texture2D nube3;
        Texture2D plant;
        Texture2D background2;
        Texture2D nubesVarias;
        Texture2D koala;
        private bool visible;

        public TestStage(AreaEntity owner)
        {
            this.owner = owner;
            initialize();
        }

        public void initialize()
        {
            // se registra al componente manager activo
            Program.GAME.ComponentManager.addDrawableOnly(this);
            // carga todos las imgs a los coñazos :)
            ground = Program.GAME.Content.Load<Texture2D>("teststage/ground");
            background = Program.GAME.Content.Load<Texture2D>("teststage/background");
            towers = Program.GAME.Content.Load<Texture2D>("teststage/towers");
            moon = Program.GAME.Content.Load<Texture2D>("teststage/moon");
            nube = Program.GAME.Content.Load<Texture2D>("teststage/nube");
            nube2 = Program.GAME.Content.Load<Texture2D>("teststage/nube2");
            nube3 = Program.GAME.Content.Load<Texture2D>("teststage/nube3");
            plant = Program.GAME.Content.Load<Texture2D>("teststage/plant");
            background2 = Program.GAME.Content.Load<Texture2D>("teststage/background_stuff");
            nubesVarias = Program.GAME.Content.Load<Texture2D>("teststage/nubes_varias");
            koala = Program.GAME.Content.Load<Texture2D>("teststage/Koala");
            visible = true;
        }

        public void DrawUpdate(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchWithMatrix();
            // begin dibujado  Program.GAME.camera.gettransformation(Program.GAME.GraphicsDevice)
            //sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Program.GAME.Camera.Transform);
            // aqui verifico con que color se tiene que dibujar... esto seria mejor cambiarlo con eventos
            Color color = Color.White;
            if (owner.IsTranceModeOn)
            {
                color = Color.BlueViolet;
            }
            // background
            sb.Draw(background, new Rectangle(0, 0, 1280, 700), null, color, 0f,
                Vector2.Zero, SpriteEffects.None, GameLayers.BACK_BACKGROUND);
            sb.Draw(background2, new Rectangle(0, 120, 1280, 600), null, color, 0f,
                Vector2.Zero, SpriteEffects.None, GameLayers.BACK_BACKGROUND);
            sb.Draw(ground, new Rectangle(0, 700, 1280, 68), color);
            sb.Draw(towers, new Rectangle(0, 300, 400, 400), null, color, 0f,
                Vector2.Zero, SpriteEffects.None, GameLayers.BACK_BACKGROUND);
            sb.Draw(moon, new Rectangle(480, 20, 400, 400), null, color, 0f,
                Vector2.Zero, SpriteEffects.None, GameLayers.BACK_BACKGROUND);
            sb.Draw(nubesVarias, new Rectangle(480, 120, 360, 140), null, color, 0f,
                Vector2.Zero, SpriteEffects.None, GameLayers.BACK_BACKGROUND);
            sb.Draw(nube, new Rectangle(120, 20, 270, 140), null, color, 0f,
                Vector2.Zero, SpriteEffects.None, GameLayers.BACK_BACKGROUND);

            sb.Draw(towers, new Rectangle(400, 300, 400, 400), null, color, 0f,
                Vector2.Zero, SpriteEffects.None, GameLayers.BACK_BACKGROUND);
            // trance mode!
            if (owner.IsTranceModeOn)
            {
                sb.Draw(koala, new Rectangle(0, 0, 1280, 768), null, color, 0f,
                    Vector2.Zero, SpriteEffects.None, GameLayers.BACK_BACKGROUND);
            }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public IEntity Owner
        {
            get { return owner; }
        }
    }
}