using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.Base;
using Microsoft.Xna.Framework;

namespace FeInwork.core.managers
{
    /// <summary>
    /// Controller manager maneja todos los controles del juego.
    /// Si se trata de agregar un componente tipo BaseControlComponent 
    /// al component manager entonces tira una excepcion ya que los updates
    /// de estos componentes se hacen aca.
    /// 
    /// Esta clase tiene un stack de controles donde el ultimo de la lista
    /// es el que se considera como activo y se hace update.
    /// </summary>
    public class ControlManager
    {
        /// <summary>
        /// Instancia de este objeto
        /// </summary>
        private static ControlManager instance;
        private List<BaseControlComponent> controlStack;
        /// <summary>
        /// True para no hacer update a ningun controller
        /// </summary>
        private bool isDisableAll;

        private ControlManager()
        {
            controlStack = new List<BaseControlComponent>();
        }

        /// <summary>
        /// Agrega un control component a la lista y lo habilita.
        /// </summary>
        /// <param name="control"></param>
        public void addController(BaseControlComponent control)
        {
            if(!controlStack.Contains(control)){
                control.Enabled = true;
                controlStack.Add(control);
            }else{
                throw new Exception("Controller already added!");
            }
        }

        /// <summary>
        /// Saca un control de la lista.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool removeController(BaseControlComponent control)
        {
            control.Enabled = false;
            return controlStack.Remove(control);
        }

        /// <summary>
        /// Hace update al control principal.
        /// </summary>
        /// <param name="gameTime"></param>
        public void update(GameTime gameTime)
        {
            // actualiza el ultimo
            // por ahora no comprobar si la lista esta vacia coz al iniciar el juego se tiene un control por defecto.
            controlStack[controlStack.Count-1].Update(gameTime);
        }

        /// <summary>
        /// Obtiene la unica instancia existente del ControlManager
        /// </summary>
        public static ControlManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ControlManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// True para desabilitar todos los controles.
        /// </summary>
        public bool IsDisableAll
        {
            get{
                return this.isDisableAll;
            }
            set
            {
                this.isDisableAll = value;
            }
        }
    }
}
