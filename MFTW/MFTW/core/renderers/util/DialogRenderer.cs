using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.core.interfaces;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using FeInwork.Core.Managers;
using FeInwork.core.util;
using FeInwork.FeInwork.util;
using FeInwork.core.managers;

namespace FeInwork.core.renderers.util
{
    public class DialogRenderer : IDrawableFE
    {
        private static DialogRenderer instance;
        /// <summary>
        /// Font que va a dibujar
        /// </summary>
        private SpriteFont font;
        /// <summary>
        /// Para basarse en cuanto de alto tendra una linea!
        /// </summary>
        private int fontHeight;
        /// <summary>
        /// Fondo de la caja de mensajes.
        /// </summary>
        private Texture2D blank;
        /// <summary>
        /// 
        /// </summary>
        private IDialogProvider currentProvider;
        /// <summary>
        /// Para saber si mostra ro no los botones junto a las cajas de dialogo.
        /// </summary>
        private bool isShowButtons = true;
        /// <summary>
        /// Rectangulo a dibujar detras del texto.
        /// </summary>
        private Rectangle dialogRectangle;
        /// <summary>
        /// Rectangulo para los botones.
        /// </summary>
        private Rectangle buttonRectangle = new Rectangle(0, 0, 80, 82);
        /// <summary>
        /// Guarda las texturas que se pueden estar utilizando en un dialogo para los avatars.
        /// </summary>
        private Dictionary<string, Texture2D> textures = new Dictionary<string,Texture2D>();

        private DialogRenderer()
        {
            initialize();
        }

        private void initialize()
        {
            font = Program.GAME.Content.Load<SpriteFont>("MenuFont");
            fontHeight = (int)font.MeasureString("Q").Y;
            blank = Program.GAME.Content.Load<Texture2D>("teststage/blank_pixel");
            //buttons = Program.GAME.Content.Load<Texture2D>("teststage/xboxControllerSpriteFont");
            BasicComponentManager.Instance.addDrawableOnly(this);
        }

        public void Draw(GameTime gameTime)
        {
            DialogParameters param = currentProvider.getCurrentDialogParameter();
            string text = DialogManager.Instance.TextToDraw.ToString();
            setupDialogBox(text, ref param);

            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchHud();
            // dibujar avatar si tiene
            /*if (param.CustomAssetName != null)
            {
                Texture2D texture = getTextureForParameter(param);
                sb.Draw(texture,
                    new Rectangle(boxRectangle.X,
                        boxRectangle.Y - (int)param.AvatarPixelsToShow,
                        texture.Width, texture.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, GameLayers.MIDDLE_HUD_AREA);
            }*/
            sb.Draw(blank, dialogRectangle, null, Color.Black * .6f, 0, 
                Vector2.Zero, SpriteEffects.None, GameLayers.MIDDLE_HUD_AREA);
            sb.DrawString(font, text,
                new Vector2(
                    dialogRectangle.X + param.HorizontalMargin,
                    dialogRectangle.Y + param.VerticalMargin
                    ),
                    Color.White, 0f, Vector2.Zero, param.Scale,
                    SpriteEffects.None, GameLayers.FRONT_HUD_AREA);
            if (isShowButtons)
            {
                // botones 
                // A
            }
        }

        private void setupDialogBox(string text, ref DialogParameters param)
        {
            // para saber cuanto tomara el texto
            Vector2 size = font.MeasureString(text);
            Rectangle safeArea = currentProvider.SafeArea;

            dialogRectangle.X = safeArea.X;
            dialogRectangle.Y = safeArea.Y;

            // ahora a ver si el cuadro de texto es grande o se adapta al texto
            if (param.IsAdaptToText)
            {
                // margen por 2 para que se aplique de los dos lados
                dialogRectangle.Width = (int)size.X + (param.HorizontalMargin * 2);
                dialogRectangle.Height = (int)size.Y + (param.VerticalMargin * 2);
            }
            else
            {
                dialogRectangle.Width = safeArea.Width;
                dialogRectangle.Height = (fontHeight * param.MaxLines) + (param.VerticalMargin * 2);
            }
        }

        #region Properties
        public bool Visible
        {
            get
            {
                if (currentProvider == null)
                {
                    return false;
                }
                else
                {
                    return currentProvider.Enabled;
                }
            }

            set { }
        }

        public IDialogProvider CurrentDialogProvider
        {
            set
            {
                this.currentProvider = value;
                //si esto es null then eliminar texturas guardadas
                if (value == null)
                {
                    textures.Clear();
                }
            }

            get
            {
                return this.currentProvider;
            }
        }

        public bool IsShowButtons
        {
            set
            {
                this.isShowButtons = value;
            }

            get
            {
                return this.isShowButtons;
            }
        }
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }
        #endregion

        // instancia 
        public static DialogRenderer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DialogRenderer();
                }
                return instance;
            }
        }
    }
}
