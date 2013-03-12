using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.FeInwork.util;
using FeInwork.Core.Base;
using FeInwork.FeInwork.entities;
using FeInwork.core.collision.bodies;
using FeInwork.core.managers;
using FeInwork.Core.Interfaces;

namespace FeInwork.FeInwork.components
{
    public class AreaCollisionComponent : IComponent
    {
        private AreaEntity owner;
        private List<AreaCollisionBodyWrapper> wrapperList;
        private Dictionary<int, List<AreaCollisionBodyWrapper>> propertyWrapperCollection;


        public AreaCollisionComponent(AreaEntity owner)
        {
            this.owner = owner;
            this.wrapperList = new List<AreaCollisionBodyWrapper>();
            this.propertyWrapperCollection = new Dictionary<int, List<AreaCollisionBodyWrapper>>();
            this.initialize();
        }

        public void addBody(CollisionBody body)
        {
            wrapperList.Add(new AreaCollisionBodyWrapper(owner, body));
            CollisionManager.Instance.addContainer(body);
        }

        public void addBodies(List<CollisionBody> bodies)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                this.addBody(bodies[i]);
            }
        }

        public void addWrapper(AreaCollisionBodyWrapper wrapper)
        {
            wrapperList.Add(wrapper);
            int[] wrapperProperties = wrapper.getPropertyList();
            for (int propertyIndex = 0; propertyIndex < wrapperProperties.Length; propertyIndex++)
            {
                int currentProperty = wrapperProperties[propertyIndex];
                if (!propertyWrapperCollection.ContainsKey(currentProperty))
                {
                    propertyWrapperCollection.Add(currentProperty, new List<AreaCollisionBodyWrapper>());
                }
                propertyWrapperCollection[currentProperty].Add(wrapper);
            }
            CollisionManager.Instance.addContainer(wrapper.Body);
        }

        public void addWrappers(List<AreaCollisionBodyWrapper> wrappers)
        {
            for (int i = 0; i < wrappers.Count; i++)
            {
                this.addWrapper(wrappers[i]);
            }
        }

        public List<AreaCollisionBodyWrapper> getWrappersWithProperty(int property)
        {
            List<AreaCollisionBodyWrapper> wrappersToReturn = null;
            propertyWrapperCollection.TryGetValue(property, out wrappersToReturn);
            if (wrappersToReturn == null) return new List<AreaCollisionBodyWrapper>();
            return wrappersToReturn;
        }

        public List<CollisionBody> getBodiesWithProperty(int property)
        {
            List<AreaCollisionBodyWrapper> wrappersToReturn = null;
            List<CollisionBody> bodiesToReturn = new List<CollisionBody>();
            propertyWrapperCollection.TryGetValue(property, out wrappersToReturn);
            if (wrappersToReturn == null) return bodiesToReturn;
            for (int i = 0; i < wrappersToReturn.Count; i++)
            {
                bodiesToReturn.Add(wrappersToReturn[i].Body);
            }
            return bodiesToReturn;
        }

        public void initialize()
        {
            for (int wrapperIndex = 0; wrapperIndex < wrapperList.Count; wrapperIndex++)
            {
                CollisionManager.Instance.addContainer(wrapperList[wrapperIndex].Body);
            }
            Program.GAME.ComponentManager.addComponent(this);
        }

        public IEntity Owner
        {
            get { return this.owner; }
        }
    }
}
