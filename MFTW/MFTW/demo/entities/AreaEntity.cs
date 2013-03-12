using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using FeInwork.FeInwork.components;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.world;
using FeInwork.core.managers;
using FeInwork.FeInwork.util;
using FeInwork.core.collision.bodies;
using Microsoft.Xna.Framework;
using FeInwork.core.collision;
using FeInwork.core.util;
using FeInwork.FeInwork.listeners;

namespace FeInwork.FeInwork.entities
{
    public class AreaEntity : Entity, DeadListener
    {
        /// <summary>
        /// El ancho del cuarto medido en bloques
        /// </summary>
        protected int widthBlocks;
        /// <summary>
        /// El alto del cuarto medido en bloques
        /// </summary>
        protected int heightBlocks;
        /// <summary>
        /// Dirección desde donde cargará el contenido
        /// </summary>
        protected string contentURL;
        /// <summary>
        /// IEntities que se encuentran dentro de este Area que seran
        /// inicializadas al momento de cargar el cuarto y liberadas
        /// al momento de regresar al cuarto a su estado original
        /// </summary>
        protected FEList<IEntity> areaItems = new FEList<IEntity>();

        protected AreaCollisionComponent collisionComponent;

        /// <summary>
        /// Obtiene el ancho del cuarto medido en bloques
        /// </summary>
        public int WidthBlocks
        {
            get { return this.widthBlocks; }
        }

        /// <summary>
        /// Obtiene el alto del cuarto medido en bloques
        /// </summary>
        public int HeightBlocks
        {
            get { return this.heightBlocks; }
        }

        /// <summary>
        /// Crea una AreaEntity con una cantidad de bloques de alto y ancho fijos.
        /// </summary>
        /// <param name="widthBlocks">El ancho del cuarto medido en bloques</param>
        /// <param name="heightBlocks">El alto del cuarto medido en bloques</param>
        /// <param name="roomId">Id del cuarto</param>
        /// <param name="content">Direccion de donde se debe realizar la lectura del contenido del mismo</param>
        public AreaEntity(int widthBlocks, int heightBlocks, string roomId, string contentURL)
            : base(roomId)
        {
            this.widthBlocks = widthBlocks;
            this.heightBlocks = heightBlocks;
            this.contentURL = contentURL;

            this.initialize();
        }

        public override void initialize()
        {
            // este del padre crea el property body asi que es importante llamarlo primero.
            base.initialize();
            // se agrega a si mismo al contenedor de cuartos del WorldManager
            WorldManager.Instance.addArea(this);
            // se asignas unas propiedades extras
            addState(EntityState.TranceMode, false);
        }

