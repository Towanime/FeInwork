using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using FeInwork.core.util;

namespace FeInwork.core.collision.bodies
{
    public class Sphere : CollisionBody
    {
        #region Variables
        /// <summary>
        /// Radio de la Esfera
        /// </summary>
        private float radius; 
        #endregion

        #region Constructors
        public Sphere(string id, Vector2 startPosition, float radius, IEntity owner, bool isSolid)
            : base(owner, id, isSolid)
        {
            this.center = new Vector2(startPosition.X + radius, startPosition.Y + radius);
            this.radius = radius;
            this.BuildAABB(); 
        } 
        #endregion

        #region Properties
        public float Diameter
        {
            get { return radius * 2; }
        }

        public float Radius
        {
            get { return radius; }
        } 
        #endregion

        #region Protected Methods
        /// <summary>
        /// Verifica si esta esfera esta colisionando o va a colisionar
        /// con un poligono dado
        /// </summary>
        /// <param name="polygon">Poligono con el cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera esta esfera</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected override CollisionResult IntersectWithPolygon(Polygon polygon, ref Vector2 distance)
        {
            CollisionResult result = new CollisionResult();
            result.triggeringBody = this;
            result.affectedBody = polygon;
            // Empezamos asumiendo que las dos figuras no
            // se encuentran intersectando
            result.intersect = false;
            result.willIntersect = false;
            // Se toman tanto la cantidad de puntos
            // como la cantidad de lados del poligono
            int sideCountPolygon = polygon.Sides.Count;
            int pointCountPolygon = polygon.Vertices.Count;
            // Dos valores distintos para guardar 
            float minIntervalDistanceAfterMove = float.PositiveInfinity;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();

            // Ahora se estudia cada lado del poligono para verificar si el centro
            // de la esfera se encuentra perpendicular a algun punto de ese lado
            for (int sideIndex = 0; sideIndex < sideCountPolygon; sideIndex++)
            {
                Line currentSide = polygon.Sides[sideIndex];

                // Se crea un vector paralelo al lado donde puedan proyectase
                // ambos puntos del lado actual mas el centro de la esfera
                Vector2 axis = new Vector2(currentSide.Edge.X, currentSide.Edge.Y);
                axis.Normalize();

                float centerA = Vector2.Dot(axis, this.Center);
                float minB = Vector2.Dot(axis, currentSide.StartPoint);
                float maxB = Vector2.Dot(axis, currentSide.EndPoint);

                float velocityProjection = Vector2.Dot(axis, distance);

                // Se realiza un chequeo preliminar antes de sumar el vector
                // de distancia
                #region Verificaciones de intersección actual

                // Si el punto centro se encuentra perpendicular a algun
                // punto del lado actual, entonces la esfera puede encontrarse en esa region
                if (minB <= centerA && maxB >= centerA)
                {
                    // Creamos un eje perpendicular a la linea para obtener la distancia
                    // entre un punto de la linea y el centro de la esfera
                    axis = new Vector2(-currentSide.Edge.Y, currentSide.Edge.X);
                    axis.Normalize();

                    // Ya que el eje es perpendicular, tanto el punto inicial de la linea
                    // como el final terminan en la misma posicion al ser proyectados
                    float pointA = Vector2.Dot(axis, this.Center);
                    float pointB = Vector2.Dot(axis, currentSide.EndPoint);

                    // Se obtiene el intervalo y se guarda en caso de que sea menor
                    // al intervalo anterior (La esfera se encontrara en la region de voronoi
                    // que tenga el punto mas cercano desde la esfera hacia el poligono)
                    float intervalDistance = Math.Abs(pointA - pointB);
                    if (intervalDistance < minIntervalDistance)
                    {
                        minIntervalDistance = intervalDistance;
                    }
                }
                #endregion

                // Aplicamos la proyeccion de velocidad a el lado actual
                centerA += velocityProjection;

                // Si el punto centro se encuentra perpendicular a algun
                // punto del lado actual, entonces la esfera puede encontrarse en esa region
                if (minB <= centerA && maxB >= centerA)
                {
                    // Creamos un eje perpendicular a la linea para obtener la distancia
                    // entre un punto de la linea y el centro de la esfera
                    axis = new Vector2(-currentSide.Edge.Y, currentSide.Edge.X);
                    axis.Normalize();

                    // Volvemos a aplicar la proyeccion de velocidad puesto que
                    // esta vez estamos proyectando en un diferente eje
                    velocityProjection = Vector2.Dot(axis, distance);
                    // Ya que el eje es perpendicular, tanto el punto inicial de la linea
                    // como el final terminan en la misma posicion al ser proyectados
                    float pointA = Vector2.Dot(axis, this.Center) + velocityProjection;
                    float pointB = Vector2.Dot(axis, currentSide.EndPoint);

                    // Se obtiene el intervalo y se guarda en caso de que sea menor
                    // al intervalo anterior (La esfera se encontrara en la region de voronoi
                    // que tenga el punto mas cercano desde la esfera hacia el poligono)
                    float intervalDistance = Math.Abs(pointA - pointB);
                    if (intervalDistance < minIntervalDistanceAfterMove)
                    {
                        minIntervalDistanceAfterMove = intervalDistance;
                        translationAxis = axis;

                        Vector2 d = this.Center - polygon.Center;
                        if (Vector2.Dot(d, translationAxis) < 0)
                        {
                            translationAxis = -translationAxis;
                        }
                    }
                }
            }

            // Luego se estudia la distancia entre cada vertice del poligono
            // contra el centro de la esfera, si se encuentra alguna distancia
            // menor que las ya guardadas entonces se guarda
            for (int pointIndex = 0; pointIndex < pointCountPolygon; pointIndex++)
            {
                Vector2 currentPoint = polygon.Vertices[pointIndex];
                // Creamos una linea que vaya desde el vertice hasta el centro
                // de la esfera, sumandole el vector de distancia al vertice
                Line line = new Line(this.Center + distance, currentPoint);

                Vector2 axis = new Vector2(line.Edge.X, line.Edge.Y);
                axis.Normalize();

                if (line.Lenght < minIntervalDistanceAfterMove)
                {
                    minIntervalDistanceAfterMove = line.Lenght;
                    translationAxis = axis;

                    Vector2 d = polygon.Center - this.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                    {
                        translationAxis = -translationAxis;
                    }
                }

                // Misma verificacion sin sumar el vector de distancia
                #region Verificaciones de intersección actual
                line = new Line(currentPoint, this.Center);

                axis = new Vector2(line.Edge.X, line.Edge.Y);
                axis.Normalize();

                if (line.Lenght < minIntervalDistance)
                {
                    minIntervalDistance = line.Lenght;
                }
                #endregion
            }

            // Se verifica si el poligono intersecta
            bool isInside = polygon.PointInBody(this.Center);
            if (isInside || minIntervalDistance < this.Radius)
            {
                result.intersect = true;
            }

            // Se verifica si intersectaran y aplica un vector de transicion
            // diferente dependiendo de si el centro de la esfera se encuentra
            // dentro o fuera del poligono
            isInside = polygon.PointInBody(this.Center - distance);
            if (isInside)
            {
                result.minimumTranslationVector =
                       translationAxis * (this.Radius + minIntervalDistanceAfterMove);
                result.willIntersect = true;
            }
            else if (minIntervalDistanceAfterMove < this.Radius)
            {
                result.minimumTranslationVector =
                       translationAxis * Math.Abs(this.Radius - minIntervalDistanceAfterMove);
                result.willIntersect = true;
            }

            result.translationAxis = translationAxis;
            return result;
        }

