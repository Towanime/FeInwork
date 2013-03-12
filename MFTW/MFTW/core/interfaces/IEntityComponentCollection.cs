using System;
using System.Collections.Generic;
using System.Text;

namespace FeInwork.Core.Interfaces
{
    public interface IEntityComponentCollection
	{
        /// <summary>
        /// CONSULTAR CON GRUPO ANTES DE USAR ESTE METODO! AGREGA UN COMPONENTE 
        /// DEL MISMO TIPO QUE OTRO QUE YA EXISTA EN LA ENTIDAD, ASI SE PUEDEN TENER DOS TIPOS PARA UNA ENTIDAD.
        /// SOLO CASOS ESPECIALES!
        /// </summary>
        /// <param name="componet"></param>
        /// <param name="addAsException"></param>
		void addComponent(IComponent componet, bool addAsException);

        void addComponent(IComponent componet);

        IEnumerable<IComponent> ComponentList
        {
            get;
        }

		T find<T>() where T : IComponent;

        List<T> findByBaseClass<T>() where T : IComponent;

        bool remove<T>() where T : IComponent;

        bool remove(IComponent toRemove);

        bool removeNow<T>() where T : IComponent;

        bool removeNow(IComponent toRemove);

        void removeAll();

        bool contains<T>() where T : IComponent;

        bool contains(Type type);

        bool IsReadOnly
        {
            get;
            set;
        }
	}
}
