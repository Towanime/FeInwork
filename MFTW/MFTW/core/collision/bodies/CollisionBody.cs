using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Base;
using Microsoft.Xna.Framework;
using FeInwork.Core.Interfaces;
using FeInwork.Core.Managers;
using FeInwork.FeInwork.listeners;
using FeInwork.core.util;
using FeInwork.FeInwork.util;

namespace FeInwork.core.collision.bodies
{
    public abstract class CollisionBody
    {
        #region Variables
        /// <summary>
        /// Id obligatorio para un colision body y poder entregar los metodos de colision correctamente.
        /// </summary>
        protected string id;
        /// <summary>
        /// Entidad dueña del cuerpo
        /// </summary>
        protected IEntity owner;
        /// <summary>
        /// Determina si este poligono puede 
        /// colisionar con otros poligonos sólidos
        /// </summary>
        protected bool isSolid;
        /// <summary>
        /// Determina la capa de colisiones en la que se encuentra
        /// el body, por defecto es la capa 0.0
        /// </summary>
        protected float layer = GameLayers.MIDDLE_PLAY_AREA;
        /// <summary>
        /// Un tag de color especifico para este cuerpo en caso de que
        /// Listeners de colision quieran escuchar a un tag de color especifico
        /// </summary>
        protected Color? colorTag = null;
        /// <summary>
        /// Centro del cuerpo de colision
        /// </summary>
        protected Vector2 center;
        /// <summary>
        /// HashCode
        /// </summary>
        protected int hashCode = 0;
        public Rectangle AABB;
        public QuadTreeNode lastBigNode;
        #endregion

        #region Constructors
        /// <summary>
        /// Crea un nuevo cuerpo
        /// </summary>
        /// <param name="owner">Entidad dueña de este cuerpo</param>
        /// <param name="isSolid">Determina si el cuerpo es sólido o no</param>
        /// <param name="colorTag">Le agrega una etiqueta de color al cuerpo</param>
        public CollisionBody(IEntity owner, string bodyId, bool isSolid)
        {
            // anti-vladimir validation
            // por ahora no se valida si ya existe id 
            if (bodyId == null || bodyId == "")
            {
                throw new Exception("CollisionBody debe tener un Id valido.");
            }
            this.owner = owner;
            this.id = bodyId;
            this.isSolid = isSolid;
        }
        #endregion

        #region Properties
        public string Id
        {
            get { return this.id; }
        }

        public Vector2 Center 
        {
            get { return this.center; }
        }

        public IEntity Owner
        {
            get { return this.owner; }
        }

        /// <summary>
        /// Determina si este poligono puede 
        /// colisionar con otros poligonos sólidos
        /// </summary>
        public bool Solid
        {
            get { return this.isSolid; }
            set { this.isSolid = value; }
        }

        /// <summary>
        /// Get o Set la propiedad que Determina la capa de colisiones en 
        /// la que se encuentra el body, por defecto es la capa 0.0
        /// </summary>
        public float Layer
        {
            get { return this.layer; }
            set { this.layer = value; }
        }