        /// <summary>
        /// Verifica si esta esfera esta colisionando o va a colisionar
        /// con una caja (rectangulo) dada
        /// </summary>
        /// <param name="box">Caja con la cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera esta esfera</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected override CollisionResult IntersectWithBox(Box box, ref Vector2 distance)
        {
            CollisionResult result = new CollisionResult();
            result.triggeringBody = this;
            result.affectedBody = box;
            // Empezamos asumiendo que las dos figuras no
            // se encuentran intersectando
            result.intersect = false;
            result.willIntersect = false;
            int sideCountRectangle = 4;
            int pointCountRectangle = 4;
            // Dos valores distintos para guardar 
            float minIntervalDistanceAfterMove = float.PositiveInfinity;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();

            // Ahora se estudia cada lado del rectangulo para verificar si el centro
            // de la esfera se encuentra perpendicular a algun punto de ese lado
            for (int sideIndex = 0; sideIndex < sideCountRectangle; sideIndex++)
            {
                Line currentSide = box.Sides[sideIndex];

                // Se crea un vector paralelo al lado donde puedan proyectase
                // ambos puntos del lado actual mas el centro de la esfera
                Vector2 axis = new Vector2(currentSide.Edge.X, currentSide.Edge.Y);
                axis.Normalize();

                float centerA = Vector2.Dot(axis, this.Center);
                float minB = Vector2.Dot(axis, currentSide.StartPoint);
                float maxB = Vector2.Dot(axis, currentSide.EndPoint);

                float velocityProjection = Vector2.Dot(axis, distance);

                // Se realiza un chequeo preliminar antes de sumar el vector
                // de distancia
                #region Verificaciones de intersección actual

                // Si el punto centro se encuentra perpendicular a algun
                // punto del lado actual, entonces la esfera puede encontrarse en esa region
                if (minB <= centerA && maxB >= centerA)
                {
                    // Creamos un eje perpendicular a la linea para obtener la distancia
                    // entre un punto de la linea y el centro de la esfera
                    axis = new Vector2(-currentSide.Edge.Y, currentSide.Edge.X);
                    axis.Normalize();

                    // Ya que el eje es perpendicular, tanto el punto inicial de la linea
                    // como el final terminan en la misma posicion al ser proyectados
                    float pointA = Vector2.Dot(axis, this.Center);
                    float pointB = Vector2.Dot(axis, currentSide.EndPoint);

                    // Se obtiene el intervalo y se guarda en caso de que sea menor
                    // al intervalo anterior (La esfera se encontrara en la region de voronoi
                    // que tenga el punto mas cercano desde la esfera hacia el rectangulo)
                    float intervalDistance = Math.Abs(pointA - pointB);
                    if (intervalDistance < minIntervalDistance)
                    {
                        minIntervalDistance = intervalDistance;
                    }
                }
                #endregion

                // Aplicamos la proyeccion de velocidad a el lado actual
                centerA += velocityProjection;

                // Si el punto centro se encuentra perpendicular a algun
                // punto del lado actual, entonces la esfera puede encontrarse en esa region
                if (minB <= centerA && maxB >= centerA)
                {
                    // Creamos un eje perpendicular a la linea para obtener la distancia
                    // entre un punto de la linea y el centro de la esfera
                    axis = new Vector2(-currentSide.Edge.Y, currentSide.Edge.X);
                    axis.Normalize();

                    // Volvemos a aplicar la proyeccion de velocidad puesto que
                    // esta vez estamos proyectando en un diferente eje
                    velocityProjection = Vector2.Dot(axis, distance);
                    // Ya que el eje es perpendicular, tanto el punto inicial de la linea
                    // como el final terminan en la misma posicion al ser proyectados
                    float pointA = Vector2.Dot(axis, this.Center) + velocityProjection;
                    float pointB = Vector2.Dot(axis, currentSide.EndPoint);

                    // Se obtiene el intervalo y se guarda en caso de que sea menor
                    // al intervalo anterior (La esfera se encontrara en la region de voronoi
                    // que tenga el punto mas cercano desde la esfera hacia el rectangulo)
                    float intervalDistance = Math.Abs(pointA - pointB);
                    if (intervalDistance < minIntervalDistanceAfterMove)
                    {
                        minIntervalDistanceAfterMove = intervalDistance;
                        translationAxis = axis;

                        Vector2 d = this.Center - box.Center;
                        if (Vector2.Dot(d, translationAxis) < 0)
                        {
                            translationAxis = -translationAxis;
                        }
                    }
                }
            }

            // Luego se estudia la distancia entre cada vertice del rectangulo
            // contra el centro de la esfera, si se encuentra alguna distancia
            // menor que las ya guardadas entonces se guarda
            for (int pointIndex = 0; pointIndex < pointCountRectangle; pointIndex++)
            {
                Vector2 currentPoint = box.Points[pointIndex];
                // Creamos una linea que vaya desde el vertice hasta el centro
                // de la esfera, sumandole el vector de distancia al vertice
                Line line = new Line(this.Center + distance, currentPoint);

                Vector2 axis = new Vector2(line.Edge.X, line.Edge.Y);
                axis.Normalize();

                if (line.Lenght < minIntervalDistanceAfterMove)
                {
                    minIntervalDistanceAfterMove = line.Lenght;
                    translationAxis = axis;

                    Vector2 d = this.Center - box.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                    {
                        translationAxis = -translationAxis;
                    }
                }

                // Misma verificacion sin sumar el vector de distancia
                #region Verificaciones de intersección actual
                line = new Line(this.Center, currentPoint);

                axis = new Vector2(line.Edge.X, line.Edge.Y);
                axis.Normalize();

                if (line.Lenght < minIntervalDistance)
                {
                    minIntervalDistance = line.Lenght;
                }
                #endregion
            }

            // Se verifica si el poligono intersecta
            bool isInside = box.PointInBody(this.Center);
            if (isInside || minIntervalDistance < this.Radius)
            {
                result.intersect = true;
            }

            // Se verifica si intersectaran y aplica un vector de transicion
            // diferente dependiendo de si el centro de la esfera se encuentra
            // dentro o fuera del rectangulo
            isInside = box.PointInBody(this.Center - distance);
            if (isInside)
            {
                result.minimumTranslationVector =
                       translationAxis * (this.Radius + minIntervalDistanceAfterMove);
                result.willIntersect = true;
            }
            else if (minIntervalDistanceAfterMove < this.Radius)
            {
                result.minimumTranslationVector =
                       translationAxis * Math.Abs(this.Radius - minIntervalDistanceAfterMove);
                result.willIntersect = true;
            }

            result.translationAxis = translationAxis;
            return result;
        }

