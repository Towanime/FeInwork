using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;

namespace FeInwork.Core.Base
{
    public class EntityComponentCollection : IEntityComponentCollection
    {
        // diccionario de componentes, no se pueden repetir!
        // la key es el Type de dicho componente a agregar
        private Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
        /// <summary>
        /// Esta lista es para excepciones especiales! consultar antes de agregar algo a esta lista!
        /// Es para guardar refs de componentes que no se puedan agregar en el principal coz ya uno de esta clase este agregado...
        /// Sirve por ejemplo para agregar varios parallax effect a un cuarto.
        /// </summary>
        private List<IComponent> duplicatedComponents = new List<IComponent>();
        /// <summary>
        /// Especifica que esta entidad no puede ser modificada agregando componentes!
        /// Esta variable se puede poner en true solo luego de registrar todos los 
        /// componetes, de lo contrario al agregar uno se lanzara una excepcion.
        /// </summary>
        protected bool isReadOnly;

        public IEnumerable<IComponent> ComponentList
        {
            get
            {
                IEnumerable<IComponent> s = this.components.Values;
                if (s != null && duplicatedComponents.Count > 0)
                {
                    return s.Concat(duplicatedComponents.AsEnumerable<IComponent>());
                }
                return s;
            }
        }

        public void addComponent(IComponent component, bool addAsException)
        {
            if (IsReadOnly)
            {
                throw new ArgumentException("Esta entidad no puede ser modificada, es read-only.");
            }

            // si ya tiene un componente del mismo tipo del que se quiere agregar, se verifica para agregar exception
            if (components.ContainsKey(component.GetType()))
            {

                if (addAsException)
                {
                    duplicatedComponents.Add(component);
                    return;
                }
                else
                {
                    return;
                }
            }
            // si llega hasta aqui entonces no existe en esta lista y se agrega el comp
            components.Add(component.GetType(), component);
        }

        /// <summary>
        /// Constructor para agregar un componente, por default no se sobrescribe
        /// </summary>
        /// <param name="component"></param>
        public void addComponent(IComponent component)
        {
            if (IsReadOnly)
            {
                throw new ArgumentException("Esta entidad no puede ser modificada, es read-only.");
            }
            // si ya tiene un componente del mismo tipo del que se quiere agregar, se verifica para sobreescribir
            if (components.ContainsKey(component.GetType()))
            {   //Default false
                throw new Exception("Solo puede existir un componente del mismo tipo para una entidad.");
            }
            // si llega hasta aqui entonces no existe en esta lista y se agrega el comp
            components.Add(component.GetType(), component);
        }

        /// <summary>
        /// Retorna un componente dependiendo del tipo que se especifique al momento 
        /// de llamar este metodo.
        /// NO busca en lista de duplicados!
        /// </summary>
        /// <typeparam name="T">Clase del componente a buscar.</typeparam>
        /// <returns></returns>
        public T find<T>() where T : IComponent
        {
            Type type = typeof(T);

            if (components.ContainsKey(type))
            {
                return (T)components[type];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Encuentra una lista de los componentes que sean subclase de la clase indicada al
        /// llamar al metodo.
        /// NO busca en duplicados!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> findByBaseClass<T>() where T : IComponent
        {
            List<T> list = new List<T>();
            foreach (KeyValuePair<Type,IComponent> entry in components)
            {
                if (entry.Key.IsSubclassOf(typeof(T)))
                {
                    list.Add((T)entry.Value);
                }
            }
            return list;
        }

        public bool contains<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }

        public bool contains(Type type)
        {
            throw new NotImplementedException();
        }

        public bool remove<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }

        public bool removeNow<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }

        public bool remove(IComponent toRemove)
        {
            throw new NotImplementedException();
        }

        public bool removeNow(IComponent toRemove)
        {
            return this.components.Remove(toRemove.GetType());
        }

        public void removeAll()
        {
            this.components.Clear();
        }

        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
            set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                }
            }
        }
    }
}
