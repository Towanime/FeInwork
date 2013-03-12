using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.core.collision.bodies;
using FeInwork.FeInwork.components;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.components.interfaces;
using FeInwork.core.collision;

namespace FeInwork.core.Base
{
    /// <summary>
    /// Clase camara, esta deberia ser static?, solo habra una camara en todo el juego.
    /// Esta misma clase maneja el input de la camara es decir los botones con la cual se 
    /// movera, hara zoom in y out.
    /// TO DO: falta poneerle limites a la camara, es decir que no se mueva si llega a cierta x o y
    /// donde posiblemente no hayan sprites! y se vea el fondo azul feo.
    /// </summary>
    public class Camera2D : ICamera2D, PositionChangedListener
    {
        /// <summary>
        /// Zoom de los elementos en pantalla.
        /// Esta variable es el valor actual del zoom que se utiliza en la formula de 
        /// calculo para la matriz.
        /// El minimo de este valor es 0.1f!
        /// </summary>
        private float zoom;
        /// <summary>
        /// Este es el valor limite del zoom cuando se llama al metodo zoomOut.
        /// Se usa para actualizar el zoom hasta que llegue a este target.
        /// Si el metodo zoomout es llamado varias veces entonces el zoomTarget se incrementa hasta un 
        /// maximo permitido.
        /// </summary>
        private float zoomTarget;
        /// <summary>
        /// La velocidad con la que se realiza el zoom.
        /// </summary>
        private float zoomSpeed;
        /// <summary>
        /// Variable importante que dicta el maximo zoom in que puede tener la camara.
        /// </summary>
        private float zoomInLimit;
        /// <summary>
        /// Variable importante que especifica un limite para realizar el zoomOut.
        /// </summary>
        private float zoomOutLimit;
        /// <summary>
        /// Toda la magia va aca! es una matriz que ayuda a mover todos los elementos que se dibujen, 
        /// esto causa el efecto deseado.
        /// </summary>
        private Matrix transform;
        /// <summary>
        /// La cordenada en Y que se quiere alcanzar.
        /// </summary>
        private float yTarget;
        /// <summary>
        /// Coordenada en X que se quiere alcanzar.
        /// </summary>
        private float xTarget;
        /// <summary>
        /// Por si se quiere rotar la camara.
        /// </summary>
        private float rotation;
        /// <summary>
        /// Ancho del viewport para el juego
        /// </summary>
        private float viewportWidth;
        /// <summary>
        /// Alto del viewport para el juego
        /// </summary>
        private float viewportHeight;
        /// <summary>
        /// Velocidad en la que la camara se movera al punto que tiene como foco!
        /// </summary>
        private float moveSpeed;
        /// <summary>
        /// Verifica si la camara esta habilitada.
        /// </summary>
        private bool isEnabled;
        /// <summary>
        /// Target o Focus que tendra la camara, solo puede existir uno a la vez.
        /// </summary>
        private IFocusable focus;
        //Entidad para manejar limites dela camara en relacion con el mundo
        private IEntity collisionEntity;
        private Box cameraCollisionBody;
        private int initCollisionWidth;
        private int initCollisionHeight;
        /// <summary>
        /// Limites de camara
        /// </summary>
        private float? limitX1;
        private float? limitX2;
        private float? limitY1;
        private float? limitY2;

        /// <summary>
        /// Rectangulo que contiene la posición, ancho y alto
        /// de la cámara. Usado para chequeos rapidos de intersección
        /// con otros rectángulos.
        /// </summary>
        private Rectangle cameraViewRectangle;

        /// <summary>
        /// Crea la camara para el juego, recibe de una vez el foco.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="focus"></param>
        public Camera2D(Game game, IFocusable focus)
        {
            this.focus = focus;
            this.viewportHeight = game.GraphicsDevice.Viewport.Height;
            this.viewportWidth = game.GraphicsDevice.Viewport.Width;
            this.zoom = 1.0f;
            this.zoomInLimit = 1.6f;
            this.zoomOutLimit = 0.7f;
            this.zoomSpeed = 1.0f;
            this.zoomTarget = 1.0f;
            this.rotation = 0.0f;
            this.cameraViewRectangle = new Rectangle(0, 0, (int)viewportWidth, (int)viewportHeight);
            this.moveSpeed = 2f;
            this.isEnabled = true;
            this.initCollisionWidth = 250;
            this.initCollisionHeight = 250;
            initialize();
        }

