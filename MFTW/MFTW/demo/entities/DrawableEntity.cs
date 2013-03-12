using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using FeInwork.core.interfaces;
using Microsoft.Xna.Framework;
using FeInwork.Core.Util;
using FeInwork.FeInwork.util;

namespace FeInwork.FeInwork.entities
{
    /// <summary>
    /// Clase que agrega metodos utiles para todas las entidades que se van a dibujar en pantalla.
    /// </summary>
    public class DrawableEntity: Entity, IFocusable
    {
        //private Dictionary<Type, AbstractDrawEffect> effectList;
        private List<AbstractDrawEffect> effectList;

        public DrawableEntity(string entityId)
            : base(entityId)
        {
            initialize();
        }

        public DrawableEntity()
        {
            initialize();
        }

        public override void initialize()
        {
            // Se agregan las propiedades y entidades básicas que todo
            // DrawableEntity debería tener
            this.addVectorProperty(EntityProperty.Position, Vector2.Zero);
            this.addVectorProperty(EntityProperty.Scale, new Vector2(1, 1));
            this.addFloatProperty(EntityProperty.Rotation, UtilMethods.logicToDrawAngle(GameAngles.RIGHT_ANGLE));
            this.addState(EntityState.FacingRight, true);
            this.addState(EntityState.Visible, true);
        }

        /// <summary>
        /// Agrega un nuevo efecto de dibujado a la lista de efectos
        /// </summary>
        /// <param name="effect"></param>
        public void addDrawEffect(AbstractDrawEffect effect)
        {
            if (effectList == null)
            {
                //effectList = new Dictionary<Type, AbstractDrawEffect>();
                effectList = new List<AbstractDrawEffect>();
            }

            /*Type type = effect.GetType();
            if (!effectList.ContainsKey(type))
            {
                effectList.Add(type, null);
            }*/

            //effectList[type] = effect;
            effectList.Add(effect);
        }

        /*public void removeDrawEffect<T>(T effectType) where T : AbstractDrawEffect
        {
            if (effectList == null) return;

            Type type = typeof(T);
            effectList.Remove(type);
        }*/

        /*public T findDrawEffect<T>(T effectType) where T : AbstractDrawEffect
        {
            if (effectList == null) return default(T);

            Type type = typeof(T);

            if (effectList.ContainsKey(type))
            {
                return (T)effectList[type];
            }
            else
            {
                return default(T);
            }
        }*/

        public T findDrawEffect<T>() where T : AbstractDrawEffect
        {
            if (effectList == null) return default(T);

            Type type = typeof(T);

            for (int i = 0; i < effectList.Count; i++) 
            {
                if (type.Equals(effectList[i].GetType()))
                {
                    return (T)effectList[i];
                }
                
            }
            
            return default(T);
        }

        /// <summary>
        /// Devuelve la lista de efectos de dibujado de esta entidad
        /// </summary>
        public List<AbstractDrawEffect> Effects
        {
            get { return this.effectList; }
        }

        public float X
        {
            get { return getVectorProperty(EntityProperty.Position).X; }
            set { changeVectorProperty(EntityProperty.Position, 
                getVectorProperty(EntityProperty.Position) + new Vector2(value, 0), false); }
        }

        public float Y
        {
            get { return getVectorProperty(EntityProperty.Position).Y; }
            set { changeVectorProperty(EntityProperty.Position, 
                getVectorProperty(EntityProperty.Position) + new Vector2(0, value), false); }
        }

        public Vector2 Position
        {
            get 
            {
                return getVectorProperty(EntityProperty.Position); 
            }
            set 
            {
                changeVectorProperty(EntityProperty.Position, value, false);
            }
        }

        public Vector2 Scale
        {
            get { return getVectorProperty(EntityProperty.Scale); }
            set { changeVectorProperty(EntityProperty.Scale, value, false); }
        }

        public bool IsFacingRight
        {
            get { return getState(EntityState.FacingRight); }
            set { changeState(EntityState.FacingRight, value, false); }
        }

        public float Rotation
        {
            get { return getFloatProperty(EntityProperty.Rotation); }
            set { changeFloatProperty(EntityProperty.Rotation, value, false); }
        }

        public bool Visible
        {
            get { return getState(EntityState.Visible); }
            set { changeState(EntityState.Visible, value, false); }
        }
    }
}
