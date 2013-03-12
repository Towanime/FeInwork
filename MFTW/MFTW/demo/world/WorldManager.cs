using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.entities;
using FeInwork.Core.Interfaces;
using FeInwork.FeInwork.renderers.animation;
using Microsoft.Xna.Framework;
using FeInwork.FeInwork.renderers;
using FeInwork.Core.Util;
using FeInwork.FeInwork.components;
using FeInwork.core.physics;
using FeInwork.core.collision;
using FeInwork.core.collision.bodies;
using FeInwork.Core.Managers;
using FeInwork.collision.responses;
using FeInwork.core.Base;
using FeInwork.core.managers;
using FeInwork.FeInwork.util;
using FeInwork.FeInwork.actions;
using FeInwork.FeInwork.components.interfaces;
using FeInwork.FeInwork.draweffects;
using FeInwork.Core.Base;
using FeInwork.FeInwork.triggers;
using FeInwork.Core.Base.Animation;

namespace FeInwork.FeInwork.world
{
    public class WorldManager
    {
        /// <summary>
        /// Instancia unica del WorldManager.
        /// </summary>
        private static WorldManager instance;
        /// <summary>
        /// Lista de cuartos indexadas por Id.
        /// </summary>
        private Dictionary<string, AreaEntity> area = new Dictionary<string, AreaEntity>();
        /// <summary>
        /// Cuarto actual.
        /// </summary>
        private AreaEntity actualArea;
        /// <summary>
        /// Entidad principal del juego cuyo estado es trasladado desde distintos cuartos.
        /// </summary>
        private DrawableEntity mainEntity;
        /// <summary>
        /// Indica si este Manager ya ha cargado su data inicial.
        /// </summary>
        private bool hasLoaded = false;

        /// <summary>
        /// Cuarto actual.
        /// </summary>
        public AreaEntity ActualArea
        {
            get { return this.actualArea; }
        }

        /// <summary>
        /// Entidad principal del juego cuyo estado es trasladado desde distintos cuartos.
        /// </summary>
        public IEntity MainEntity
        {
            get { return this.mainEntity; }
            set { this.mainEntity = (DrawableEntity) value; }
        }

        private WorldManager()
        {
            initialize();
        }

        private void initialize()
        {
            this.area = new Dictionary<string, AreaEntity>();
        }

        /// <summary>
        /// Carga inicial despues de haber construido el objeto, solo debe hacerse una vez
        /// </summary>
        public void LoadContent()
        {
            if (hasLoaded == false)
            {
                // Aquí inicializo todo lo referente a la entidad principal ya que aún no se donde ponerlo
                DrawableEntity goemon = new DrawableEntity(GlobalIDs.MAIN_CHARACTER_ID);
                this.mainEntity = goemon;
                goemon.Position = new Vector2(175 + 100, 200 + 200);
                //HERE DEADEVENT SE DEBERÏA MOVER PERO SI NO PRODUCE BUG
                //el orden de creación de los componentes influye en el orden en que se envían los eventos
                goemon.addComponent(new DeadComponent(goemon));

                /************** Inicialización animación Goemon **********/
                Animation runningAnimation = new Animation(ActionsList.Running, 30, 38, 8, 40, 9, new Vector2(3.84f, 5f), 0, 0);
                runningAnimation.addAllDefaultFrames(6);
                Animation attackAnimation = new Animation(ActionsList.Attack, 55, 52, 5, 4, 185, new Vector2(3.84f, 5f), 10, 10);
                attackAnimation.addAllDefaultFrames(2);
                attackAnimation.addFrame(new AnimationFrame(new Rectangle(110, 185, 48, 52), 5, null, new Vector2(3.84f, 5f), 10, 10));
                attackAnimation.addFrame(new AnimationFrame(new Rectangle(162, 185, 55, 52), 5, null, new Vector2(3.84f, 5f), -10, 10));
                attackAnimation.addFrame(new AnimationFrame(new Rectangle(220, 185, 55, 52), 5,
                    "Rectangle TestBody False 180 70 0 -20 False;FeInwork.collision.responses.SingleHitCollisionResponse!TestBody;10",
                    new Vector2(3.84f, 5f), -10, 10));

                Animation deadAnimation = new Animation(ActionsList.Dead, 76, 78, 6, 192, 577, new Vector2(1.7f, 1.7f));
                deadAnimation.addAllDefaultFrames(4);
                Animation stillAnimation = new Animation(ActionsList.Default, 28, 34, 60, 315, 10, new Vector2(3.84f, 5f), 0, 0);
                stillAnimation.addAllDefaultFrames(4);

                //Todas las animiaciones se deben agregar al animation manager que necesita una por defecto
                AnimationManagerComponent animationManager = new AnimationManagerComponent(goemon, new AliveAttackerActionChooser(goemon), stillAnimation);
                animationManager.addAction(attackAnimation);
                animationManager.addAction(runningAnimation);
                animationManager.addAction(deadAnimation);
                /************** FIN de Inicialización animación Goemon **********/

                // Efectos de dibujo a aplicarle al personaje principal
                goemon.addDrawEffect(new HaloEffect(goemon));
                goemon.addDrawEffect(new FadeEffect(goemon, 0.02f, 0.4f, 1.0f));
                goemon.addComponent(new AnimationRenderer(goemon, AssetNames.GOEMON_ASSET, animationManager, 5, 5));
                
                goemon.addComponent(new ControlComponent(goemon));
                goemon.addComponent(new PhysicalAttackComponent(goemon, 10));
                goemon.addComponent(new SpecialActionManagerComponent(goemon));
                HealthComponent hp = new HealthComponent(goemon, null, 100, 70, 120);
                goemon.addComponent(hp);
                //goemon.addComponent(new HealthRenderer(goemon, hp, 10, 10));

                PhysicsData goemonPhysics = new PhysicsData();
                goemonPhysics.MinimumVelocity = new Vector2(-500, -5000);
                goemonPhysics.MaximumVelocity = new Vector2(500, 2000);
                goemonPhysics.Weight = 20;
                goemonPhysics.RunningImpulse = 30000;
                goemonPhysics.AirImpulse = 15000;
                goemonPhysics.JumpImpulse = 1200000;
                goemon.addComponent(new BasicPhysicsComponent(goemon, goemonPhysics));

                Box goemonMainBody = ShapeFactory.CreateRectangle("goemonMainBody", goemon, true, false, 90, 160, goemon.Position, true, GameLayers.MIDDLE_PLAY_AREA, GameConstants.TANGIBLE_BODY_TAG);
                Box goemonPortalBody = ShapeFactory.CreateRectangle("goemonPortalBody", goemon, true, false, 20, 20, goemon.Position, true, GameLayers.MIDDLE_PLAY_AREA, GameConstants.OPEN_DOOR_TAG);
                Box goemonInteractBody = ShapeFactory.CreateRectangle("goemonInteractBody", goemon, false, false, 95, 140, new Vector2(135 + 100, 130 + 200), false, GameLayers.MIDDLE_PLAY_AREA, Color.Blue);
                Box goemonTriggerBody = ShapeFactory.CreateRectangle("goemonTriggerBody", goemon, false, false, 95, 140, new Vector2(135 + 100, 130 + 200), false, GameLayers.MIDDLE_PLAY_AREA, Color.AliceBlue);

                Box goemonTriggerBody2 = ShapeFactory.CreateRectangle("goemonTriggerBody2", goemon, false, false, 200, 140, new Vector2(135 + 100, 130 + 200), false, GameLayers.MIDDLE_PLAY_AREA, Color.AliceBlue);

                List<CollisionBody> goemonBodies = new List<CollisionBody>();
                goemonBodies.Add(goemonMainBody);
                goemonBodies.Add(goemonPortalBody);
                goemonBodies.Add(goemonInteractBody);
                goemonBodies.Add(goemonTriggerBody);
                //goemonBodies.Add(goemonTriggerBody2);
                //goemonBodies.Add(earthCastingArea);
                goemon.addComponent(new AnimatedCollisionComponent(goemon, goemonBodies));
                
                // listener general de colisiones para mario
                EventManager.Instance.addCollisionListener(goemon, new SoftBodyCollisionResponse(goemon));

                Camera2D camera = new Camera2D(Program.GAME, goemon);
                Program.GAME.Camera = camera;
                // mover camara a un target random lejos
                camera.MoveSpeed = 2f;

                // debería ir código para cargar la lista de rooms y doors de los xml
                AreaEntity roomTestM = new AreaTestMetroid();

                this.hasLoaded = true;
            }
            else
            {
                throw new InvalidOperationException("El WorldManager ya ha sido cargado anteriormente.");
            }
        }

