using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Events;
using FeInwork.Core.Listeners;
using FeInwork.Core.Util;
using System.Reflection;
using FeInwork.core.Base;
using FeInwork.FeInwork.listeners;
using FeInwork.FeInwork.events;
using FeInwork.core.collision.bodies;
using Microsoft.Xna.Framework;
using FeInwork.core.util;
using FeInwork.FeInwork.util;
using FeInwork.core.events;

namespace FeInwork.Core.Managers
{
    public class EventManager
    {
        private const string reflectionString = "invoke";
        /// <summary>
        /// Arreglo que contendra el objeto evento usado
        /// en el metodo tryInvokeEvent
        /// </summary>
        private Object[] eventObjectArray;
        /// <summary>
        /// Arreglo que contendra el tipo de argumento del
        /// metodo usado en el metodo tryInvokeEvent
        /// </summary>
        private Type[] methodTypeArray;
        /// <summary>
        /// Enum para diferencias en que lista buscar a la hora de activar o desactivar un listener.
        /// </summary>
        public enum ListenerTypes
        {
            PropertyListeners,
            StateListeners,
            CollisionListeners,
            GeneralListeners
        }
        private static EventManager instance;
        /// <summary>
        /// Listeners generales sin filtro. 
        /// </summary>
        private Dictionary<int, List<EventListenerWrapper>> listeners;
        /// <summary>
        /// Filtrados por entidad y tipo de evento.
        /// </summary>
        private Dictionary<int, Dictionary<IEntity, List<EventListenerWrapper>>> filteredGeneralListeners;
        /// <summary>
        /// Listeners de propiedades sin filtrado solamente or la propiedad que quieren escuchar. 
        /// </summary>
        private Dictionary<int, List<EventListenerWrapper>> propertyListeners;
        /// <summary>
        /// Filtrados por entidad y tipo de propiedad.
        /// </summary>
        private Dictionary<int, Dictionary<IEntity, List<EventListenerWrapper>>> filteredPropertyListeners;
        /// <summary>
        /// Listeners de estado sin filtros.
        /// </summary>
        private Dictionary<int, List<EventListenerWrapper>> entityStateListeners;
        /// <summary>
        /// Filtrados por entidad y estado.
        /// </summary>
        private Dictionary<int, Dictionary<IEntity, List<EventListenerWrapper>>> filteredStateListeners;
        /// <summary>
        /// nueva lista de listenes de colisiones por entidad
        /// </summary>
        private Dictionary<IEntity, List<EventListenerWrapper>> collisionListeners;
        /// <summary>
        /// Pool de listas de eventos para utilizar
        /// </summary>
        private Dictionary<int, List<AbstractEvent>> eventPool;
        /// <summary>
        /// Pool de Wrappers de Listeners para utilizar
        /// </summary>
        private List<EventListenerWrapper> wrapperPool = new List<EventListenerWrapper>();
        /// <summary>
        /// Diccionario utilizado para guardar y reutilizar 
        /// MethodInfo para mejorar rendimiento a la hora de utilizar reflection.
        /// El key es el tipo de listener que invocara el metodo y el value
        /// es un par de tipo de evento y objeto invoke listo para utilizar.
        /// </summary>
        private Dictionary<Type, Dictionary<Type, MethodInfo>> reflectionMethods;

        private EventManager()
        {
            initialize();
        }

        private void initialize()
        {
            eventObjectArray = new Object[1];
            methodTypeArray = new Type[1];
            // esto supuestamente ayuda
            listeners = new Dictionary<int, List<EventListenerWrapper>>();
            filteredGeneralListeners = new Dictionary<int, Dictionary<IEntity, List<EventListenerWrapper>>>();
            propertyListeners = new Dictionary<int, List<EventListenerWrapper>>();
            filteredPropertyListeners = new Dictionary<int, Dictionary<IEntity, List<EventListenerWrapper>>>();
            entityStateListeners = new Dictionary<int, List<EventListenerWrapper>>();
            filteredStateListeners = new Dictionary<int, Dictionary<IEntity, List<EventListenerWrapper>>>();
            collisionListeners = new Dictionary<IEntity, List<EventListenerWrapper>>();
            eventPool = new Dictionary<int, List<AbstractEvent>>();
            wrapperPool = new List<EventListenerWrapper>();
            reflectionMethods = new Dictionary<Type, Dictionary<Type, MethodInfo>>();
        }

