using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using FeInwork.FeInwork.util;

namespace FeInwork.Core.Base
{
    /// <summary>
    /// Manejador de inputs para llevar un control sobre
    /// las acciones relacionadas a los mismos.
    /// </summary>
    public class InputManager
    {
        #region Private Variables

        /// <summary>
        /// Estado de teclado anterior
        /// </summary>
        private static KeyboardState lastKeyboardState;

        /// <summary>
        /// Estado de teclado actual
        /// </summary>
        private static KeyboardState currentKeyboardState;

        /// <summary>
        /// Estado de gamepad actual
        /// </summary>
        private static GamePadState lastGamePadState;

        /// <summary>
        /// Estado de gamepad anterior
        /// </summary>
        private static GamePadState currentGamePadState;

        #endregion

        #region Public Methods

        /// <summary>
        /// Actualiza ambos estados de teclado constantemente
        /// </summary>
        public static void update()
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            lastGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(0);
        }

        /// <summary>
        /// Verifica si la tecla recibida es una
        /// nueva tecla presionada.
        /// </summary>
        /// <param name="key">Tecla a verificar</param>
        /// <returns>True si es una nueva tecla.</returns>
        public static bool isNewPress(Keys key)
        {
            return (
                lastKeyboardState.IsKeyUp(key) &&
                currentKeyboardState.IsKeyDown(key));
        }

        /// <summary>
        /// Verifica si el boton recibida es un
        /// nuevo boton presionado.
        /// </summary>
        /// <param name="key">Boton a verificar</param>
        /// <returns>True si es un nuevo boton.</returns>
        public static bool isNewPress(Buttons button)
        {
            return (
                lastGamePadState.IsButtonUp(button) &&
                currentGamePadState.IsButtonDown(button));
        }

        /// <summary>
        /// Verifica si una tecla o boton son
        /// nuevas presiones
        /// </summary>
        /// <param name="key">Tecla a verificar</param>
        /// <param name="button">Boton a verificar</param>
        /// <returns>True si es una nueva tecla o boton.</returns>
        public static bool isNewPressKeyOrButton(Keys key, Buttons button)
        {
            return isNewPress(key) || isNewPress(button);
        }

        /// <summary>
        /// Verifica si la tecla recibida se ha
        /// quedado presionada.
        /// </summary>
        /// <param name="key">Tecla a verificar</param>
        /// <returns>True si esta siendo presionada.</returns>
        public static bool isCurPress(Keys key)
        {
            return (
                lastKeyboardState.IsKeyDown(key) &&
                currentKeyboardState.IsKeyDown(key));
        }

        /// <summary>
        /// Verifica si el boton recibido se ha
        /// quedado presionado.
        /// </summary>
        /// <param name="key">Boton a verificar</param>
        /// <returns>True si esta siendo presionado.</returns>
        public static bool isCurPress(Buttons button)
        {
            return (
                lastGamePadState.IsButtonDown(button) &&
                currentGamePadState.IsButtonDown(button));
        }

        /// <summary>
        /// Verifica si una tecla o boton son
        /// estan siendo presionadas.
        /// </summary>
        /// <param name="key">Tecla a verificar</param>
        /// <param name="button">Boton a verificar</param>
        /// <returns>True si la tecla o boton estan siendo presionados.</returns>
        public static bool isCurPressKeyOrButton(Keys key, Buttons button)
        {
            return isCurPress(key) || isCurPress(button);
        }

