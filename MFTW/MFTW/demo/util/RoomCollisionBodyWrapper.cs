using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.collision.bodies;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.FeInwork.entities;
using Microsoft.Xna.Framework;

namespace FeInwork.FeInwork.util
{
    public class AreaCollisionBodyWrapper : IPropertyContainer
    {
        private CollisionBody body;
        private PropertyContainer propertyContainer;

        public CollisionBody Body
        {
            get { return this.body; }
        }

        public AreaCollisionBodyWrapper(AreaEntity roomOwner, CollisionBody body)
        {
            this.body = body;
            propertyContainer = new PropertyContainer(roomOwner);
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

        public bool containsState(int state)
        {
            return propertyContainer.containsState(state);
        }

        public int[] getPropertyList()
        {
            return propertyContainer.getPropertyList();
        }
    }
}
