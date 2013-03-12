using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FeInwork.FeInwork.world;
using FeInwork.Core.Interfaces;
using FeInwork.core.Base;
using FeInwork.core.managers;
using FeInwork.Core.Util;
using FeInwork.Core.Managers;

namespace FeInwork
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameClass : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        //private SpriteBatch spriteBatch;
        private IComponentManager componentManager;
        private WorldManager worldManager;
        private EntityManager entityManager;
        private CollisionManager collisionManager;
        private Camera2D camera;
        private GameDebug gameDebug;
        private XNAConsole gameConsole;
        private bool isWorkMode = false;
        private bool isDebugMode = false;

        public GameClass()
        {
            #if DEBUG
                isDebugMode = true;
            #endif
            graphics = new GraphicsDeviceManager(this);
            if (isWorkMode)
            {
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 480;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
            }
            Content.RootDirectory = "Content";
            // Para activar vSync
            graphics.SynchronizeWithVerticalRetrace = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            componentManager = BasicComponentManager.Instance;
            worldManager = WorldManager.Instance;
            collisionManager = CollisionManager.Instance;
            entityManager = EntityManager.Instance;
            // batch manager
            SpriteBatchManager.Instance.initialize(GraphicsDevice);

            if (isDebugMode)
            {
                gameDebug = GameDebug.Instance;
                gameConsole = XNAConsole.Instance;
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            worldManager.LoadContent();
            worldManager.setArea("AreaTestMetroid");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (isDebugMode)
            {
                // tell the TimeRuler that we're starting a new frame. you always want
                // to call this at the start of Update
                gameDebug.DebugSystem.TimeRuler.StartFrame();
                // Start measuring time for "Update".
                gameDebug.DebugSystem.TimeRuler.BeginMark(PerformanceUtility.GameDebugTools.TimeRuler.updateMarker, Color.Blue);
                gameDebug.Update(gameTime);
            }

            worldManager.Update(gameTime);
            componentManager.update(gameTime);
            entityManager.update(gameTime);

            base.Update(gameTime);

            if (isDebugMode)
            {
                // Stop measuring time for "Update".
                gameDebug.DebugSystem.TimeRuler.EndMark(PerformanceUtility.GameDebugTools.TimeRuler.updateMarker);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (isDebugMode)
            {
                // Start measuring time for "Draw".
                gameDebug.DebugSystem.TimeRuler.BeginMark(PerformanceUtility.GameDebugTools.TimeRuler.drawMarker, Color.Yellow);
                GraphicsDevice.Clear(Color.Black);

                SpriteBatchManager.Instance.begin();
                componentManager.draw(gameTime);
                SpriteBatchManager.Instance.end();
                SpriteBatchManager.Instance.beginDebug();
                gameDebug.Draw(gameTime);
                SpriteBatchManager.Instance.endDebug();
                base.Draw(gameTime);

                // Stop measuring time for "Draw".
                gameDebug.DebugSystem.TimeRuler.EndMark(PerformanceUtility.GameDebugTools.TimeRuler.drawMarker);
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
                SpriteBatchManager.Instance.begin();
                componentManager.draw(gameTime);
                SpriteBatchManager.Instance.end();
                base.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        #region Properties - para obtener cosas utiles

        public IComponentManager ComponentManager
        {
            get { return this.componentManager; }
        }

        public WorldManager WorldManager
        {
            get { return this.worldManager; }
        }

        public EntityManager EntityManager
        {
            get { return this.entityManager; }
        }

        public CollisionManager CollisionManager
        {
            get { return this.collisionManager; }
        }

        public XNAConsole GameConsole
        {
            get { return this.gameConsole; }
        }

        public GameDebug GameDebug
        {
            get { return this.gameDebug; }
        }

        public float ResolutionWidth
        {
            get { return graphics.PreferredBackBufferWidth; }
        }

        public float ResolutionHeight
        {
            get { return graphics.PreferredBackBufferHeight; }
        }

       /* public SpriteBatch SpriteBatch 
        { 
            get { return spriteBatch; } 
        }*/

        public GraphicsDeviceManager Graphics 
        { 
            get { return graphics; } 
        }

        public Camera2D Camera 
        { 
            get { return camera; } 
            set { camera = value; } 
        }

        public bool IsDebugMode
        {
            get { return isDebugMode; }
        }
        #endregion
    }
}
