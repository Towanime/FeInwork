using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.core.util
{
    /// <summary>
    /// Clase para que lleve el conteo de segundos que han pasado desde que se inicia.
    /// Tiene metodos para comprobar cuantos segundos han pasado o minutos y para reiniciarlo.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// Cantidad de minutos transcurridos.
        /// </summary>
        private int minutes;
        /// <summary>
        /// Cantidad de segundos transcurridos desde el nuevo minuto.
        /// </summary>
        private int seconds;
        /// <summary>
        /// Total de segundos transcurridos desde que el contador entro en marcha.
        /// </summary>
        private int totalSeconds;
        /// <summary>
        /// Milisegundos transcurridos desde el ultimo update.
        /// </summary>
        private long milliseconds;
        /// <summary>
        /// Multiplicador para segundos.
        /// </summary>
        private float interval = 1000f;

        public Timer()
        {
            // nothing yet?
        }

        public void updateTime(GameTime gameTime)
        {
            milliseconds += (long)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (milliseconds >= interval)
            {
                seconds += 1;
                totalSeconds += 1;
                milliseconds = 0;
            }
            // otro minuto?
            if(seconds == 60){
                minutes += 1;
                seconds = 0;
            }
        }

        /// <summary>
        /// Verifica si el timer ya ha pasado por este segundo desde que se incio.
        /// </summary>
        /// <param name="sec">Segundo a evaluar.</param>
        /// <param name="resetIfTrue">True para hacer que el contador reincie sus valores de cumplirse la condicion</param>
        /// <returns>true si el timer ha pasado por ese segundo o es igual al mismo.</returns>
        public bool totalSecondsPassed(int sec, bool resetIfTrue)
        {
            bool toReturn = false;
            if(sec <= totalSeconds){
                toReturn = true;
                if (resetIfTrue)
                {
                    reset();
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Verifica si el minuto ya ha pasado por este segundo.
        /// </summary>
        /// <param name="sec">Segundo a evaluar.</param>
        /// <param name="resetIfTrue">True para hacer que el contador reincie sus valores de cumplirse la condicion</param>
        /// <returns>true si el timer ha pasado por ese segundo o es igual al mismo.</returns>
        public bool secondsInMinutePassed(int sec, bool resetIfTrue)
        {
            bool toReturn = false;
            if (sec <= seconds)
            {
                toReturn = true;
                if (resetIfTrue)
                {
                    reset();
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Reinicia los valores de un contador.
        /// Total de segundos y minutos = 0
        /// </summary>
        public void reset()
        {
            milliseconds = 0;
            seconds = 0;
            totalSeconds = 0;
            minutes = 0;
        }

        #region Properties

        public int Minutes
        {
            get
            {
                return this.minutes;
            }
        }

        /// <summary>
        /// Estos segundos son los que pasan desde que comenzo el nuevo minuto.
        /// Para obtener el total de 
        /// </summary>
        public int CurrentSeconds
        {
            get
            {
                return this.seconds;
            }
        }

        public int TotalSeconds
        {
            get
            {
                return this.totalSeconds;
            }
        }
        #endregion
    }
}
