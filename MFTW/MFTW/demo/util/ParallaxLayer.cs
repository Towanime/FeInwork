using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Managers;
using FeInwork.Core.Util;
using FeInwork.Core.Listeners;
using FeInwork.Core.Events;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.world;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.FeInwork.entities;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.core.managers;

namespace FeInwork.FeInwork.util
{
    /// <summary>
    /// Esta clase contiene un grupo de draw parameters que se usan para representar una textura en pantalla
    /// sin necesidad de crear entidades de por medio.
    /// Todos los items de este objeto se moveran a la misma velocidad definida.
    /// Se pueden usar multiples objetos y variar la velocidad para crear mejores efectos!
    /// </summary>
    public class ParallaxLayer : BaseComponent, IDrawUpdateableFE
    {
        /// <summary>
        /// Items a dibujar en la capa especifica.
        /// </summary>
        private List<DrawParameters> items;
        /// <summary>
        /// Velocidad con la que los items de esta capa se desplazan.
        /// </summary>
        //private float speed;
        /// <summary>
        /// Distancia minima que se debe mover la entidad para que se haga scroll.
        /// </summary>
        private float minimalDistance;
        /// <summary>
        /// Para especificar si el efecto se aplica en frente de la zona de juego o en el fondo.
        /// </summary>
        private bool drawOnFront;
        /// <summary>
        /// Si se dibuja o no.
        /// </summary>
        private bool isVisible;
        // vector2 para post
        private Vector2 oldPosition;
        private Vector2 newPosition;

        public ParallaxLayer(AreaEntity room, List<DrawParameters> items) : base(room)
        {
            this.items = items;
            initialize();
        }

        public ParallaxLayer(AreaEntity room, float minimalDistance, List<DrawParameters> items, bool drawOnFront)
            : base(room)
        {
            this.drawOnFront = drawOnFront;
            this.items = items;
            this.minimalDistance = minimalDistance;
            initialize();
        }

        public override void initialize()
        {
           // parallaxLayerRenderer = new ParallaxLayerRenderer(this);
            // para escuhar los cambios de posicion de la entidad de la camara!
           // EventManager.Instance.addPropertyListener(EntityProperty.Position, WorldManager.Instance.MainEntity, this);
            oldPosition = Vector2.Zero;
            isVisible = true;
            BasicComponentManager.Instance.addDrawableOnly(this);
        }
        
        private void moveItems(List<DrawParameters> items, Vector2 oldPosition, Vector2 newPosition)
        {
            for (int i = 0; i < items.Count; i++)
            {
                DrawParameters d = items[i];
                if (newPosition.X > oldPosition.X) // a la izq si se mueve a la der
                {
                    d.Position = new Vector2(d.Position.X - d.MoveSpeed, d.Position.Y);
                }
                else if (newPosition.X < oldPosition.X) // a la derecha si se mueve a la izq
                {
                    d.Position = new Vector2(d.Position.X + d.MoveSpeed, d.Position.Y);
                }

                // esto es solo si la imagen es loop!
                if (d.Loop)
                {
                    if (d.Position.X + (d.Scale.X * d.SourceRectangle.Width) < 0)
                    {
                        d.Position = new Vector2(0, d.Position.Y);
                    }
                }
                items[i] = d;
            }
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch sb = this.DrawOnFront ? SpriteBatchManager.Instance.getSpriteBatchBetween() :
                SpriteBatchManager.Instance.getSpriteBatchBackground();

            for (int i = 0; i < Items.Count; i++)
            {
                DrawParameters d = Items[i];
                if (d.Draw)
                {
                    // valida si es loop!
                    if (d.Loop)
                    {
                        // saca cuantas repeticiones de esta imagen se necesitan para cubrir la pantalla
                        int repeats = (int)Math.Ceiling((double)(Program.GAME.GraphicsDevice.Viewport.Width - (d.Scale.X * d.SourceRectangle.Width))
                            / (d.Scale.X * d.SourceRectangle.Width));
                        for (int j = 0; j <= repeats; j++)
                        {
                            if (j >= 1)
                            {
                                // saca esto extra por si queda alguna parte por rellenar en pantalla!
                                //if (((d.Scale.X * d.SourceRectangle.Width) - d.Position.X + ((d.Scale.X * d.SourceRectangle.Width) * j - repeats + 1) - Program.GAME.GraphicsDevice.Viewport.Width) < Program.GAME.GraphicsDevice.Viewport.Width)
                                if (Program.GAME.GraphicsDevice.Viewport.Width - ((d.Scale.X * d.SourceRectangle.Width) + d.Position.X) - ((d.Scale.X * d.SourceRectangle.Width) * j - repeats + 1) > 0)
                                {
                                    repeats++;
                                }
                                sb.Draw(d.Texture, new Vector2(d.Position.X + ((d.Scale.X * d.SourceRectangle.Width) * j) - 1, d.Position.Y), d.SourceRectangle, d.Color * d.Alpha, d.Rotation, d.Origin,
                                     d.Scale, d.Effects, d.LayerDepth);
                            }
                            else
                            {
                                sb.Draw(d.Texture, d.Position, d.SourceRectangle, d.Color * d.Alpha, d.Rotation, d.Origin,
                                    d.Scale, d.Effects, d.LayerDepth);
                            }
                        }
                    }
                    else
                    {
                        sb.Draw(d.Texture, d.Position, d.SourceRectangle, d.Color * d.Alpha, d.Rotation, d.Origin,
                            d.Scale, d.Effects, d.LayerDepth);
                    }
                }
            }
        }

        public void DrawUpdate(GameTime gameTime)
        {
            newPosition = Program.GAME.Camera.CollisionEntity.getVectorProperty(EntityProperty.Position);
            float moveX = Math.Abs((newPosition - oldPosition).X);
            if (moveX > minimalDistance)
            {
                moveItems(items, oldPosition, newPosition);
            }
            oldPosition = newPosition;
        }

        #region Properties
        /// <summary>
        /// Read Only, devuelve la lista de drawparameters de esta capa.
        /// </summary>
        public List<DrawParameters> Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// Especifica la distancia minima que la entidad principal debe moverse para
        /// poder hacer scroll en alguna direccion.
        /// </summary>
        public float MinimalDistance
        {
            get { return this.minimalDistance; }
            set { this.minimalDistance = value; }
        }

        /// <summary>
        /// Read only, especifica si se dibuja entre el hud y el area de juego o en el fondo.
        /// </summary>
        public bool DrawOnFront
        {
            get { return this.drawOnFront; }
        }

        public bool Visible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                this.isVisible = value;
            }
        }
        #endregion Properties

    }
}
