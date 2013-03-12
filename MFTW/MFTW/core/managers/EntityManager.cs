using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using Microsoft.Xna.Framework;

namespace FeInwork.core.managers
{
    public class EntityManager
    {
        /// <summary>
        /// Instancia de este objeto
        /// </summary>
        private static EntityManager instance;
        /// <summary>
        /// Dictionario que guarda todas las entidades del juego indexado por Id
        /// </summary>
        private Dictionary<string, IEntity> entities;
        /// <summary>
        /// 
        /// </summary>
        private List<IEntity> entitiesToRemove;
        /// <summary>
        /// 
        /// </summary>
        private StringBuilder generatedId;
        /// <summary>
        /// 
        /// </summary>
        private Random random;

        private EntityManager()
        {
            entities = new Dictionary<string, IEntity>();
            entitiesToRemove = new List<IEntity>();
            generatedId = new StringBuilder();
            random = new Random();
        }

        public string generateId()
        {
            generatedId.Remove(0, generatedId.Length);
            while (generatedId.Length == 0 || entities.ContainsKey(generatedId.ToString()))
            {
                generatedId.Remove(0, generatedId.Length);
                generatedId.Append(random.Next(0, 999999999));
                if (generatedId.Length >= 7)
                {
                    generatedId.Remove(6, generatedId.Length - 6);
                }                
            }
            return generatedId.ToString();
        }

        public void update(GameTime gameTime)
        {
            for (int i = entitiesToRemove.Count - 1; i >= 0; i--)
            {
                IEntity entity = entitiesToRemove[i];
                this.removeEntity(entity);
            }

            if (entitiesToRemove.Count > 0)
            {
                entitiesToRemove.Clear();
            }
        }

        /// <summary>
        /// Agrega una entidad al Manager cuyo Id sea único
        /// </summary>
        /// <param name="entity">Entidad a agregar</param>
        public void addEntity(IEntity entity)
        {
            if (this.entities.ContainsKey(entity.Id))
            {
                throw new InvalidOperationException("No se puede agregar una entidad cuyo Id se encuentre repetido");
            }
            else
            {
                this.entities.Add(entity.Id, entity);
            }
        }

        public void requestRemoveEntity(IEntity entity)
        {
            if (!this.entitiesToRemove.Contains(entity))
            {
                this.entitiesToRemove.Add(entity);
            }
        }

        /// <summary>
        /// Remueve una entidad de este manager y al mismo tiepo de
        /// otros managers mayores para así liberar todas las referencias
        /// de esta entidad.
        /// </summary>
        /// <param name="entity">Entidad a remover.</param>
        public void removeEntity(IEntity entity)
        {
            this.entities.Remove(entity.Id);

            EventManager.Instance.removeEntityFromListeners(entity);
            Program.GAME.ComponentManager.removeComponentsFromEntity(entity);
        }

        /// <summary>
        /// Obtiene la unica instancia existente del EntityManager
        /// </summary>
        public static EntityManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntityManager();
                }
                return instance;
            }
        }
    }
}