        /// <summary>
        /// Verifica si esta esfera esta colisionando o va a colisionar
        /// con otra esféra dada
        /// </summary>
        /// <param name="sphere">Segunda esféra con la cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera la esfera principal (this)</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected override CollisionResult IntersectWithSphere(Sphere sphere, ref Vector2 distance)
        {
            CollisionResult result = new CollisionResult();
            result.triggeringBody = this;
            result.affectedBody = sphere;
            
            float distanceBetween = 0;
            distanceBetween = Vector2.Distance(this.center, sphere.center);
            result.intersect = distanceBetween < this.radius + sphere.radius;
            distanceBetween = Vector2.Distance(this.center, sphere.center + distance);
            result.willIntersect = distanceBetween < this.radius + sphere.radius;

            if (result.willIntersect)
            {
                Vector2 translationAxis = new Vector2((this.center.X) - (sphere.center.X + distance.X),
                    (this.center.Y) - (sphere.center.Y + distance.Y));
                translationAxis = Vector2.Normalize(translationAxis);
                result.translationAxis = translationAxis;
                result.minimumTranslationVector =
                       translationAxis * ((this.radius + sphere.radius) - distanceBetween);
            }

            return result;
        } 
        #endregion

        #region Public Methods
        /// <summary>
        /// Crea un rectangulo que sea capaz de contener toda la esfera por dentro
        /// modificando los valores minimos y maximos pasados por referencia
        /// </summary>
        /// <param name="minX">La coordenada en x del lado izquierdo</param>
        /// <param name="minY">La coordenada en y del lado superior</param>
        /// <param name="maxX">La coordenada en x del lado derecho</param>
        /// <param name="maxY">La coordenada en y del lado inferior</param>
        public override void BuildAABB(out float minX, out float minY, out float maxX, out float maxY)
        {
            minX = this.center.X - this.radius;
            minY = this.center.Y - this.radius;
            maxX = this.center.X + this.radius;
            maxY = this.center.Y + this.radius;
        }

