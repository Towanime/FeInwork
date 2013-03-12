using System;
using System.Collections.Generic;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using FeInwork.Core.Events;
using FeInwork.Core.Util;
using FeInwork.FeInwork.util;
using FeInwork.core.events;
using Microsoft.Xna.Framework;

namespace FeInwork.Core.Base
{
    public class PropertyContainer : IPropertyContainer
    {

        // dueño de este container
        private IEntity owner;
        // propiedades de otro tipos 
        private Dictionary<int, object> properties;
        // para vector2
        private Dictionary<int, Vector2> vectorProperties;
        // ints
        private Dictionary<int, int> intProperties;
        // floats
        private Dictionary<int, float> floatProperties;
        // bools
        private Dictionary<int, bool> boolProperties;
        // states
        private Dictionary<int, bool> states;


        public PropertyContainer(IEntity owner)
        {
            this.owner = owner;
            initializer();
        }

        public void initializer()
        {
            // inicializar diccionarios
            properties = new Dictionary<int, object>();
            intProperties = new Dictionary<int, int>();
            floatProperties = new Dictionary<int, float>();
            boolProperties = new Dictionary<int, bool>();
            vectorProperties = new Dictionary<int, Vector2>();
            states = new Dictionary<int, bool>();
        }

        #region Property methods

        public T getProperty<T>(int property)
        {
            try
            {
                return (T)properties[property];
            }
            catch (Exception e)
            {
                throw new Exception("Propiedad " + property +
                    " no ha sido registrada para la entidad: " + owner.Id + " "+e.ToString());
            }            
        }

        public int getIntProperty(int property)
        {
            int value;
            if (intProperties.TryGetValue(property, out value))
            {
                return value;
            }
            else
            {
                throw new Exception("Propiedad " + property +
                    " no ha sido registrada para la entidad: " + owner.Id);
            }
        }
        
        public float getFloatProperty(int property)
        {
            float value;
            if (floatProperties.TryGetValue(property, out value))
            {
                return value;
            }
            else
            {
                throw new Exception("Propiedad " + property +
                    " no ha sido registrada para la entidad: " + owner.Id);
            }
        }

        public bool getBoolProperty(int property)
        {
            bool value;
            if (boolProperties.TryGetValue(property, out value))
            {
                return value;
            }
            else
            {
                throw new Exception("Propiedad " + property +
                    " no ha sido registrada para la entidad: " + owner.Id);
            }
        }

        public Vector2 getVectorProperty(int property)
        {
            Vector2 value;
            if (vectorProperties.TryGetValue(property, out value))
            {
                return value;
            }
            else
            {
                throw new Exception("Propiedad " + property +
                    " no ha sido registrada para la entidad: " + owner.Id);
            }
        }

        public void addProperty<T>(int property, T value)
        {
            if (!properties.ContainsKey(property))
            {
                // si todo bien se continua.
                properties.Add(property, value);
            }
        }

        public void addIntProperty(int property, int value)
        {
            if (!intProperties.ContainsKey(property))
            {
                // si todo bien se continua.
                intProperties.Add(property, value);
            }
        }

        public void addFloatProperty(int property, float value)
        {
            if (!floatProperties.ContainsKey(property))
            {
                // si todo bien se continua.
                floatProperties.Add(property, value);
            }
        }

        public void addBoolProperty(int property, bool value)
        {
            if (!boolProperties.ContainsKey(property))
            {
                // si todo bien se continua.
                boolProperties.Add(property, value);
            }
        }

        public void addVectorProperty(int property, Vector2 value)
        {
            if (!vectorProperties.ContainsKey(property))
            {
                // si todo bien se continua.
                vectorProperties.Add(property, value);
            }
        }

        public bool containsProperty(int property)
        {
            return properties.ContainsKey(property);
        }

        public bool containsIntProperty(int property)
        {
            return intProperties.ContainsKey(property);
        }

        public bool containsFloatProperty(int property)
        {
            return floatProperties.ContainsKey(property);
        }

        public bool containsBoolProperty(int property)
        {
            return boolProperties.ContainsKey(property);
        }

        public bool containsVectorProperty(int property)
        {
            return vectorProperties.ContainsKey(property);
        }

        public bool containsState(int state)
        {
            return states.ContainsKey(state);
        }

        public void changeProperty<T>(int property, T newValue, bool notify)
        {
            object old = null;
            // verifica si es null
            if (newValue == null)
            {
                throw new ArgumentNullException( "El valor por defecto para la propiedad agregada no debe ser null. Property: " + property);
            }
            // todo bien? move on, valida que sea nuevo valor diferente al actual
            if (properties.TryGetValue(property, out old))
            {
                if (!old.Equals(newValue))
                {
                    // si esta en el diccionario se cambia
                    properties[property] = newValue;
                    // enviar evento
                    if (notify)
                    {
                        EventManager.Instance.fireEvent(PropertyChangeEvent.Create(this.owner, property, old, newValue));
                    }
                }
            }
            else
            {
                // Si no existe entonces se añade con su valor por defecto
                // y se vuelve a llamar a este metodo  
                addProperty<T>(property, newValue);
            }
        }