        /// <summary>
        /// Obtiene un objeto evento de su pool respectivo
        /// según su tipo para ser reutilizado y evitar crear
        /// nuevas instancias de ese tipo de eventos
        /// </summary>
        /// <typeparam name="T">Clase del tipo de evento que tenga 
        /// como clase base AbstractEvent</typeparam>
        /// <param name="eventType">Tipo de evento</param>
        /// <returns>Evento obtenido del pool o nulo en caso de que no encuentre ninguno</returns>
        public T GetEventFromType<T>(int eventType) where T : AbstractEvent
        {
            if (eventPool.ContainsKey(eventType))
            {
                for (int i = 0; i < eventPool[eventType].Count; i++)
                {
                    if (eventPool[eventType][i].IsAvailable)
                    {
                        return (T)eventPool[eventType][i];
                    }
                }
            }
            return default(T);
        }

        /// <summary>
        /// Agrega un nuevo objeto evento a su pool de eventos respectivo.
        /// Este evento solo debe llamarse en caso de que el metodo 
        /// "GetEventFromType" retorne null, de otra forma debe usarse la instancia
        /// obtenida por ese evento.
        /// </summary>
        /// <typeparam name="T">Clase del tipo de evento que tenga 
        /// como clase base AbstractEvent</typeparam>
        /// <param name="eventObject">Evento a agregar al pool</param>
        /// <returns>El evento agregado</returns>
        public T AddEventToPool<T>(T eventObject) where T : AbstractEvent
        {
            if (!eventPool.ContainsKey(eventObject.Type))
            {
                eventPool.Add(eventObject.Type, new List<AbstractEvent>());
            }
            eventPool[eventObject.Type].Add(eventObject);
            return eventObject;
        }