        private void initialize()
        {
            Program.GAME.ComponentManager.addUpdateableOnly(this);
            // crea la entidad 
            this.collisionEntity = new Entity(GlobalIDs.CAMERA_ENTITY_ID);
            // crea body de colision 
            // para que quede justo en el centro de focus 
            float x = (viewportWidth / 2) - (initCollisionWidth / 2);
            float y = (viewportHeight / 2) - (initCollisionHeight / 2);
            this.cameraCollisionBody = ShapeFactory.CreateRectangle(GlobalIDs.CAMERA_COLLISION_BODY_ID, collisionEntity, true, false/*, viewportWidth - 80, viewportHeight - 80, new Vector2(40, 40)*/, viewportWidth, viewportHeight, Vector2.Zero, false, GameLayers.FRONT_HUD_AREA);
            // componente de colision basico
            this.collisionEntity.addComponent(new AnimatedCollisionComponent(collisionEntity, this.cameraCollisionBody));
            // asigna x y y
            this.collisionEntity.addVectorProperty(EntityProperty.Position, new Vector2(x, y));
            // registrar listener
            EventManager.Instance.addListener(EventType.POSITION_CHANGED_EVENT, collisionEntity, this);
        }

        public void Update(GameTime gameTime)
        {
            transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 0)) *
                        Matrix.CreateTranslation(new Vector3(viewportWidth * 0.5f, viewportHeight * 0.5f, 0));

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float newX = (Focus.Position.X - Position.X) * MoveSpeed * delta;
            float newY = (Focus.Position.Y - Position.Y) * MoveSpeed * delta;
            this.move(new Vector2(newX, newY));

            if (zoom < zoomTarget)
            {
                Zoom += delta * zoomSpeed;
            }
            if (zoom > zoomTarget)
            {
                Zoom -= delta * zoomSpeed;
            }

            // Se obtienen las dimensiones actuales de la camara segun el zoom
            float actualCameraWidth = viewportWidth / Zoom;
            float actualCameraHeight = viewportHeight / Zoom;

