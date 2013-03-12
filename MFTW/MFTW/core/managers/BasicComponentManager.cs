using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FeInwork.core.managers;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Base;
using FeInwork.core.Base;

namespace FeInwork.Core.Managers
{
	public class BasicComponentManager : IComponentManager
	{
        // instancia de este objeto
        private static BasicComponentManager instance;
        /// <summary>
        /// Cambias estas listas POR algo que acepte valores unicos!! just in case
        /// </summary>
		private List<Core.Interfaces.IUpdateableFE> updateables;
        private List<Core.Interfaces.IDrawableFE> drawables;
        private List<Core.Interfaces.IDrawUpdateableFE> drawUpdateables;
        // listas temporales
        private List<Core.Interfaces.IUpdateableFE> newUpdateables;
        private List<Core.Interfaces.IDrawableFE> newDrawables;
        private List<Core.Interfaces.IDrawUpdateableFE> newDrawUpdateables;

        private BasicComponentManager()
        {
            updateables = new List<Core.Interfaces.IUpdateableFE>();
            drawables = new List<Core.Interfaces.IDrawableFE>();
            drawUpdateables = new List<Core.Interfaces.IDrawUpdateableFE>();
            newUpdateables = new List<Core.Interfaces.IUpdateableFE>();
            newDrawables = new List<Core.Interfaces.IDrawableFE>();
            newDrawUpdateables = new List<Core.Interfaces.IDrawUpdateableFE>();
        }

        public void update(GameTime gameTime)
        {
            InputManager.update();
            // actualizar lista por si hay algo nuevo que agregar.
            updateUpdateableList();
            for (int i = 0; i < updateables.Count; i++)
            {
                IUpdateableFE component = updateables[i];
                if (component.Enabled)
                {
                    component.Update(gameTime);
                }
            }
            // Llamadas a drawUpdates
            for (int i = 0; i < drawUpdateables.Count; i++)
            {
                IDrawUpdateableFE component = drawUpdateables[i];
                component.DrawUpdate(gameTime);
            }
            // update el controller principal
            ControlManager.Instance.update(gameTime);
        }

        public void draw(GameTime gameTime)
        {
            updateDrawableList();
            for (int i = 0; i < drawables.Count; i++)
            {
                IDrawableFE component = drawables[i];
                if (component.Visible)
                {
                    component.Draw(gameTime);
                }
            }
        }

        public void addComponent(IComponent component)
        {
            // verifica el tipo del componente 
            Type ct = component.GetType();
            // verifica primero si es un componente de control * tactica anti-vladimir *
            if (Program.GAME.IsDebugMode && ct.IsSubclassOf(typeof(BaseControlComponent)))
            {
                throw new Exception("Usar ControlComponent para manejar este tipo de componentes >_> *This did not happen, sí tu shit de eventos desordenados sinsentido, Victor Fuck U*");
            }

            IUpdateableFE updateable = null;
            IDrawUpdateableFE drawUpdateable = null;
            IDrawableFE drawable = null;

            // si es de tipo updateable se agrega a lista correspondiente
            if ((updateable = component as IUpdateableFE) != null)
            {
                // uso as aqui xq ya se supone que es de tipo IUpdateable asi que no regresara null :/
                newUpdateables.Add(updateable);
            }

            if ((drawUpdateable = component as IDrawUpdateableFE) != null)
            {
                // lo mismo que con iupdateable
                newDrawUpdateables.Add(drawUpdateable);
                newDrawables.Add(drawUpdateable);
            }
            else if ((drawable = component as IDrawableFE) != null)
            {
                // lo mismo que con iupdateable
                newDrawables.Add(drawable);
            }
           // arreglar lata
            // si no es ninguno de los dos tipo de compos que este manejador MANEJA then se tira la excepcion
            //throw new ArgumentException("El Component Manager actual (BasicComponentManager) no soporta componentes de tipo: " + component.GetType().ToString());
        }

        public void addDrawableOnly(IDrawableFE drawableObject)
        {
            newDrawables.Add(drawableObject);
            // Si es IDrawUpdateableFE lo agrega tambien a su
            // respectiva lista
            IDrawUpdateableFE drawUpdateable = null;
            if ((drawUpdateable = drawableObject as IDrawUpdateableFE) != null)
            {
                this.newDrawUpdateables.Add(drawUpdateable);
            }
        }

        public void addUpdateableOnly(IUpdateableFE updateableObject)
        {
            newUpdateables.Add(updateableObject);
        }

        private void updateUpdateableList()
        {
            // solo si hay algo new
            if(newUpdateables.Count > 0)
            {
                updateables.InsertRange(updateables.Count, newUpdateables);
                newUpdateables.Clear();
            }
            // actualiza también la lista de drawUpdateables
            if (newDrawUpdateables.Count > 0)
            {
                drawUpdateables.InsertRange(drawUpdateables.Count, newDrawUpdateables);
                newDrawUpdateables.Clear();
            }
        }

        private void updateDrawableList()
        {
            // solo si hay algo new
            if (newDrawables.Count > 0)
            {
                drawables.InsertRange(drawables.Count, newDrawables);
                newDrawables.Clear();
            }
        }

        public void removeComponent(IComponent component)
        {
            Type ct = component.GetType();

            IUpdateableFE updateable = null;
            IDrawUpdateableFE drawUpdateable = null;
            IDrawableFE drawable = null;

            if ((updateable = component as IUpdateableFE) != null)
            {
                this.updateables.Remove(updateable);
            }
            // Si es IDrawUpdateableFE lo elimina de ambas listas de draw, de otra forma
            // solo verifica si es IDrawable para eliminarlo
            if ((drawUpdateable = component as IDrawUpdateableFE) != null)
            {
                this.drawUpdateables.Remove(drawUpdateable);
                this.drawables.Remove(drawUpdateable);
            }
            else if ((drawable = component as IDrawableFE) != null)
            {
                this.drawables.Remove(drawable);
            }
        }

        public void removeUpdateable(IUpdateableFE updateableObject)
        {
            this.updateables.Remove(updateableObject);
        }

        public void removeDrawable(IDrawableFE drawableObject)
        {
            this.drawables.Remove(drawableObject);

            // Verifica adicionalmente si el objeto es un
            // DrawUpdateable para eliminarlo de la lista
            IDrawUpdateableFE drawUpdateable = null;
            if ((drawUpdateable = drawableObject as IDrawUpdateableFE) != null)
            {
                this.drawUpdateables.Remove(drawUpdateable);
            }
        }

        public void removeComponentsFromEntity(IEntity entity)
        {
            foreach(IComponent component in entity.ComponentList)
            {
                this.removeComponent(component);
            }

            entity.removeAll();
        }

        public static BasicComponentManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BasicComponentManager();
                }
                return instance;
            }
        }
    }
}