        /// <summary>
        /// Dispara un evento a los listeners intersados, si un listener esta filtrando por entidad
        /// es necesario tener cuidado que cuando creen el objeto event se especifique como origen
        /// una clase tipo IComponent/BaseComponent o una IEntity, de lo contrario sera ignorados los 
        /// listeners especificos.
        /// </summary>
        /// <param name="eventObject"></param>
        public void fireEvent(IEvent eventObject)
        {
            // sacar los interesados
            List<EventListenerWrapper> targets = null;
            // ve si tiene listeners interesados sin filtro
            if (listeners.TryGetValue(eventObject.Type, out targets))
            {
                // sacar los interesados
                // List<EventListenerWrapper> targets = listeners[eventObject.Type];
                for (int i = 0; i < targets.Count; i++)
                {
                    EventListenerWrapper wrapper = targets[i];
                    // se notifica uno por uno
                    // verifica si se filtra o no
                    tryInvokeEvent(wrapper.listener, eventObject);
                }
            }
            // ahora llama a los eventos que esten filtrando por entidad
            // sacar primero la entidad a buscar

            Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
            IComponent comp = null;
            // entidad a buscar
            IEntity entity = null;
            if ((comp = eventObject.Origin as IComponent) != null)
            {
                // asigna la entidad
                entity = comp.Owner;
            }
            else
            {
                // un ultimo intento
                entity = eventObject.Origin as IEntity;
            }
            if (entity != null && filteredGeneralListeners.TryGetValue(eventObject.Type, out dic))
            {
                List<EventListenerWrapper> toInvoke = null;
                if (dic.TryGetValue(entity, out toInvoke))
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        tryInvokeEvent(toInvoke[i].listener, eventObject);
                    }
                }
            }
        }

        // metodo overloaded para ser mas especificos con los de tipo property change 
        // y darle un mejor trato!
        public void fireEvent(PropertyChangeEvent eventObject)
        {
            List<EventListenerWrapper> targets = null;
            if (propertyListeners.TryGetValue(eventObject.PropertyName, out targets))
            {
                // sacar los interesados
                // List<EventListenerWrapper> targets = propertyListeners[eventObject.PropertyName];
                for (int i = 0; i < targets.Count; i++)
                {
                    EventListenerWrapper wrapper = targets[i];
                    // se notifica uno por uno
                    // verifica si se filtra o no
                    ((PropertyChangeListener)wrapper.listener).invoke(eventObject);
                }
            }
            // ahora llama a los eventos que esten filtrando por entidad
            // sacar primero la entidad a buscar

            Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
            IComponent comp = null;
            // entidad a buscar
            IEntity entity = null;
            if ((comp = eventObject.Origin as IComponent) != null)
            {
                // asigna la entidad
                entity = comp.Owner;
            }
            else
            {
                // un ultimo intento
                entity = eventObject.Origin as IEntity;
            }
            if (entity != null && filteredPropertyListeners.TryGetValue(eventObject.PropertyName, out dic))
            {
                List<EventListenerWrapper> toInvoke = null;
                if (dic.TryGetValue(entity, out toInvoke))
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        ((PropertyChangeListener)toInvoke[i].listener).invoke(eventObject);
                    }
                }
            }
        }

        /// <summary>
        /// Envia un evento de cambio de estado.
        /// </summary>
        /// <param name="eventObject"></param>
        public void fireEvent(StateChangeEvent eventObject)
        {

            // sacar los interesados
            List<EventListenerWrapper> targets = null;
            if (entityStateListeners.TryGetValue(eventObject.State, out targets))
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    EventListenerWrapper wrapper = targets[i];
                    // se notifica uno por uno
                    // verifica si se filtra o no
                    ((StateChangeListener)wrapper.listener).invoke(eventObject);
                }
            }
            // ahora llama a los eventos que esten filtrando por entidad
            // sacar primero la entidad a buscar

            Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
            IComponent comp = null;
            // entidad a buscar
            IEntity entity = null;
            if ((comp = eventObject.Origin as IComponent) != null)
            {
                // asigna la entidad
                entity = comp.Owner;
            }
            else
            {
                // un ultimo intento
                entity = eventObject.Origin as IEntity;
            }
            if (entity != null && filteredStateListeners.TryGetValue(eventObject.State, out dic))
            {
                List<EventListenerWrapper> toInvoke = null;
                if (dic.TryGetValue(entity, out toInvoke))
                {
                    for (int i = 0; i < toInvoke.Count; i++)
                    {
                        ((StateChangeListener)toInvoke[i].listener).invoke(eventObject);
                    }
                }
            }
        }

        /// <summary>
        /// Envia un evento de colision.
        /// </summary>
        /// <param name="collisionEvent"></param>
        public void fireEvent(CollisionEvent collisionEvent)
        {
            // realiza proceso para las dos afectadas
            List<EventListenerWrapper> triggeringEntityWrappers = null;
            if (collisionListeners.TryGetValue(collisionEvent.TriggeringEntity, out triggeringEntityWrappers))
            {
                invokeCollisions(collisionEvent, collisionEvent.CollisionResult.triggeringBody, triggeringEntityWrappers, collisionEvent.CollisionResult.affectedBody.ColorTag);
            }
            // realiza proceso para las dos afectadas
            List<EventListenerWrapper> affectedEntityWrappers = null;
            if (collisionListeners.TryGetValue(collisionEvent.AffectedEntity, out affectedEntityWrappers))
            {
                invokeCollisions(collisionEvent, collisionEvent.CollisionResult.affectedBody, affectedEntityWrappers, collisionEvent.CollisionResult.triggeringBody.ColorTag);
            }
        }

        /// <summary>
        /// Trata de invocar un metodo invoke del listener usando reflection.
        /// </summary>
        /// <param name="listenerToCall"></param>
        /// <param name="eventObject"></param>
        private void tryInvokeEvent(IEventListener listenerToCall, IEvent eventObject)
        {
            Type listenerType = listenerToCall.GetType();
            Type eventType = eventObject.GetType();

            MethodInfo metodo = null;
            Dictionary<Type, MethodInfo> listenerInvokes = null;

            // Se intenta reutilizar el objeto MethodInfo para hacer el invoke
            if (!reflectionMethods.TryGetValue(listenerType, out listenerInvokes))
            {
                listenerInvokes = new Dictionary<Type, MethodInfo>();
                reflectionMethods.Add(listenerType, listenerInvokes);
            }

            if (!listenerInvokes.TryGetValue(eventType, out metodo))
            {
                // En caso de que no exista, se crea uno nuevo y se agrega al dictionary
                methodTypeArray[0] = eventType;
                metodo = listenerType.GetMethod(reflectionString, methodTypeArray);
                listenerInvokes.Add(eventType, metodo);
            }

            if (metodo != null)
            {
                eventObject.IsAvailable = false;
                eventObjectArray[0] = eventObject;
                metodo.Invoke(listenerToCall, eventObjectArray);
                eventObject.IsAvailable = true;
            }
            else
            {
                // si el metodo es nulo no tiene ninguna implementacion y no deberia esta aqui!
                throw new Exception("No metan Listeners sin tener metodos invokes!");
            }
        }

        /// <summary>
        /// Metodo para logica de como se envian los eventos de colision a los involucrados.
        /// </summary>
        /// <param name="collisionEvent"></param>
        /// <param name="collisionBody"></param>
        /// <param name="listeners"></param>
        /// <param name="colorTag"></param>
        private void invokeCollisions(CollisionEvent collisionEvent, CollisionBody collisionBody, List<EventListenerWrapper> listeners, Color? colorTag)
        {
            // recorrer lista de listener e invocar las generales(id vacia) y las que tengan id especifico.
            for (int i = 0; i < listeners.Count; i++)
            {
                EventListenerWrapper listener = listeners[i];
                // escucha a todos los bodys o a uno en particular?
                if (listener.entityToListenId == null || listener.entityToListenId == collisionBody.Id)
                {

                    CollisionListener collisionListener = ((CollisionListener)listener.listener);
                    // Ultimo chequeo para saber si el listener está clasificado por color,
                    // en caso de que no sea asi este aceptará cualquier evento que haya llegado hasta aca,
                    // de otra forma solo aceptará los eventos que traigan consigo el mismo color del listener.
                    if (collisionListener.ColorTag == null || collisionListener.ColorTag == colorTag)
                    {
                        collisionListener.invoke(collisionEvent);
                    }
                }
            }
        }

        /// <summary>
        /// Agrega un listener que escucha todos los eventos de cierto tipo sin importar su origen.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        public void addListener(int eventType, IEventListener listener)
        {
            List<EventListenerWrapper> list = null;
            // si no existe se agrega una nueva lista
            if (!listeners.TryGetValue(eventType, out list))
            {
                list = new List<EventListenerWrapper>();
                listeners.Add(eventType, list);
            }
            // se agrega
            //listeners[eventType].Add(listener);
            // se crea como listener sin filtro
            IComponent comp = null;
            EventListenerWrapper wrapper = null;
            string ownerId = ((comp = listener as IComponent) != null) ? comp.Owner.Id : null;

            if (wrapperPool.Count == 0)
            {
                wrapper = new EventListenerWrapper(listener, ownerId);
            }
            else
            {
                wrapper = wrapperPool[0];
                wrapper.reset();
                wrapper.listener = listener;
                wrapper.ownerEntityId = ownerId;
                wrapperPool.RemoveAt(0);
            }

            list.Add(wrapper);
        }

        /// <summary>
        /// Agrega un listener que escucha solo ciertos eventos originados de cierta entidad.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="entityToListen"></param>
        /// <param name="listener"></param>
        public void addListener(int eventType, IEntity entityToListen, IEventListener listener)
        {
            if (entityToListen == null)
            {
                throw new InvalidOperationException("La entidad a la cual suscribirse no debe ser nula");
            }

            //addListener(eventType, entityToListen.Id, listener);
            // obtiene diccionario de entidades para ese tipo de evento
            Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
            if (!filteredGeneralListeners.TryGetValue(eventType, out dic))
            {
                // si no existe se crea un nuevo dic y  lista de listeners
                dic = new Dictionary<IEntity, List<EventListenerWrapper>>();
                filteredGeneralListeners.Add(eventType, dic);
            }
            List<EventListenerWrapper> list = null;
            // ahora que ya tiene el dic trata de sacar la lsita donde se agregara el listener
            if (!dic.TryGetValue(entityToListen, out list))
            {
                // si no existe se crea la lista y agrega
                list = new List<EventListenerWrapper>();
                dic.Add(entityToListen, list);
            }
            // agregar listener a la lista que se encontro o ya acaba de ser creada
            IComponent comp = null;
            EventListenerWrapper wrapper = null;
            string ownerId = ((comp = listener as IComponent) != null) ? comp.Owner.Id : null;

            if (wrapperPool.Count == 0)
            {
                wrapper = new EventListenerWrapper(listener, ownerId, entityToListen.Id);
            }
            else
            {
                wrapper = wrapperPool[0];
                wrapper.reset();
                wrapper.listener = listener;
                wrapper.ownerEntityId = ownerId;
                wrapper.entityToListenId = entityToListen.Id;
                wrapperPool.RemoveAt(0);
            }

            list.Add(wrapper);
        }

        /// <summary>
        /// Agrega un listener que escucha solo ciertos eventos originados de cierta entidad.
        /// Este metodo es mas directo si se tiene el id especifico de una entidad.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="entityToListen"></param>
        /// <param name="listener"></param>
        /* public void addListener(int eventType, string entityToListen, IEventListener listener)
         {
             List<EventListenerWrapper> list = null;
             // si no existe se agrega una nueva lista
             if (!listeners.TryGetValue(eventType, out list))
             {
                 list =new List<EventListenerWrapper>();
                 listeners.Add(eventType, list);
             }
             // se agrega
             //listeners[eventType].Add(listener);
             // se crea como listener sin filtro
             IComponent comp = null;
             if ((comp = listener as IComponent) != null)
             {
                 list.Add(new EventListenerWrapper(listener, comp.Owner.Id, entityToListen));
             }
             else
             {
                 list.Add(new EventListenerWrapper(listener, null, entityToListen));
             }
         }*/

        /// <summary>
        /// Agrega un listener que escucha los eventos de una propiedad en particular sin filtrar por entidad.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        public void addPropertyListener(int property, PropertyChangeListener listener)
        {
            List<EventListenerWrapper> list = null;
            // si no existe se agrega una nueva lista
            if (!propertyListeners.TryGetValue(property, out list))
            {
                list = new List<EventListenerWrapper>();
                propertyListeners.Add(property, list);
            }
            // se agrega
            IComponent comp = null;
            EventListenerWrapper wrapper = null;
            string ownerId = ((comp = listener as IComponent) != null) ? comp.Owner.Id : null;

            if (wrapperPool.Count == 0)
            {
                wrapper = new EventListenerWrapper(listener, ownerId);
            }
            else
            {
                wrapper = wrapperPool[0];
                wrapper.reset();
                wrapper.listener = listener;
                wrapper.ownerEntityId = ownerId;
                wrapperPool.RemoveAt(0);
            }

            list.Add(wrapper);
        }

        /// <summary>
        /// Metodo para agregar un listener a una propiedad en particular y que filtre para una sola entidad.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="entityToListen"></param>
        /// <param name="listener"></param>
        public void addPropertyListener(int property, IEntity entityToListen, PropertyChangeListener listener)
        {
            //addListener(eventType, entityToListen.Id, listener);
            // obtiene diccionario de entidades para ese tipo de evento
            Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
            if (!filteredPropertyListeners.TryGetValue(property, out dic))
            {
                // si no existe se crea un nuevo dic y  lista de listeners
                dic = new Dictionary<IEntity, List<EventListenerWrapper>>();
                filteredPropertyListeners.Add(property, dic);
            }
            List<EventListenerWrapper> list = null;
            // ahora que ya tiene el dic trata de sacar la lsita donde se agregara el listener
            if (!dic.TryGetValue(entityToListen, out list))
            {
                // si no existe se crea la lista y agrega
                list = new List<EventListenerWrapper>();
                dic.Add(entityToListen, list);
            }
            // agregar listener a la lista que se encontro o ya acaba de ser creada
            IComponent comp = null;
            EventListenerWrapper wrapper = null;
            string ownerId = ((comp = listener as IComponent) != null) ? comp.Owner.Id : null;

            if (wrapperPool.Count == 0)
            {
                wrapper = new EventListenerWrapper(listener, ownerId, entityToListen.Id);
            }
            else
            {
                wrapper = wrapperPool[0];
                wrapper.reset();
                wrapper.listener = listener;
                wrapper.ownerEntityId = ownerId;
                wrapper.entityToListenId = entityToListen.Id;
                wrapperPool.RemoveAt(0);
            }

            list.Add(wrapper);
        }

        /// <summary>
        /// Metodo para agregar un listener a una propiedad en particular y que filtre para una sola entidad.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="entityToListen"></param>
        /// <param name="listener"></param>
        /*public void addPropertyListener(int property, string entityToListen, PropertyChangeListener listener)
        {
            List<EventListenerWrapper> list = null;
            // si no existe se agrega una nueva lista
            if (!propertyListeners.TryGetValue(property, out list))
            {
                list = new List<EventListenerWrapper>();
                propertyListeners.Add(property, list);
            }
            // se agrega
            IComponent comp = null;
            if ((comp = listener as IComponent) != null)
            {
                list.Add(new EventListenerWrapper(listener, comp.Owner.Id, entityToListen));
            }
            else
            {
                list.Add(new EventListenerWrapper(listener, null, entityToListen));
            }
        }*/

        // ----------     ESTADOS

        /// <summary>
        /// Agrega un listener que escucha los eventos de un estado en particular sin filtrar por entidad.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        public void addStateListener(int state, StateChangeListener listener)
        {
            List<EventListenerWrapper> list = null;
            // si no existe se agrega una nueva lista
            if (!entityStateListeners.TryGetValue(state, out list))
            {
                list = new List<EventListenerWrapper>();
                entityStateListeners.Add(state, list);
            }
            // se agrega
            IComponent comp = null;
            EventListenerWrapper wrapper = null;
            string ownerId = ((comp = listener as IComponent) != null) ? comp.Owner.Id : null;

            if (wrapperPool.Count == 0)
            {
                wrapper = new EventListenerWrapper(listener, ownerId);
            }
            else
            {
                wrapper = wrapperPool[0];
                wrapper.reset();
                wrapper.listener = listener;
                wrapper.ownerEntityId = ownerId;
                wrapperPool.RemoveAt(0);
            }

            list.Add(wrapper);
        }

        /// <summary>
        /// Metodo para agregar un listener a un estado en particular y que filtre para una sola entidad.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="entityToListen"></param>
        /// <param name="listener"></param>
        public void addStateListener(int state, IEntity entityToListen, StateChangeListener listener)
        {
            // obtiene diccionario de entidades para ese tipo de evento
            Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
            if (!filteredStateListeners.TryGetValue(state, out dic))
            {
                // si no existe se crea un nuevo dic y  lista de listeners
                dic = new Dictionary<IEntity, List<EventListenerWrapper>>();
                filteredStateListeners.Add(state, dic);
            }
            List<EventListenerWrapper> list = null;
            // ahora que ya tiene el dic trata de sacar la lsita donde se agregara el listener
            if (!dic.TryGetValue(entityToListen, out list))
            {
                // si no existe se crea la lista y agrega
                list = new List<EventListenerWrapper>();
                dic.Add(entityToListen, list);
            }
            // agregar listener a la lista que se encontro o ya acaba de ser creada
            IComponent comp = null;
            EventListenerWrapper wrapper = null;
            string ownerId = ((comp = listener as IComponent) != null) ? comp.Owner.Id : null;
            if (wrapperPool.Count == 0)
            {
                wrapper = new EventListenerWrapper(listener, ownerId, entityToListen.Id);
            }
            else
            {
                wrapper = wrapperPool[0];
                wrapper.reset();
                wrapper.listener = listener;
                wrapper.ownerEntityId = ownerId;
                wrapper.entityToListenId = entityToListen.Id;
                wrapperPool.RemoveAt(0);
            }

            list.Add(wrapper);
        }

        /// <summary>
        /// Metodo para agregar un listener a un estado en particular y que filtre para una sola entidad.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="entityToListen"></param>
        /// <param name="listener"></param>
        /*public void addStateListener(int state, string entityToListen, StateChangeListener listener)
        {
            List<EventListenerWrapper> list = null;
            // si no existe se agrega una nueva lista
            if (!entityStateListeners.TryGetValue(state, out list))
            {
                list = new List<EventListenerWrapper>();
                entityStateListeners.Add(state, list);
            }
            // se agrega
            IComponent comp = null;
            if ((comp = listener as IComponent) != null)
            {
                list.Add(new EventListenerWrapper(listener, comp.Owner.Id, entityToListen));
            }
            else
            {
                list.Add(new EventListenerWrapper(listener, null, entityToListen));
            }
        }*/

        // ----------     Estados end

        /// <summary>
        /// Metodo agrega un listener de colision a una entidad en particular.
        /// En este metodo el listener es ejecutado sin importar que objeto de colision de una 
        /// entidad lo genere, por lo tanto este son listeners basicos para todas las colisiones
        /// de una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="listener"></param>
        public void addCollisionListener(IEntity entity, CollisionListener listener)
        {
            List<EventListenerWrapper> list = null;
            // si no existe se agrega una nueva lista
            if (!collisionListeners.TryGetValue(entity, out list))
            {
                list = new List<EventListenerWrapper>();
                collisionListeners.Add(entity, list);
            }
            // se agrega
            EventListenerWrapper wrapper = null;
            if (wrapperPool.Count == 0)
            {
                wrapper = new EventListenerWrapper(listener, entity.Id);
            }
            else
            {
                wrapper = wrapperPool[0];
                wrapper.reset();
                wrapper.listener = listener;
                wrapper.ownerEntityId = entity.Id;
                wrapperPool.RemoveAt(0);
            }

            list.Add(wrapper);
        }

        /// <summary>
        /// Metodo agrega un listener a una entidad en particular y filtra por el objeto 
        /// que origina dicho evento.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="collisionObjectToListen"></param>
        /// <param name="listener"></param>
        public void addCollisionListener(IEntity entity, string collisionObjectToListen, CollisionListener listener)
        {
            List<EventListenerWrapper> list = null;
            // si no existe se agrega una nueva lista
            if (!collisionListeners.TryGetValue(entity, out list))
            {
                list = new List<EventListenerWrapper>();
                collisionListeners.Add(entity, list);
            }
            // se agrega
            EventListenerWrapper wrapper = null;
            if (wrapperPool.Count == 0)
            {
                wrapper = new EventListenerWrapper(listener, entity.Id, collisionObjectToListen);
            }
            else
            {
                wrapper = wrapperPool[0];
                wrapper.reset();
                wrapper.listener = listener;
                wrapper.ownerEntityId = entity.Id;
                wrapper.entityToListenId = collisionObjectToListen;
                wrapperPool.RemoveAt(0);
            }

            list.Add(wrapper);
        }

        public void activateListener(ListenerTypes listenerType, IEventListener iEventListener)
        {
            switch (listenerType)
            {
                case ListenerTypes.GeneralListeners:
                    // LOLZ esto es un culo so hacerlo lata!
                    break;
            }
        }


        /// <summary>
        /// Remueve de todos los listeners las referencias de una entidad 
        /// </summary>
        /// <param name="entity">Entidad a remover.</param>
        public void removeEntityFromListeners(IEntity entity)
        {
            removeEntityFromNormalListeners(entity);
            removeEntityFromPropertyListeners(entity);
            removeEntityFromStateListeners(entity);
            removeEntityFromCollisionListeners(entity);
        }

        /// <summary>
        /// Remueve de la lista de listeners regulares las referencias de una entidad.
        /// </summary>
        /// <param name="entity">Entidad a remover.</param>
        public void removeEntityFromNormalListeners(IEntity entity)
        {
            foreach (int eventType in listeners.Keys)
            {
                List<EventListenerWrapper> wrappers = listeners[eventType];

                for (int i = wrappers.Count - 1; i >= 0; i--)
                {
                    string ownerEntityId = wrappers[i].ownerEntityId;
                    string entityToListen = wrappers[i].entityToListenId;

                    if ((ownerEntityId != null && ownerEntityId.Equals(entity.Id))
                        || (entityToListen != null && entityToListen == entity.Id))
                    {
                        wrappers[i].reset();
                        wrapperPool.Add(wrappers[i]);
                        wrappers.RemoveAt(i);
                    }
                }
            }

            // ahora a remover listenres filtrados por entidad
            foreach (int eventType in filteredGeneralListeners.Keys)
            {
                Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
                if (filteredGeneralListeners.TryGetValue(eventType, out dic))
                {
                    List<EventListenerWrapper> toClear = null;
                    if (dic.TryGetValue(entity, out toClear))
                    {
                        for (int i = toClear.Count - 1; i >= 0; i--)
                        {
                            toClear[i].reset();
                            wrapperPool.Add(toClear[i]);
                            toClear.RemoveAt(i);
                        }
                        dic.Remove(entity);
                    }
                }
            }

        }

        /// <summary>
        /// Remueve de la lista de listeners de propiedades las referencias de una entidad.
        /// </summary>
        /// <param name="entity">Entidad a remover.</param>
        public void removeEntityFromPropertyListeners(IEntity entity)
        {
            foreach (int property in propertyListeners.Keys)
            {
                List<EventListenerWrapper> wrappers = propertyListeners[property];

                for (int i = propertyListeners[property].Count - 1; i < 0; i--)
                {
                    string ownerEntityId = wrappers[i].ownerEntityId;
                    string entityToListen = wrappers[i].entityToListenId;

                    if ((ownerEntityId != null && ownerEntityId.Equals(entity.Id))
                        || (entityToListen != null && entityToListen == entity.Id))
                    {
                        wrappers[i].reset();
                        wrapperPool.Add(wrappers[i]);
                        wrappers.RemoveAt(i);
                    }
                }
            }

            // ahora a remover listenres filtrados por entidad
            foreach (int eventType in filteredPropertyListeners.Keys)
            {
                Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
                if (filteredPropertyListeners.TryGetValue(eventType, out dic))
                {
                    List<EventListenerWrapper> toClear = null;
                    if (dic.TryGetValue(entity, out toClear))
                    {
                        for (int i = toClear.Count - 1; i < 0; i--)
                        {
                            toClear[i].reset();
                            wrapperPool.Add(toClear[i]);
                            toClear.RemoveAt(i);
                        }
                        dic.Remove(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Remueve de la lista de listeners de estado las referencias de una entidad.
        /// </summary>
        /// <param name="entity">Entidad a remover.</param>
        public void removeEntityFromStateListeners(IEntity entity)
        {
            foreach (int state in entityStateListeners.Keys)
            {
                List<EventListenerWrapper> wrappers = entityStateListeners[state];

                for (int i = entityStateListeners[state].Count - 1; i < 0; i--)
                {
                    string ownerEntityId = wrappers[i].ownerEntityId;
                    string entityToListen = wrappers[i].entityToListenId;

                    if ((ownerEntityId != null && ownerEntityId.Equals(entity.Id))
                        || (entityToListen != null && entityToListen == entity.Id))
                    {
                        wrappers[i].reset();
                        wrapperPool.Add(wrappers[i]);
                        wrappers.RemoveAt(i);
                    }
                }
            }

            // ahora a remover listenres filtrados por entidad
            foreach (int state in filteredStateListeners.Keys)
            {
                Dictionary<IEntity, List<EventListenerWrapper>> dic = null;
                if (filteredStateListeners.TryGetValue(state, out dic))
                {
                    List<EventListenerWrapper> toClear = null;
                    if (dic.TryGetValue(entity, out toClear))
                    {
                        for (int i = toClear.Count - 1; i < 0; i--)
                        {
                            toClear[i].reset();
                            wrapperPool.Add(toClear[i]);
                            toClear.RemoveAt(i);
                        }
                        dic.Remove(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Remueve de la lista de listeners de colisiones las referencias de una entidad.
        /// </summary>
        /// <param name="entity">Entidad a remover.</param>
        public void removeEntityFromCollisionListeners(IEntity entity)
        {
            if (!collisionListeners.ContainsKey(entity)) return;

            List<EventListenerWrapper> wrappers = collisionListeners[entity];
            for (int i = wrappers.Count - 1; i >= 0; i--)
            {
                wrappers[i].reset();
                wrapperPool.Add(wrappers[i]);
                wrappers.RemoveAt(i);
            }
            collisionListeners.Remove(entity);
        }

        public void removeCollisionListenerForCollisionBody(CollisionBody col)
        {
            if (!collisionListeners.ContainsKey(col.Owner)) return;

            List<EventListenerWrapper> wrappers = collisionListeners[col.Owner];
            for (int i = wrappers.Count - 1; i >= 0; i--)
            {
                if (wrappers[i].entityToListenId == col.Id)
                {
                    wrappers[i].reset();
                    wrapperPool.Add(wrappers[i]);
                    wrappers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Retorna LA instancia principal del event manager para realizar operaciones.
        /// </summary>
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }
                return instance;
            }
        }
    }
}