        /// <summary>
        /// Invierte la posicion de todos los puntos de este body
        /// en el eje horizontal como si de un espejo se tratase
        /// </summary>
        /// <param name="referencePoint">Punto de referencia para invertir</param>
        public override void MirrorHorizontal(Vector2 referencePoint)
        {
            this.center.X = referencePoint.X - (this.center.X - referencePoint.X);
            this.BuildAABB(); 
        }

        /// <summary>
        /// Invierte la posicion de todos los puntos de este body
        /// en el eje vertical como si de un espejo se tratase
        /// </summary>
        /// <param name="referencePoint">Punto de referencia para invertir</param>
        public override void MirrorVertical(Vector2 referencePoint)
        {
            this.center.Y = referencePoint.Y - (this.center.Y - referencePoint.Y);
            this.BuildAABB(); 
        }

        /// <summary>
        /// Desplaza la esfera completa según una distancia
        /// </summary>
        /// <param name="x">Distancia a desplazar en x</param>
        /// <param name="y">Distancia a desplazar en y</param>
        public override void Offset(float x, float y)
        {
            this.center.X += x;
            this.center.Y += y;
            this.BuildAABB(); 
        }

        /// <summary>
        /// Determina si el punto dado se encuentra dentro de esta esfera
        /// </summary>
        /// <param name="point">Vector2 con las coordenadas del punto</param>
        /// <returns>True si el punto se encuentra dentro de la esfera</returns>
        public override bool PointInBody(Vector2 point)
        {
            float distance = Vector2.Distance(this.center, point);
            return distance < this.radius;
        }

