using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork;
using PerformanceUtility.GameDebugTools;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework.Input;
using FeInwork.core.managers;
using FeInwork.FeInwork.world;
using Microsoft.Xna.Framework.GamerServices;
using System.Globalization;
using FeInwork.FeInwork.util;

namespace FeInwork.Core.Util
{
    public class GameDebug
    {
        GameClass game;
        // Our debug system. We can keep this reference or use the DebugSystem.Instance
        // property once we've called DebugSystem.Initialize.
        DebugSystem debugSystem;

        Texture2D blank;
        SpriteFont font;

        Texture2D pointer;

        Vector2 mouseScreenPosition = Vector2.Zero;
        Vector2 mouseWorldPosition = Vector2.Zero;

        // Position for debug command test.
        Vector2 debugPos = new Vector2(100, 100);

        // Stopwatch for TimeRuler test.
        Stopwatch stopwatch = new Stopwatch();

        private static GameDebug instance;

        public static GameDebug Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameDebug();
                }
                return instance;
            }
        }

        public DebugSystem DebugSystem
        {
            get
            {
                return debugSystem;
            }
        }

        public GameDebug()
        {
            game = Program.GAME;
            font = game.Content.Load<SpriteFont>("CourierNew");
            blank = game.Content.Load<Texture2D>("teststage/blank_pixel");
            pointer = game.Content.Load<Texture2D>("permanent/cursor");

            // initialize the debug system with the game and the name of the font 
            // we want to use for the debugging
            debugSystem = DebugSystem.Initialize(game, "CourierNew");

            // register a new command that lets us move a sprite on the screen
            debugSystem.DebugCommandUI.RegisterCommand(
                "pos",              // Name of command
                "set position",     // Description of command
                PosCommand          // Command execution delegate
                );
        }

        public void Update(GameTime gameTime)
        {
            HandleInput();

	        MouseState mouseState = Mouse.GetState();
            mouseScreenPosition.X = mouseState.X;
            mouseScreenPosition.Y = mouseState.Y;

            mouseWorldPosition = game.Camera.Position + (mouseScreenPosition - new Vector2(game.ResolutionWidth / 2, game.ResolutionHeight / 2)) / game.Camera.Zoom;

            game.GameConsole.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = SpriteBatchManager.Instance.getSpriteBatchDebugHud();

            // Show usage.
            string message =
                "ScreenCursorCoords X:" + Math.Round(mouseScreenPosition.X) + ", Y:" + Math.Round(mouseScreenPosition.Y) + "\n" +
                "WorldCursorCoords X:" + Math.Round(mouseWorldPosition.X) + ", Y:" + Math.Round(mouseWorldPosition.Y);

            Vector2 size = font.MeasureString(message);
            Layout layout = new Layout(game.GraphicsDevice.Viewport);

            float margin = font.LineSpacing;
            Rectangle rc = new Rectangle(0, 0,
                                    (int)(size.X + margin),
                                    (int)(size.Y + margin));

            // Compute boarder size, position.
            rc = layout.Place(rc, 0.01f, 0.01f, Alignment.TopRight);
            spriteBatch.Draw(blank, rc, Color.Black * .5f);

            spriteBatch.Draw(pointer, mouseScreenPosition, new Rectangle(0, 0, pointer.Width, pointer.Height),
                Color.White * 0.7f, 0, new Vector2(pointer.Width / 2, pointer.Height / 2), 1, SpriteEffects.None, 1);

            // Draw usage message text.
            layout.ClientArea = rc;
            Vector2 pos = layout.Place(size, 0, 0, Alignment.Center);
            spriteBatch.DrawString(font, message, pos, Color.White);

            game.CollisionManager.Draw(gameTime);
            game.GameConsole.Draw(gameTime);
        }

        private void HandleInput()
        {
            // Handle exit game.
            if (InputManager.isNewPressKeyOrButton(Keys.Escape, Buttons.Back))
            {
                game.Exit();
            }

            // Show/Hide FPS counter by press A button.
            if (InputManager.isNewPressKeyOrButton(Keys.F1, Buttons.DPadUp))
            {
                debugSystem.FpsCounter.Visible = !debugSystem.FpsCounter.Visible;
            }

            // Show/Hide TimeRuler by press B button.
            if (InputManager.isNewPressKeyOrButton(Keys.F2, Buttons.DPadDown))
            {
                debugSystem.TimeRuler.Visible = !debugSystem.TimeRuler.Visible;
            }

            // Show/Hide TimeRuler log by press X button.
            if (InputManager.isNewPressKeyOrButton(Keys.F3, Buttons.DPadLeft))
            {
                debugSystem.TimeRuler.Visible = true;
                debugSystem.TimeRuler.ShowLog = !debugSystem.TimeRuler.ShowLog;
            }

            if (InputManager.isNewPressKeyOrButton(Keys.F4, Buttons.DPadRight))
            {
                CollisionManager.Instance.changeDrawingLayer();
            }

            if (InputManager.isNewPressKeyOrButton(Keys.F5, Buttons.RightStick))
            {
                game.GameConsole.ShowHideWindow();
            }
        }

        // Invoked after the user inputs text from the on screen keyboard
        private void InputDebugCommandCallback(IAsyncResult result)
        {
            // get the string they entered
            string cmd = Guide.EndShowKeyboardInput(result);

            // if they entered something, execute the command
            if (!string.IsNullOrEmpty(cmd))
                debugSystem.DebugCommandUI.ExecuteCommand(cmd);
        }

        /// <summary>
        /// This method is called from DebugCommandHost when the user types the 'pos'
        /// command into the command prompt. This is registered with the command prompt
        /// through the DebugCommandUI.RegisterCommand method we called in Initialize.
        /// </summary>
        public void PosCommand(IDebugCommandHost host, string command, IList<string> arguments)
        {
            // if we got two arguments from the command
            if (arguments.Count == 2)
            {
                // process text "pos xPos yPos" by parsing our two arguments
                debugPos.X = Single.Parse(arguments[0], CultureInfo.InvariantCulture);
                debugPos.Y = Single.Parse(arguments[1], CultureInfo.InvariantCulture);
            }
            else
            {
                // if we didn't get two arguments, we echo the current position of the cat
                host.Echo(String.Format("Pos={0},{1}", debugPos.X, debugPos.Y));
            }
        }
    }
}