        /// <summary>
        /// Get o Set un tag de color especifico para este cuerpo en caso de que
        /// Listeners de colision quieran escuchar a un tag de color especifico
        /// </summary>
        public Color? ColorTag
        {
            get { return this.colorTag; }
            set { this.colorTag = value; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Verifica si este cuerpo esta colisionando o va a colisionar
        /// con un poligono dado
        /// </summary>
        /// <param name="polygon">Poligono con el cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera este cuerpo</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected abstract CollisionResult IntersectWithPolygon(Polygon polygon, ref Vector2 distance);

        /// <summary>
        /// Verifica si este cuerpo esta colisionando o va a colisionar
        /// con una caja (rectangulo) dada
        /// </summary>
        /// <param name="box">Caja con la cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera este cuerpo</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected abstract CollisionResult IntersectWithBox(Box box, ref Vector2 distance);

        /// <summary>
        /// Verifica si este cuerpo esta colisionando o va a colisionar
        /// con una esféra dada
        /// </summary>
        /// <param name="sphere">Esféra con la cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera este cuerpo</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected abstract CollisionResult IntersectWithSphere(Sphere sphere, ref Vector2 distance);

        /// <summary>
        /// Determina la distancia en la cual se separan dos segmentos en una dimensión
        /// dada las posiciones minimas y maximas de cada uno
        /// </summary>
        /// <param name="minA">Punto de inicio del segmento A</param>
        /// <param name="maxA">Punto final del segmento A</param>
        /// <param name="minB">Punto de inicio del segmento B</param>
        /// <param name="maxB">Punto final del segmento B</param>
        /// <returns>Distancia entre los dos segmentos</returns>
        protected static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Crea un rectangulo que sea capaz de contener toda la figura por dentro
        /// modificando los valores minimos y maximos pasados por referencia
        /// </summary>
        /// <param name="minX">La coordenada en x del lado izquierdo</param>
        /// <param name="minY">La coordenada en y del lado superior</param>
        /// <param name="maxX">La coordenada en x del lado derecho</param>
        /// <param name="maxY">La coordenada en y del lado inferior</param>
        public abstract void BuildAABB(out float minX, out float minY, out float maxX, out float maxY);

        /// <summary>
        /// Invierte la posicion de todos los puntos de este body
        /// en el eje horizontal como si de un espejo se tratase
        /// </summary>
        /// <param name="referencePoint">Punto de referencia para invertir</param>
        public abstract void MirrorHorizontal(Vector2 referencePoint);

        /// <summary>
        /// Invierte la posicion de todos los puntos de este body
        /// en el eje vertical como si de un espejo se tratase
        /// </summary>
        /// <param name="referencePoint">Punto de referencia para invertir</param>
        public abstract void MirrorVertical(Vector2 referencePoint);

        /// <summary>
        /// Determina si el punto dado se encuentra dentro de este cuerpo
        /// </summary>
        /// <param name="point">Vector2 con las coordenadas del punto</param>
        /// <returns>True si el punto se encuentra dentro del cuerpo</returns>
        public abstract bool PointInBody(Vector2 point);

        /// <summary>
        /// Se realiza una proyección de este cuerpo sobre un eje dado
        /// y cambia las referencias del valor minimo y maximo del cuerpo en el plano
        /// </summary>
        /// <param name="axis">Eje sobre el cual se hara la proyección</param>
        /// <param name="min">Float que contendra el valor minimo</param>
        /// <param name="max">Float que contendra el valor maximo</param>
        public abstract void Project(ref Vector2 axis, out float min, out float max);

        /// <summary>
        /// Verifica si este cuerpo esta colisionando o va a colisionar
        /// con otro cuerpo dado
        /// </summary>
        /// <param name="polygon">Cuerpo con el cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera este cuerpo</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        public CollisionResult Intersect(CollisionBody secondBody, ref Vector2 distance)
        {
            bool intersect = true;
            bool willIntersect = true;

            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            this.BuildAABB(out minX, out minY, out maxX, out maxY);

            float minSecondX = 0, minSecondY = 0, maxSecondX = 0, maxSecondY = 0;
            secondBody.BuildAABB(out minSecondX, out minSecondY, out maxSecondX, out maxSecondY);

            if (minX > maxSecondX || maxX < minSecondX || minY > maxSecondY || maxY < minSecondY) intersect = false;

            minX += distance.X;
            maxX += distance.X;
            minY += distance.Y;
            maxY += distance.Y;

            if (minX > maxSecondX || maxX < minSecondX || minY > maxSecondY ||maxY < minSecondY) willIntersect = false;

            if (!intersect && !willIntersect)
            {
                CollisionResult result = new CollisionResult();
                result.triggeringBody = this;
                result.affectedBody = secondBody;
                return result;
            }

            if (secondBody.GetType() == typeof(Box))
            {
                return this.IntersectWithBox((Box)secondBody, ref distance);
            }
            else if (secondBody.GetType() == typeof(Polygon))
            {
                return this.IntersectWithPolygon((Polygon)secondBody, ref distance);
            }
            else
            {
                return this.IntersectWithSphere((Sphere)secondBody, ref distance);
            }
        }

        /// <summary>
        /// Crea un rectangulo que sea capaz de contener toda la figura por dentro
        /// modificando los valores minimos y maximos pasados por referencia
        /// </summary>
        /// <returns>Rectangulo que cubre la figura en ese momento.</returns>
        public Rectangle BuildAABB()
        {
            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            this.BuildAABB(out minX, out minY, out maxX, out maxY);
            this.AABB.X = (int)minX;
            this.AABB.Y = (int)minY;
            this.AABB.Width = (int)(maxX - minX);
            this.AABB.Height = (int)(maxY - minY);
            return this.AABB;
        }

        /// <summary>
        /// Mueve la figura según la distancia entre un punto y el nuevo punto
        /// </summary>
        /// <param name="oldPoint">Punto de inicio</param>
        /// <param name="newPoint">Punto final</param>
        public void OffsetRelativeTo(Vector2 oldPoint, Vector2 newPoint)
        {
            Vector2 distance = newPoint - oldPoint;
            this.Offset(distance.X, distance.Y);
        }

        /// <summary>
        /// Desplaza el cuerpo completo según una distancia
        /// </summary>
        /// <param name="distance">Vector que contiene la distancia
        /// por la cual se moverá este cuerpo</param>
        public void Offset(Vector2 distance)
        {
            Offset(distance.X, distance.Y);
        }

        /// <summary>
        /// Desplaza el cuerpo completo según una distancia
        /// </summary>
        /// <param name="x">Distancia a desplazar en x</param>
        /// <param name="y">Distancia a desplazar en y</param>
        public abstract void Offset(float x, float y);

        public abstract void Draw(PrimitiveBatch primitiveBatch);

        /// <summary>
        /// Metodo para ayudar a registrar un listener de un cuerpo en particular mas facilmente.
        /// Igual se pueden usar los addCollisionListener del eventManager pero es mas recomendado
        /// agregarlos desde aca si son para un cuerpo en particular.
        /// </summary>
        /// <param name="listener"></param>
        public void addCollisionListener(CollisionListener listener)
        {
            EventManager.Instance.addCollisionListener(this.owner, this.id, listener);
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

            CollisionBody body = (CollisionBody)obj;

            return this.id ==body.id &&
                this.owner.Id == body.owner.Id;
        }

        public override int GetHashCode()
        {
            const int multiplier = 7;
            if (hashCode == 0)
            {
                int code = 15;
                code = multiplier * code + id.GetHashCode();
                code = multiplier * code + owner.Id.GetHashCode();
                code = multiplier * code;
                hashCode = code;
            }
            return hashCode;
        }
        #endregion
    }
}
