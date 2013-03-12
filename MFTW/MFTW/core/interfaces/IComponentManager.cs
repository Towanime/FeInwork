using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.Core.Interfaces
{
    /// <summary>
    /// Interface define el objeto que controlara todo lo referente a llamadas al update y drawing de los componentes
    /// existentes.
    /// Los que implementen estas clases deben ser singletons....
    /// </summary>
	public interface IComponentManager
	{
        /// <summary>
        /// Hace update a todos los componetes que tenga en su lista y que implementen IUpdateable.
        /// </summary>
        /// <param name="gameTime"></param>
		void update(GameTime gameTime);

        /// <summary>
        /// Llama metodo draw a todos los componentes dibujables.
        /// </summary>
        /// <param name="gameTIme"></param>
		void draw(GameTime gameTime);

        /// <summary>
        /// Trata de agregar un componente a una de sus listas, este metodo depende mucho de la implementacion 
        /// que se le de, pues por ejempo si paso un componente tipo IUpdateable lo mas seguro es que el 
        /// manager al que se le pasa si contenga una lista de componentes updatables... de lo contrario si 
        /// paso un componente tipo x es posible que no tenga una lista de ese tipo y no lo pueda manejar.
        /// En esta caso que no se tenga una lista para manejar cierto tipos de componentes diferentes a 
        /// updateable o drawable se debe tirar una ArgumentException.
        /// Todo esto es Just in case.
        /// </summary>
        /// <param name="component"></param>
        void addComponent(IComponent component);

        /// <summary>
        /// Agrega a la lista de objetos dibujables un objeto que no necesariamente es un componente.
        /// </summary>
        /// <param name="drawableObject"></param>
        void addDrawableOnly(IDrawableFE drawableObject);

        /// <summary>
        /// Agrega a la lista de objectos a actualizar un objeto que no necesariamente es un componente.
        /// </summary>
        /// <param name="updateableObject"></param>
        void addUpdateableOnly(IUpdateableFE updateableObject);

        /// <summary>
        /// Remueve un componente tanto de la lista de dibujado como de la lista de objetos actualizables.
        /// </summary>
        /// <param name="component">Componente a remover</param>
        void removeComponent(IComponent component);

        /// <summary>
        /// Remueve un objeto de la lista de objetos actualizables.
        /// </summary>
        /// <param name="updateableObject">Objeto actualizable a remover</param>
        void removeUpdateable(IUpdateableFE updateableObject);

        /// <summary>
        /// Remueve un objeto de la lista de objetos dibujables.
        /// </summary>
        /// <param name="drawableObject">Objeto dibujable a remover</param>
        void removeDrawable(IDrawableFE drawableObject);

        /// <summary>
        /// Remueve todos los componentes de ambas listas cuyo dueño sea una entidad específica.
        /// </summary>
        /// <param name="entity">Entidad dueña de los componentes a remover.</param>
        void removeComponentsFromEntity(IEntity entity);
	}
}
