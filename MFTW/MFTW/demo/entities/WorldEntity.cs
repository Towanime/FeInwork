using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Util;
using Microsoft.Xna.Framework;

namespace AlchemistDemo.alchemist.entities
{
    /// <summary>
    /// Clase que representa el stage que es mostrado, tiene propiedades utiles como 
    /// la que sirve para saber en que estado esta el "mundo" ej: TranceMode
    /// </summary>
    public class WorldEntity : Entity
    {
        
        public WorldEntity() 
        {
            initialize();
        }

        public override void initialize()
        {
            // este del padre crea el property body asi que es importante llamarlo primero.
            base.initialize();
            // se asignas unas propiedades extras
            addProperty<bool>(PropertyList.TranceMode, false);
            // se le agrega el componente renderer
            addComponent(new TestStage(this), false);
        }

        // a este tipo de entidades se le puede poner getters para obtener ciertas propiedades facilmente
        public bool IsTranceModeOn
        {
            // por ahora no notificar eventos :/
            set { changeProperty<bool>(PropertyList.TranceMode, value, false); }
            get { return getProperty<bool>(PropertyList.TranceMode); }
        }
    }
}