        /// <summary>
        /// Verifica si la tecla recibida acaba
        /// de dejarse de presionar
        /// </summary>
        /// <param name="key">Tecla a verificar</param>
        /// <returns>True si fue la tecla presionada anteriormente.</returns>
        public static bool isOldPress(Keys key)
        {
            return (
                lastKeyboardState.IsKeyDown(key) &&
                currentKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        /// Verifica si el boton recibido acaba
        /// de dejarse de presionar
        /// </summary>
        /// <param name="key">Tecla a verificar</param>
        /// <returns>True si fue la tecla presionada anteriormente.</returns>
        public static bool isOldPress(Buttons button)
        {
            return (
                lastGamePadState.IsButtonDown(button) &&
                currentGamePadState.IsButtonUp(button));
        }

        /// <summary>
        /// Verifica si una tecla o boton acaban
        /// de dejar de ser presionados
        /// </summary>
        /// <param name="key">Tecla a verificar</param>
        /// <param name="button">Boton a verificar</param>
        /// <returns>True si alguno acaba de dejar de ser presionado.</returns>
        public static bool isOldPressKeyOrButton(Keys key, Buttons button)
        {
            return isOldPress(key) || isOldPress(button);
        }

        /// <summary>
        /// Verifica que todas las teclas dentro de la lista
        /// esten siendo presionadas.
        /// </summary>
        /// <param name="keyList">Lista de teclas a verificar</param>
        /// <returns>True si todas las teclas estan siendo presionadas</returns>
        public static bool isPressingAll(Keys[] keyList)
        {
            bool isPressed = true;
            for (int i=0; i<keyList.Length; i++)
            {
                Keys key = keyList[i];
                if (isCurPress(key) == false)
                {
                    isPressed = false;
                    break;
                }
            }
            return isPressed;
        }

        /// <summary>
        /// Verifica que todos los botones dentro de la lista
        /// esten siendo presionados.
        /// </summary>
        /// <param name="buttonList">Lista de botones a verificar</param>
        /// <returns>True si todos los botones estan siendo presionados</returns>
        public static bool isPressingAll(Buttons[] buttonList)
        {
            bool isPressed = true;
            for (int i = 0; i < buttonList.Length; i++)
            {
                Buttons button = buttonList[i];
                if (isCurPress(button) == false)
                {
                    isPressed = false;
                    break;
                }
            }
            return isPressed;
        }

        /// <summary>
        /// Verifica si alguna de las teclas dentro de la lista
        /// esta siendo presionada
        /// </summary>
        /// <param name="keyList">Lista de teclas a verificar</param>
        /// <returns>True si alguna tecla de la lista esta siendo presionada.</returns>
        public static bool isPressingAny(Keys[] keyList)
        {
            bool isPressed = false;
            for (int i = 0; i < keyList.Length; i++)
            {
                Keys key = keyList[i];
                if (isCurPress(key) == true)
                {
                    isPressed = true;
                    break;
                }
            }
            return isPressed;
        }

        /// <summary>
        /// Verifica si alguno de los botones dentro de la lista
        /// esta siendo presionado
        /// </summary>
        /// <param name="buttonList">Lista de botones a verificar</param>
        /// <returns>True si algun boton de la lista esta siendo presionado.</returns>
        public static bool isPressingAny(Buttons[] buttonList)
        {
            bool isPressed = false;
            for (int i = 0; i < buttonList.Length; i++)
            {
                Buttons button = buttonList[i];
                if (isCurPress(button) == true)
                {
                    isPressed = true;
                    break;
                }
            }
            return isPressed;
        }

        /*
        /// <summary>
        /// Verifica si la tecla recibida se ha
        /// quedado presionada y si ha cumplido un cierto tiempo
        /// de retraso.
        /// </summary>
        /// <param name="key">Tecla a verificar</param>
        /// <param name="delay">Tiempo de buffer en segundos</param>
        /// <returns>True si esta siendo presionada y si ha cumplido
        /// con el tiempo de retraso.</returns>
        public static bool isCurKeyboardPressWithDelay(Keys key, int delay)
        {
            keyTimers[(int)key - 1]++;
            if (
                lastKeyboardState.IsKeyDown(key) &&
                currentKeyboardState.IsKeyDown(key) &&
                keyTimers[(int)key - 1] >= delay)
            {
                keyTimers[(int)key - 1] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        */

        public static double LeftThumbStickAngle
        {
            get
            {
                return Math.Atan2(currentGamePadState.ThumbSticks.Left.X, -currentGamePadState.ThumbSticks.Left.Y);
            }
        }

        public static double RightThumbStickAngle
        {
            get
            {
                return Math.Atan2(currentGamePadState.ThumbSticks.Right.X, -currentGamePadState.ThumbSticks.Right.Y);
            }
        }

        public static double CurrentAngle
        {
            get
            {
                double angle = GameAngles.RIGHT_ANGLE;

                #if XBOX
                    angle = InputManager.LeftThumbStickAngle;
                #else
                    bool pressingRight = InputManager.isCurPress(Keys.Right);
                    bool pressingLeft = InputManager.isCurPress(Keys.Left);
                    bool pressingUp = InputManager.isCurPress(Keys.Up);
                    bool pressingDown = InputManager.isCurPress(Keys.Down);

                    if (pressingRight && pressingUp) angle = GameAngles.UPPER_RIGHT_ANGLE;
                    else if (pressingRight && pressingDown) angle = GameAngles.LOWER_RIGHT_ANGLE;
                    else if (pressingLeft && pressingUp) angle = GameAngles.UPPER_LEFT_ANGLE;
                    else if (pressingLeft && pressingDown) angle = GameAngles.LOWER_LEFT_ANGLE;
                    else if (pressingRight) angle = GameAngles.RIGHT_ANGLE;
                    else if (pressingLeft) angle = GameAngles.LEFT_ANGLE;
                    else if (pressingUp) angle = GameAngles.UPPER_ANGLE;
                    else if (pressingDown) angle = GameAngles.LOWER_ANGLE;
                #endif

                return angle;
            }
        }

        #endregion
    }
}
