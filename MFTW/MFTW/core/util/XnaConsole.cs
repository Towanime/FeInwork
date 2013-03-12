using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using FeInwork;

namespace FeInwork.Core.Util
{
    public class XNAConsole : DrawableGameComponent
    {
        private static XNAConsole instance;

        enum ConsoleState
        {
            Closed,
            Closing,
            Open,
            Opening,
            Stopped
        }

        const double AnimationTime = 0.3;
        const int LinesDisplayed = 20;

        string OutputBuffer;
        int lineWidth, consoleXSize, consoleYSize;

        GraphicsDevice device;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D background;

        ConsoleState State;
        bool isConsoleStopped;
        bool showWindow;
        bool hideWindow;
        double StateStartTime;
        KeyboardState LastKeyState, CurrentKeyState;

        public XNAConsole(Game game, SpriteFont font)
            : base(game)
        {
            this.Visible = false;
            device = game.GraphicsDevice;
            spriteBatch = new SpriteBatch(device);
            this.font = font;
            background = new Texture2D(device, 1, 1, false,
                SurfaceFormat.Color);
            background.SetData<Color>(new Color[1] { new Color(0, 0, 0, 125) });

            consoleXSize = Game.Window.ClientBounds.Right - Game.Window.ClientBounds.Left - 20;
            consoleYSize = font.LineSpacing * LinesDisplayed + 20;
            lineWidth = (int)((consoleXSize - 20) / font.MeasureString("a").X) - 2;

            State = ConsoleState.Closed;
            StateStartTime = 0;
            LastKeyState = this.CurrentKeyState = Keyboard.GetState();

            OutputBuffer = "";
        }

        private bool IsKeyPressed(Keys key)
        {
            return CurrentKeyState.IsKeyDown(key) && !LastKeyState.IsKeyDown(key);
        }



        public override void Update(GameTime gameTime)
        {
            double now = gameTime.TotalGameTime.TotalSeconds;
            double elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds;

            LastKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            if (State == ConsoleState.Closing)
            {
                if (now - StateStartTime > AnimationTime)
                {
                    State = ConsoleState.Closed;
                    StateStartTime = now;
                    this.Visible = false;
                }
                return;
            }

            if (State == ConsoleState.Opening)
            {
                if (now - StateStartTime > AnimationTime)
                {
                    State = ConsoleState.Open;
                    StateStartTime = now;
                }
                return;
            }

            if (State == ConsoleState.Closed)
            {
                if (showWindow)
                {
                    State = ConsoleState.Opening;
                    StateStartTime = now;
                    this.Visible = true;
                    showWindow = false;
                }
                else
                {
                    return;
                }
            }

            if (State == ConsoleState.Open)
            {
                if (hideWindow)
                {
                    State = ConsoleState.Closing;
                    StateStartTime = now;
                    hideWindow = false;
                    return;
                }
            }
        }

        public void ShowHideWindow()
        {
            if (State == ConsoleState.Open)
            {
                hideWindow = true;
            }

            if (State == ConsoleState.Closed)
            {
                showWindow = true;
            }
        }

        private List<string> ParseOutputBuffer(string line)
        {
            List<string> wraplines = new List<string>();
            if (line.Length > 0)
            {
                wraplines.Add("");
                int lineNum = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    string ch = line.Substring(i, 1);

                    if (ch == "\n" || wraplines[lineNum].Length > lineWidth)
                    {
                        wraplines.Add("");
                        lineNum++;
                    }
                    else
                    {
                        wraplines[lineNum] += ch;
                    }
                }
            }

            wraplines.Reverse();
            return wraplines;
        }

        public static void Write(string str)
        {
            if (!Instance.isConsoleStopped)
            {
                Instance.OutputBuffer += str;
            }
        }

        public static void WriteLine(string str)
        {
            if (!Instance.isConsoleStopped)
            {
                Instance.OutputBuffer += "\n" + str;
            }
        }

        public static void Clear()
        {
            Instance.OutputBuffer = "";
        }

        public static void Stop()
        {
            if ((Instance.State == ConsoleState.Open ||
                Instance.State == ConsoleState.Opening) && Instance.isConsoleStopped == false)
            {
                WriteLine("Console Stopped");
                Instance.isConsoleStopped = true;
            }
        }

        public static void Resume()
        {
            if ((Instance.State == ConsoleState.Open ||
                Instance.State == ConsoleState.Opening) && Instance.isConsoleStopped == true)
            {
                Instance.isConsoleStopped = false;
            }
        }

        public static XNAConsole Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XNAConsole(Program.GAME, Program.GAME.Content.Load<SpriteFont>("CourierNew"));
                }
                return instance;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.DrawOrder = 100;

            double now = gameTime.TotalGameTime.TotalSeconds;

            consoleXSize = this.Game.Window.ClientBounds.Right - this.Game.Window.ClientBounds.Left - 20;
            consoleYSize = this.font.LineSpacing * LinesDisplayed + 20;

            int consoleXOffset = 10;
            int consoleYOffset = 0;

            if (State == ConsoleState.Opening)
            {
                consoleYOffset = (int)MathHelper.Lerp(-consoleYSize, 0, (float)Math.Sqrt((float)(now - StateStartTime) / (float)AnimationTime));
            }
            else if (State == ConsoleState.Closing)
            {
                consoleYOffset = (int)MathHelper.Lerp(0, -consoleYSize, ((float)(now - StateStartTime) / (float)AnimationTime) * ((float)(now - StateStartTime) / (float)AnimationTime));
            }

            this.lineWidth = (int)((consoleXSize - 20) / font.MeasureString("a").X) - 2;

            if (State != ConsoleState.Closed)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                spriteBatch.Draw(background, new Rectangle(consoleXOffset, consoleYOffset, consoleXSize, consoleYSize), Color.White);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                List<string> lines = ParseOutputBuffer(OutputBuffer);
                for (int j = 0; j < lines.Count; j++ )
                {
                    string str = lines[j];
                    if (j < LinesDisplayed)
                    {
                        if (isConsoleStopped && j == 0)
                        {
                            spriteBatch.DrawString(font, str, new Vector2(consoleXOffset + 10, consoleYOffset + consoleYSize - 30 - font.LineSpacing * (j)), Color.Red);
                        }
                        else
                        {
                            spriteBatch.DrawString(font, str, new Vector2(consoleXOffset + 10, consoleYOffset + consoleYSize - 30 - font.LineSpacing * (j)), Color.White);
                        }
                    }
                }

                spriteBatch.End();
            }
        }
    }
}