        public void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Cambia inmediatamente de cuarto según la Id del cuarto
        /// </summary>
        /// <param name="areaId">Id del cuarto a cambiar</param>
        public void setArea(string areaId)
        {
            if (this.area.ContainsKey(areaId))
            {
                AreaEntity oldArea = this.actualArea;
                AreaEntity newArea = this.area[areaId];

                if (oldArea != null)
                {
                    oldArea.UnloadContent();
                }

                CollisionManager.Instance.resetState(-GameConstants.ROOM_OUTER_BORDER, -GameConstants.ROOM_OUTER_BORDER,
                    (newArea.WidthBlocks * GameConstants.BLOCK_SIZE) + GameConstants.ROOM_OUTER_BORDER * 2, (newArea.HeightBlocks * GameConstants.BLOCK_SIZE) + GameConstants.ROOM_OUTER_BORDER * 2);

                float cameraLimitX1 = 0;
                float cameraLimitY1 = 0;
                float cameraLimitX2 = 0;
                float cameraLimitY2 = 0;
                newArea.getCameraLimits(ref cameraLimitX1, ref cameraLimitY1, ref cameraLimitX2, ref cameraLimitY2);
                Program.GAME.Camera.setLimits(cameraLimitX1, cameraLimitY1, cameraLimitX2, cameraLimitY2);

                newArea.LoadContent();
                this.actualArea = newArea;
                Vector2 newEntityPosition = new Vector2(567,341);

                // Esto quizas lo cambie luego, reposicionar la entidad y cámara según las coordenadas de salida de la puerta.
                List<AbstractCollisionComponent> mainEntityCollisionComponentList = mainEntity.findByBaseClass<AbstractCollisionComponent>();
                if (mainEntityCollisionComponentList.Count > 0)
                {
                    mainEntityCollisionComponentList[0].move(
                        mainEntity.getVectorProperty(EntityProperty.Position), newEntityPosition);
                }

                mainEntity.changeVectorProperty(EntityProperty.Position, newEntityPosition, false);
                Program.GAME.Camera.resetCameraPosition();
            }
            else
            {
                // En caso de que se intente salir por una puerta que no existe.
                throw new InvalidOperationException("El cuarto con el ID " + areaId + " no se encuentra registrado.");
            }
        }

        /// <summary>
        /// Agrega un cuarto a la lista global de cuartos.
        /// </summary>
        /// <param name="room">Entidad a agregar.</param>
        public void addArea(AreaEntity room)
        {
            this.area.Add(room.Id, room);
        }

        /// <summary>
        /// Obtiene la instancia única del contenedor de Cuartos y Puertas
        /// </summary>
        public static WorldManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorldManager();
                }
                return instance;
            }
        }
    }
}
