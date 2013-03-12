using System;
using System.Collections.Generic;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.core.interfaces;
using FeInwork.Core.Util;
using FeInwork.core.managers;
using FeInwork.FeInwork.util;
using Microsoft.Xna.Framework;

namespace FeInwork.Core.Base
{
	public class Entity : EntityComponentCollection, IEntity
	{
        private PropertyContainer propertyContainer;
        private string id;
        private int hashCode;

        public Entity(string entityId)
        {
            this.id = entityId;
            EntityManager.Instance.addEntity(this);
            propertyContainer = new PropertyContainer(this);
        }

        public Entity()
        {
            this.id = EntityManager.Instance.generateId();
            EntityManager.Instance.addEntity(this);
            propertyContainer = new PropertyContainer(this);
        }

        public virtual void initialize()
        {

        }

        public string Id
        {
            get { return id;  }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            Entity entity = (Entity)obj;

            return this.Id.Equals(entity.Id);
        }

        public sealed override int GetHashCode()
        {
            const int multiplier = 23;
            if (hashCode == 0)
            {
                int code = 27;
                code = multiplier * code + id.GetHashCode();
                code = multiplier * code;
                hashCode = code;
            }
            return hashCode;
        }

        #region IPropertyContainer Propiedades, sirve como proxy para actuar de manera mas trasparente con las props de una entidad.

        public T getProperty<T>(int property)
        {
            return propertyContainer.getProperty<T>(property);
        }

        public int getIntProperty(int property)
        {
            return propertyContainer.getIntProperty(property);
        }

        public float getFloatProperty(int property)
        {
            return propertyContainer.getFloatProperty(property);
        }

        public bool getBoolProperty(int property)
        {
            return propertyContainer.getBoolProperty(property);
        }

        public Microsoft.Xna.Framework.Vector2 getVectorProperty(int property)
        {
            return propertyContainer.getVectorProperty(property);
        }

        public void addProperty<T>(int property, T value)
        {
            propertyContainer.addProperty<T>(property, value);
        }

        public void addIntProperty(int property, int value)
        {
            propertyContainer.addIntProperty(property, value);
        }

        public void addFloatProperty(int property, float value)
        {
            propertyContainer.addFloatProperty(property, value);
        }

        public void addBoolProperty(int property, bool value)
        {
            propertyContainer.addBoolProperty(property, value);
        }

        public void addVectorProperty(int property, Microsoft.Xna.Framework.Vector2 value)
        {
            propertyContainer.addVectorProperty(property, value);
        }

        public void changeProperty<T>(int property, T newValue, bool notify)
        {
            propertyContainer.changeProperty<T>(property, newValue, notify);
        }

        public void changeIntProperty(int property, int newValue, bool notify)
        {
            propertyContainer.changeIntProperty(property, newValue, notify);
        }

        public void changeFloatProperty(int property, float newValue, bool notify)
        {
            propertyContainer.changeFloatProperty(property, newValue, notify);
        }

        public void changeBoolProperty(int property, bool newValue, bool notify)
        {
            propertyContainer.changeBoolProperty(property, newValue, notify);
        }

        public void changeVectorProperty(int property, Vector2 newValue, bool notify)
        {
            propertyContainer.changeVectorProperty(property, newValue, notify);
        }

        public bool removeProperty(int property)
        {
            return propertyContainer.removeProperty(property);
        }

        public bool containsProperty(int property)
        {
            return propertyContainer.containsProperty(property);
        }

        public bool containsIntProperty(int property)
        {
            return propertyContainer.containsIntProperty(property);
        }

        public bool containsFloatProperty(int property)
        {
            return propertyContainer.containsFloatProperty(property);
        }

        public bool containsBoolProperty(int property)
        {
            return propertyContainer.containsBoolProperty(property);
        }

        public bool containsVectorProperty(int property)
        {
            return propertyContainer.containsVectorProperty(property);
        }

        public int[] getPropertyList()
        {
            return propertyContainer.getPropertyList();
        }

        #endregion

        #region IPropertyContainer States, Este es otro proxy para estados

        public void addState(int state)
        {
            propertyContainer.addState(state);
        }

        public void addState(int state, bool value)
        {
            propertyContainer.addState(state, value);
        }

        public bool getState(int state)
        {
            return propertyContainer.getState(state);
        }

        public void changeState(int state, bool newValue, bool notify)
        {
            propertyContainer.changeState(state, newValue, notify);
        }

        public bool removeState(int state)
        {
            return propertyContainer.removeState(state);
        }

        public bool containsState(int state)
        {
            return propertyContainer.containsState(state);
        }

        public int[] getStateList()
        {
            return propertyContainer.getStateList();
        }

        #endregion
    }
}