        public void changeIntProperty(int property, int newValue, bool notify)
        {
            int old = 0;
            if (intProperties.TryGetValue(property, out old))
            {
                if (!old.Equals(newValue))
                {
                    // si esta en el diccionario se cambia
                    intProperties[property] = newValue;
                    // enviar evento
                    if (notify)
                    {
                        EventManager.Instance.fireEvent(PropertyChangeEvent.Create(this.owner, property, old, newValue));
                    }
                }
            }
            else
            {
                // Si no existe entonces se añade con su valor por defecto
                // y se vuelve a llamar a este metodo  
                addIntProperty(property, newValue);
            }
        }

        public void changeFloatProperty(int property, float newValue, bool notify)
        {
            float old;
            if (floatProperties.TryGetValue(property, out old))
            {
                if (!old.Equals(newValue))
                {
                    // si esta en el diccionario se cambia
                    floatProperties[property] = newValue;
                    // enviar evento
                    if (notify)
                    {
                        EventManager.Instance.fireEvent(PropertyChangeEvent.Create(this.owner, property, old, newValue));
                    }
                }
            }
            else
            {
                // Si no existe entonces se añade con su valor por defecto
                // y se vuelve a llamar a este metodo  
                addFloatProperty(property, newValue);
            }
        }

        public void changeBoolProperty(int property, bool newValue, bool notify)
        {
            bool old;
            if (boolProperties.TryGetValue(property, out old))
            {
                if (!old.Equals(newValue))
                {
                    // si esta en el diccionario se cambia
                    boolProperties[property] = newValue;
                    // enviar evento
                    if (notify)
                    {
                        EventManager.Instance.fireEvent(PropertyChangeEvent.Create(this.owner, property, old, newValue));
                    }
                }
            }
            else
            {
                // Si no existe entonces se añade con su valor por defecto
                // y se vuelve a llamar a este metodo  
                addBoolProperty(property, newValue);
            }
        }

        public void changeVectorProperty(int property, Vector2 newValue, bool notify)
        {
            Vector2 old;
            if (vectorProperties.TryGetValue(property, out old))
            {
                if(!old.Equals(newValue))
                {
                    // si esta en el diccionario se cambia
                    vectorProperties[property] = newValue;
                    // enviar evento
                    if (notify)
                    {
                        EventManager.Instance.fireEvent(PropertyChangeEvent.Create(this.owner, property, old, newValue));
                    }
                }      
            }
            else
            {
                // Si no existe entonces se añade con su valor por defecto
                // y se vuelve a llamar a este metodo  
                addVectorProperty(property, newValue);
            }
        }

        public bool removeProperty(int property)
        {
            // not yet!
            throw new NotImplementedException();
        }

        public int[] getPropertyList()
        {
            int totalCount = properties.Keys.Count + intProperties.Keys.Count + floatProperties.Keys.Count + boolProperties.Keys.Count + vectorProperties.Keys.Count;
            int[] keys = new int[totalCount];
            properties.Keys.CopyTo(keys, 0);
            intProperties.Keys.CopyTo(keys, properties.Count);
            floatProperties.Keys.CopyTo(keys, intProperties.Count);
            boolProperties.Keys.CopyTo(keys, floatProperties.Count);
            vectorProperties.Keys.CopyTo(keys, boolProperties.Count);
            return keys;
        }

        #endregion Property methods end

        #region States methods

        public void addState(int state)
        {
            if (!states.ContainsKey(state))
            {
                // si todo bien se continua.
                states.Add(state, false);
            }
        }

        public void addState(int state, bool value)
        {
            if (!states.ContainsKey(state))
            {
                // si todo bien se continua.
                states.Add(state, value);
            }
        }

        public bool getState(int state)
        {
            bool value;
            if (states.TryGetValue(state, out value))
            {
                return value;
            }
            else
            {
                throw new Exception("Estado" + state + " no ha sido registrado para la entidad: " + owner.Id);
            }
        }

        public void changeState(int state, bool newValue, bool notify)
        {
            bool old;
            if (states.TryGetValue(state, out old))
            {
                if (!old.Equals(newValue))
                {
                    // si esta en el diccionario se cambia
                    states[state] = newValue;
                    // enviar evento
                    if (notify)
                    {
                        EventManager.Instance.fireEvent(PropertyChangeEvent.Create(this.owner, state, old, newValue));
                    }
                }
            }
            else
            {
                // Si no existe entonces se añade con su valor por defecto
                // y se vuelve a llamar a este metodo  
                addState(state, newValue);
            }
        }

        public bool removeState(int state)
        {
            // not yet!
            throw new NotImplementedException();
        }

        public int[] getStateList()
        {
            int[] keys = new int[states.Keys.Count];
            states.Keys.CopyTo(keys, 0);
            return keys;
        }

        #endregion States methods end
    }
}
