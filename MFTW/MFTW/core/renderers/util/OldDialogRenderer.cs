using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using AlchemistDemo.alchemist.util;
using FeInwork.Core.Managers;
using Microsoft.Xna.Framework.Graphics;
using AlchemistDemo.core.interfaces;
using AlchemistDemo.core.util;
using AlchemistDemo.core.managers;

namespace AlchemistDemo.core.renderers.util
{
    public class DDialogRenderer : IDrawableFE
    {
        private static DDialogRenderer instance;
        /// <summary>
        /// Font que va a dibujar
        /// </summary>
        private SpriteFont font;
        /// <summary>
        /// Rectangulo donde se pueden dibujar las ventanas de texto.
        /// </summary>
        private Rectangle safeArea;
        /// <summary>
        /// Fondo de la caja de mensajes.
        /// </summary>
        private Texture2D blank;
        /// <summary>
        /// Textura de botones a mostrar en el cuadro de texto.
        /// </summary>
        private Texture2D buttons;
        /// <summary>
        /// 
        /// </summary>
        private IDialogProvider currentProvider;
        /// <summary>
        /// Para saber si mostra ro no los botones junto a las cajas de dialogo.
        /// </summary>
        private bool isShowButtons = true;
        /// <summary>
        /// Guarda las texturas que se pueden estar utilizando en un dialogo para los avatars.
        /// </summary>
        private Dictionary<string, Texture2D> textures;

        private DDialogRenderer()
        {
            initialize();
        }

        private void initialize()
        {
            textures = new Dictionary<string, Texture2D>();
            safeArea = new Rectangle(25, 0, (int)Program.GAME.ResolutionWidth - 50, (int)Program.GAME.ResolutionHeight - 50);
            font = Program.GAME.Content.Load<SpriteFont>("MenuFont");
            blank = Program.GAME.Content.Load<Texture2D>("teststage/blank_pixel");
            buttons = Program.GAME.Content.Load<Texture2D>("teststage/xboxControllerSpriteFont");
            BasicComponentManager.Instance.addDrawableOnly(this);
        }