        /// <summary>
        /// Se realiza una proyección de esta esfera sobre un eje dado
        /// y cambia las referencias del valor minimo y maximo de la esfera en el plano
        /// </summary>
        /// <param name="axis">Eje sobre el cual se hara la proyección</param>
        /// <param name="min">Float que contendra el valor minimo</param>
        /// <param name="max">Float que contendra el valor maximo</param>
        public override void Project(ref Vector2 axis, out float min, out float max)
        {
            min = Vector2.Dot(axis, this.center) - radius;
            max = Vector2.Dot(axis, this.center) + radius;
        }

        public override void Draw(PrimitiveBatch primitiveBatch)
        {
            double angleStep = 2f / this.Radius;
            float x = 0;
            float y = 0;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                x = (float)Math.Round(this.Center.X + this.Radius * Math.Cos(angle));
                y = (float)Math.Round(this.Center.Y + this.Radius * Math.Sin(angle));

                primitiveBatch.AddVertex(new Vector2(x, y), Color.Yellow);

                x = (float)Math.Round(this.Center.X + this.Radius * Math.Cos(angle + angleStep));
                y = (float)Math.Round(this.Center.Y + this.Radius * Math.Sin(angle + angleStep));

                primitiveBatch.AddVertex(new Vector2(x, y), Color.Yellow);
            }
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

            Sphere body = (Sphere)obj;

            return this.owner.Id == body.owner.Id &&
                this.id == body.id &&
                this.center == body.center &&
                this.radius == body.radius;
        }

        public override int GetHashCode()
        {
            const int multiplier = 8;
            if (hashCode == 0)
            {
                int code = 13;
                code = multiplier * code + owner.Id.GetHashCode();
                code = multiplier * code + id.GetHashCode();
                code = multiplier * code + center.GetHashCode();
                code = multiplier * code;
                hashCode = code;
            }
            return hashCode;
        }
        #endregion
    }
}