        /// <summary>
        /// Carga el contenido del cuarto
        /// </summary>
        public virtual void LoadContent()
        {
            collisionComponent = new AreaCollisionComponent(this);

            int blockSize = GameConstants.BLOCK_SIZE;
            int roomOuterBorder = GameConstants.ROOM_OUTER_BORDER;

            IEntity areaLimits = new Entity("roomLimits");
            List<CollisionBody> limits = new List<CollisionBody>();
            Rectangle limitsRectangle = new Rectangle();
            this.areaItems.Add(areaLimits);

            // upper play borders
            limitsRectangle.X = -roomOuterBorder;
            limitsRectangle.Y = -roomOuterBorder;
            limitsRectangle.Width = (this.WidthBlocks * blockSize) + roomOuterBorder;
            limitsRectangle.Height = roomOuterBorder;
            limits.Add(ShapeFactory.CreateRectangle("upperLimitBack", areaLimits, true, false, 
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.BACK_PLAY_AREA));
            limits.Add(ShapeFactory.CreateRectangle("upperLimitMiddle", areaLimits, true, false, 
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.MIDDLE_PLAY_AREA));
            limits.Add(ShapeFactory.CreateRectangle("upperLimitFront", areaLimits, true, false, 
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.FRONT_PLAY_AREA));

            // right play borders
            limitsRectangle.X = this.WidthBlocks * blockSize;
            limitsRectangle.Y = -roomOuterBorder;
            limitsRectangle.Width = roomOuterBorder;
            limitsRectangle.Height = (this.heightBlocks * blockSize) + roomOuterBorder;
            limits.Add(ShapeFactory.CreateRectangle("rightLimitBack", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.BACK_PLAY_AREA));
            limits.Add(ShapeFactory.CreateRectangle("rightLimitMiddle", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.MIDDLE_PLAY_AREA));
            limits.Add(ShapeFactory.CreateRectangle("rightLimitFront", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.FRONT_PLAY_AREA));

            // lower play borders
            limitsRectangle.X = 0;
            limitsRectangle.Y = this.HeightBlocks * blockSize;
            limitsRectangle.Width = (this.WidthBlocks * blockSize) + roomOuterBorder;
            limitsRectangle.Height = roomOuterBorder;
            limits.Add(ShapeFactory.CreateRectangle("lowerLimitBack", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.BACK_PLAY_AREA));
            limits.Add(ShapeFactory.CreateRectangle("lowerLimitMiddle", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.MIDDLE_PLAY_AREA));
            limits.Add(ShapeFactory.CreateRectangle("lowerLimitFront", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.FRONT_PLAY_AREA));

            // left play borders
            limitsRectangle.X = -roomOuterBorder;
            limitsRectangle.Y = 0;
            limitsRectangle.Width = roomOuterBorder;
            limitsRectangle.Height = (this.heightBlocks * blockSize) + roomOuterBorder;
            limits.Add(ShapeFactory.CreateRectangle("leftLimitBack", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.BACK_PLAY_AREA));
            limits.Add(ShapeFactory.CreateRectangle("leftLimitMiddle", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.MIDDLE_PLAY_AREA));
            limits.Add(ShapeFactory.CreateRectangle("leftLimitFront", areaLimits, true, false,
                limitsRectangle.Width, limitsRectangle.Height, new Vector2(limitsRectangle.X, limitsRectangle.Y), false, GameLayers.FRONT_PLAY_AREA));

            areaLimits.addComponent(new StaticCollisionComponent(areaLimits, limits));
            EventManager.Instance.addListener(EventType.DEAD_EVENT, this);
            // Aquí código para cargar los objetos del cuarto
        }

        /// <summary>
        /// Libera el contenido del cuarto
        /// </summary>
        public virtual void UnloadContent()
        {
            // Luego se manda a liberar toda referencia que exista a dichos objetos
            for (int i = 0; i < areaItems.Count; i++)
            {
                EntityManager.Instance.removeEntity(areaItems[i]);
            }

            areaItems.Clear();
            EventManager.Instance.removeEntityFromListeners(this);
            Program.GAME.ComponentManager.removeComponentsFromEntity(this);
            collisionComponent = null;
        }

        /// <summary>
        /// Agrega un item o entidad al cuarto
        /// </summary>
        /// <param name="roomItem"></param>
        public void addAreaItem(IEntity roomItem)
        {
            this.areaItems.Add(roomItem);
        }

        public virtual void getCameraLimits(ref float x1, ref float y1, ref float x2, ref float y2)
        {
            x1 = GameConstants.CAMERA_INNER_BORDER; 
            y1 = GameConstants.CAMERA_INNER_BORDER;
            x2 = (this.WidthBlocks * GameConstants.BLOCK_SIZE) - GameConstants.CAMERA_INNER_BORDER; 
            y2 = (this.HeightBlocks * GameConstants.BLOCK_SIZE) - GameConstants.CAMERA_INNER_BORDER;
        }

        public void invoke(events.DeadEvent eventArgs)
        {
            this.areaItems.Remove(eventArgs.DeadEntity);
        }

        public AreaCollisionComponent CollisionComponent
        {
            get
            {
                return this.collisionComponent;
            }
        }

        /// <summary>
        /// Gets o Sets modo trance
        /// </summary>
        public bool IsTranceModeOn
        {
            // por ahora no notificar eventos :/
            set { changeState(EntityState.TranceMode, value, false); }
            get { return getState(EntityState.TranceMode); }
        }
    }
}