        public void Draw(GameTime gameTime)
        {
            DialogParameters param = currentProvider.getCurrentDialogParameter();
            Rectangle boxRectangle = setupDialogBox(ref param);
            Rectangle buttonRectangle = new Rectangle(0, 0, 80, 82);

            SpriteBatch sb = SpriteBatchManager.Instance.getSpriteBatchHud();
            // dibujar avatar si tiene
            if(param.CustomAssetName != null){
                Texture2D texture = getTextureForParameter(param);
                sb.Draw(texture, 
                    new Rectangle(boxRectangle.X,
                        boxRectangle.Y - (int) param.AvatarPixelsToShow,
                        texture.Width, texture.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, GameConstants.MIDDLE_HUD_AREA);
            }
            sb.Draw(blank, boxRectangle, null, Color.Black * .6f, 0, Vector2.Zero, SpriteEffects.None, GameConstants.MIDDLE_HUD_AREA);
            /*sb.DrawString(font, param.Text,
                new Vector2(
                    boxRectangle.X + param.HorizontalMargin,
                    boxRectangle.Y + param.VerticalMargin
                    ),
                    Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, GameConstants.FRONT_HUD_AREA);
            */
            if (isShowButtons)
            {
                // botones 
                // A
                buttonRectangle.X = 875;
                buttonRectangle.Y = 52;
                sb.Draw(buttons, new Vector2(
                        boxRectangle.Right - 20,
                        boxRectangle.Bottom - 25
                        ), buttonRectangle, Color.White,
                        0f, Vector2.Zero, 0.4f, SpriteEffects.None, GameConstants.FRONT_HUD_AREA);

                //Y 
                buttonRectangle.X = 958;
                sb.Draw(buttons, new Vector2(
                        boxRectangle.Left - 15,
                        boxRectangle.Bottom - 25
                        ), buttonRectangle, Color.White,
                        0f, Vector2.Zero, 0.4f, SpriteEffects.None, GameConstants.FRONT_HUD_AREA);
            }
        }

        private Rectangle setupDialogBox(ref DialogParameters param)
        {
            Rectangle rect = new Rectangle();
            // para saber cuanto tomara el texto
            Vector2 size = font.MeasureString("");

            // primero a resolver el horizontal align
            if (param.HorizontalAlign == GameConstants.HorizontalAlign.Left)
            {
                rect.X = safeArea.X;
            }
            else if (param.HorizontalAlign == GameConstants.HorizontalAlign.Right)
            {
                rect.X = safeArea.Width - (int)size.X;
            }
            else if (param.HorizontalAlign == GameConstants.HorizontalAlign.HorizontalCenter)
            {
                rect.X = (safeArea.Width / 2) - (int)(size.X / 2);
            }
            else
            {
                // else es free!
                if (param.FreeX != 0)
                {
                    // no valida safe area ni nada
                    rect.X = (int)param.FreeX;
                }
                else
                {
                    // si es cero entonces se toma posicion defecto
                    rect.X = safeArea.X;
                }
            }

            //vertical values now 
            if (param.VerticalAlign == GameConstants.VerticalAlign.Top)
            {
                rect.Y = safeArea.Y;
                // verifica si hay que ajustar Y al posible avatar if any
                if (param.CustomAssetName != null)
                {
                    // sacar textura
                    Texture2D textura = getTextureForParameter(param);
                    if (param.AvatarPixelsToShow != 0)
                    {
                        rect.Y += (int)param.AvatarPixelsToShow;
                    }
                    else
                    {
                        // se mostrara la mitad then
                        rect.Y += textura.Height / 2;
                        param.AvatarPixelsToShow = textura.Height / 2;
                    }
                }
            }
            else if (param.VerticalAlign == GameConstants.VerticalAlign.Bottom)
            {
                rect.Y = safeArea.Height - (int)size.Y;
            }
            else if (param.VerticalAlign == GameConstants.VerticalAlign.VerticalCenter)
            {
                rect.Y = (safeArea.Height / 2) - (int)(size.Y / 2);
                // verifica si hay que ajustar Y al posible avatar if any
                if(param.CustomAssetName != null){
                    // sacar textura
                    Texture2D textura = getTextureForParameter(param);
                    if (param.AvatarPixelsToShow != 0)
                    {
                        rect.Y += (int)param.AvatarPixelsToShow;
                    }
                    else
                    {
                        // se mostrara la mitad then
                        rect.Y += textura.Height / 2;
                        // seta de una vez lo que mostrara 
                        param.AvatarPixelsToShow = textura.Height / 2;
                    }
                }
            }
            else
            {
                // else es free!
                if (param.FreeY != 0)
                {
                    // no valida safe area ni nada
                    rect.Y = (int)param.FreeY;
                }
                else
                {
                    // si es cero entonces se toma posicion defecto
                    rect.Y = safeArea.Height - (int)size.Y;
                }
            }

            // ahora a ver si el cuadro de texto es grande o se adapta al texto
            if (param.IsAdaptToText)
            {
                // margen por 2 para que se aplique de los dos lados
                rect.Width = (int)size.X + (param.HorizontalMargin * 2);
                rect.Height = (int)size.Y + (param.VerticalMargin * 2);
            }
            else
            {
                rect.Width = safeArea.Width;
                rect.Height = (int)size.Y + (param.VerticalMargin * 2);
            }
            return rect;
        }

        private Texture2D getTextureForParameter(DialogParameters param)
        {
            Texture2D texture = null;
            // si no la encuentra la carga
            if (!textures.TryGetValue(param.CustomAssetName, out texture))
            {
                texture = Program.GAME.Content.Load<Texture2D>(param.CustomAssetName);
                textures.Add(param.CustomAssetName, texture);
            }
            // regresa textura
            return texture;
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
            set
            {
            }
        }

        public Rectangle SafeArea
        {
            set { this.safeArea = value; }
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
        #endregion

        // instancia 
        public static DDialogRenderer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DDialogRenderer();
                }
                return instance;
            }
        }
    }
}