            // Actualiza la posición y dimensiones del rectangulo de cámara
            cameraViewRectangle.X = (int)(this.Position.X - actualCameraWidth / 2);
            cameraViewRectangle.Y = (int)(this.Position.Y - actualCameraHeight / 2);
            cameraViewRectangle.Width = (int)actualCameraWidth;
            cameraViewRectangle.Height = (int)actualCameraHeight;
        }

        public bool IsInView(Rectangle rectangle)
        {
            return cameraViewRectangle.Intersects(rectangle);
        }

        public bool IsInView(Vector2 position, Texture2D texture)
        {
            return cameraViewRectangle.Intersects(
                new Rectangle((int)(position.X - texture.Width / 2), (int)(position.Y - texture.Height / 2), (int)texture.Width, (int)texture.Height));
        }

        /// <summary>
        /// Agrega los limites de camara a partir de las coordenadas
        /// de la esquina superior izquierda y la esquina inferior derecha
        /// </summary>
        /// <param name="x1">limite izquierdo en x</param>
        /// <param name="y1">limite superior en y</param>
        /// <param name="x2">limite derecho en x</param>
        /// <param name="y2">limite inferior en y</param>
        public void setLimits(float x1, float y1, float x2, float y2)
        {
            this.limitX1 = x1;
            this.limitY1 = y1;
            this.limitX2 = (x2 >= (x1 + this.viewportWidth)) ? x2 : x1 + this.viewportWidth;
            this.limitY2 = (y2 >= (y1 + this.viewportHeight)) ? y2 : y1 + this.viewportHeight;
        }

        /// <summary>
        /// Elimina los limtes de camara
        /// </summary>
        public void resetLimits()
        {
            this.limitX1 = null;
            this.limitY1 = null;
            this.limitX2 = null;
            this.limitY2 = null;
        }

        /// <summary>
        /// Propiedad para mover la camara usando un vector.
        /// Solo se recomienda usar este cuando se intente mover la camara
        /// en x y y al mismo tiempo, como moverla en diagonal por ejemplo.
        /// </summary>
        /// <param name="distance"></param>
        public void move(Vector2 distance)
        {
            EventManager.Instance.fireEvent(PositionChangeRequestEvent.Create(collisionEntity, Position, this.adjustDistance(distance)));
        }

        /// <summary>
        /// Mueve la camara en x.
        /// </summary>
        /// <param name="distance"></param>
        public void moveX(float distance)
        {
            this.move(new Vector2(this.adjustXDistance(distance), 0));
        }

        /// <summary>
        /// Metodo para mover la camara en y.
        /// </summary>
        /// <param name="distance"></param>
        public void moveY(float distance)
        {
            this.move(new Vector2(0, this.adjustYDistance(distance)));            

            //position.Y += distance;
            /*if (distance < 0 && Position.Y - (viewportHeight / 2) + distance > 0)
            {
                yTarget += distance;
            }
            if (distance > 0 && Position.Y + (viewportHeight / 2) + distance < 750)
            {
                yTarget += distance;
            }*/
            // primero top
            //if (distance > 0 && position.Y + distance + (viewportHeight / 2) < 720)
            //{
            //    position.Y += distance;
            // //   position.Y += MathHelper.Clamp(position.Y + distance - (viewportHeight / 2), viewportHeight / 2, 0);
            //}
            // if (position.Y + distance - (viewportHeight / 2) > -10 || position.Y + distance + (viewportHeight / 2) < 720)
            // {
            // position.Y += distance;
            // }
        }

        private float adjustXDistance(float distance)
        {
            // verifica que no se pase del limite superior
            if (limitX1 != null && cameraCollisionBody.X + distance < limitX1)
            {
                distance = (float)limitX1 - cameraCollisionBody.X;
            }
            // si no entonces que no se pase del limite inferior
            else if (limitX2 != null && cameraCollisionBody.X + cameraCollisionBody.Width + distance > limitX2)
            {
                distance = -(cameraCollisionBody.X + cameraCollisionBody.Width - (float)limitX2);
            }
            return distance;
        }

        private float adjustYDistance(float distance)
        {
            // verifica que no se pase del limite superior
            if (limitY1 != null && cameraCollisionBody.Y + distance < limitY1)
            {
                distance = (float)limitY1 - cameraCollisionBody.Y;
            }
            // si no entonces que no se pase del limite inferior
            else if (limitY2 != null && cameraCollisionBody.Y + cameraCollisionBody.Height + distance > limitY2)
            {
                distance = -(cameraCollisionBody.Y + cameraCollisionBody.Height - (float)limitY2);
            }
            return distance;
        }

        private Vector2 adjustDistance(Vector2 distance)
        {
            return new Vector2(this.adjustXDistance(distance.X), this.adjustYDistance(distance.Y));
        }

        public void resetCameraPosition()
        {
            this.cameraCollisionBody.OffsetRelativeTo(this.Position, focus.Position);
        }

        public void zoomOut()
        {
            if (zoomTarget - 0.1f >= zoomOutLimit)
            {
                zoomTarget -= 0.1f;
            }
        }

        public void zoomIn()
        {
            if (zoomTarget + 0.1f <= zoomInLimit)
            {
                zoomTarget += 0.1f;
            }
        }

        public void resetZoom()
        {
            zoomTarget = 1.0f;
        }

        #region Properties
        /// <summary>
        /// NO USAR ESTA PROPIEDAD DIRECTAMENTE! 
        /// MEJOR USAR LOS METODOS ZOOMOUT y ZOOMIN QUE CONTROLAN LOS LIMITES
        /// AUTOMATICAMENTE.
        /// </summary>
        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                if (zoom < zoomOutLimit) zoom = zoomOutLimit;
                if (zoom > zoomInLimit) zoom = zoomInLimit;
            } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 Position
        {
            get
            {
                return cameraCollisionBody.Center;
            }
            set
            {
                this.cameraCollisionBody.OffsetRelativeTo(this.Position, value);
            }
        }

        public float MoveSpeed
        {
            get
            {
                return moveSpeed;
            }
            set
            {
                moveSpeed = value;
            }
        }

        public Matrix Transform
        {
            get { return transform; }
        }

        public IFocusable Focus
        {
            get
            {
                return focus;
            }
            set
            {
                focus = value;
            }
        }

        public IEntity CollisionEntity
        {
            get
            {
                return this.collisionEntity;
            }
        }

        public bool Enabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        #endregion


        public void invoke(PositionChangedEvent eventObject)
        {
            collisionEntity.changeVectorProperty(EntityProperty.Position, Position, false);
        }
    }
}
