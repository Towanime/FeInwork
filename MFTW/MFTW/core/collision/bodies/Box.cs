using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.util;
using Microsoft.Xna.Framework;
using FeInwork.Core.Interfaces;

namespace FeInwork.core.collision.bodies
{
    public class Box : CollisionBody
    {
        #region Variables
        /// <summary>
        /// Posicion en X del Rectangulo
        /// </summary>
        private float x;
        /// <summary>
        /// Posicion en Y del Rectangulo
        /// </summary>
        private float y;
        /// <summary>
        /// Ancho del Rectangulo
        /// </summary>
        private float width;
        /// <summary>
        /// Alto del Rectangulo
        /// </summary>
        private float height;
        /// <summary>
        /// Arreglo de vertices del rectangulo,
        /// siempre de tamaño 4
        /// </summary>
        private Vector2[] points = new Vector2[4];
        /// <summary>
        /// Arreglo de lados del rectangulo,
        /// siempre de tamaño 4
        /// </summary>
        private Line[] sides = new Line[4];
        #endregion

        #region Constructors
        public Box(string id, float x, float y, float width, float height, IEntity owner, bool isSolid)
            : base(owner, id, isSolid)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.BuildPoints();
            this.BuildEdges();
            this.CalculateCenter();
            this.BuildAABB(); 
        } 
        #endregion

        #region Properties
        public float Height
        {
            get { return height; }
        }

        public Vector2[] Points
        {
            get { return points; }
        }

        public Line[] Sides
        {
            get { return sides; }
        }

        public float Width
        {
            get { return width; }
        }

        public float X
        {
            get { return x; }
        }

        public float Y
        {
            get { return y; }
        } 
        #endregion

        #region Private Methods
        /// <summary>
        /// Construye los lados del poligono
        /// en base a los vertices.
        /// Nota: Es necesario llamar cada vez que se 
        /// altere alguno de los vertices del poligono
        /// </summary>
        private void BuildEdges()
        {
            sides[0].StartPoint = this.points[0];
            sides[0].EndPoint = this.points[1];
            sides[1].StartPoint = this.points[1];
            sides[1].EndPoint = this.points[2];
            sides[2].StartPoint = this.points[2];
            sides[2].EndPoint = this.points[3];
            sides[3].StartPoint = this.points[3];
            sides[3].EndPoint = this.points[0];
        }

        /// <summary>
        /// Construye los vertices del rectangulo
        /// en base a la posicion.
        /// </summary>
        private void BuildPoints()
        {
            points[0].X = this.x;
            points[0].Y = this.y;
            points[1].X = this.x + this.width;
            points[1].Y = this.y;
            points[2].X = this.x + this.width;
            points[2].Y = this.y + this.height;
            points[3].X = this.x;
            points[3].Y = this.y + this.height; 
        } 
        #endregion

        /// <summary>
        /// Calcula el centro del polígono
        /// </summary>
        private void CalculateCenter()
        {
            center.X = this.x + (this.width / 2);
            center.Y = this.y + (this.height / 2);
        }

        #region Protected Methods
        /// <summary>
        /// Verifica si este rectangulo esta colisionando o va a colisionar
        /// con un poligono dado.
        /// Esta verificación se realiza utilizando SAT (Separating Axis Theorem) la cual explica
        /// que, si existe una linea en el plano que pueda separar dos distintas figuras en diferentes
        /// lados por completo, entonces estas figuras no están colisionando.
        /// Nota: Este teorema solo debería aplicarse ante poligonos concavos, de otra forma el algoritmo
        /// no daria una respuesta correcta ante poligonos convexos.
        /// </summary>
        /// <param name="polygon2">Poligono con el cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera este rectangulo</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected override CollisionResult IntersectWithPolygon(Polygon polygon, ref Vector2 distance)
        {
            CollisionResult result = new CollisionResult();
            result.triggeringBody = this;
            result.affectedBody = polygon;
            // Se asume que ambas figuras intersectan hasta que
            // se pueda demostrar lo contrario
            result.intersect = true;
            result.willIntersect = true;
            // Se obtiene la cantidad de lados del rectangulo
            // y del poligono
            int edgeCountRectangle = 2;
            int edgeCountPolygon = polygon.Sides.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            Vector2 axis = new Vector2();
            Line currentSide;

            // Se realiza una verificacion por cada uno de los lados del poligono y por dos lados
            // perpendiculares del rectangulo hasta que pueda encontrarse alguna separación entre 
            // ambas o hasta que se hayan verificado todos los lados
            for (int edgeIndex = 0; edgeIndex < edgeCountPolygon + edgeCountRectangle; edgeIndex++)
            {
                if (edgeIndex < edgeCountRectangle)
                {
                    if (edgeIndex == 0)
                    {
                        currentSide = this.sides[0];
                    }
                    else
                    {
                        currentSide = this.sides[1];
                    }
                }
                else
                {
                    currentSide = polygon.Sides[edgeIndex - edgeCountRectangle];
                }

                // Se obtiene un Vector perpendicular al lado actual con valores unitarios,
                // esto servira para obtener el eje sobre el cual deben hacerse las proyecciones
                axis.X = -currentSide.Edge.Y;
                axis.Y = currentSide.Edge.X;
                axis.Normalize();

                // Se proyectan ambas figuras sobre el mismo eje
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                this.Project(ref axis, out minA, out maxA);
                polygon.Project(ref axis, out minB, out maxB);

                // Se obtiene el intervalo entre ambas figuras sobre el eje,
                // si el intervalo (separación) es mayor a 0 entonces las figuras
                // no están intersectando
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.intersect = false;

                float velocityProjection;
                Vector2.Dot(ref axis, ref distance, out velocityProjection);

                // Luego se realizan los mismos calculos pero sumando el vector
                // de velocidad al rectangulo en (posible) movimiento
                //if (velocityProjection < 0)
                //{
                    minA += velocityProjection;
                //}
                //else
                //{
                    maxA += velocityProjection;
                //}

                // Si el intervalo de distancia es menor a 0 con el rectangulo en movimiento,
                // entonces las figuras tambien intersectaran al moverse
                intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.willIntersect = false;

                // Si ya sabemos que los poligonos estan intersectando y van a intersectar,
                // no hay mucho mas que hacer aquí asi que terminamos las verificaciones
                if (result.intersect == false && result.willIntersect == false)
                {
                    break;
                }

                // Si el intervalo de distancia es el minimo, se guarda
                // junto con el eje donde fue encontrado para asi poder separar
                // a las figuras en esa dirección
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d;
                    Vector2 secondBodyCenter = polygon.Center;
                    Vector2.Subtract(ref this.center, ref secondBodyCenter, out d);

                    float dot;
                    Vector2.Dot(ref d, ref translationAxis, out dot);
                    if (dot < 0)
                    {
                        Vector2.Multiply(ref translationAxis, -1, out translationAxis);
                    }
                }
            }

            // El vector minimo de transición
            // servira para separar ambas figuras en caso de que colisionen.
            if (result.willIntersect)
            {
                Vector2.Multiply(ref translationAxis, minIntervalDistance, out result.minimumTranslationVector);
            }

            result.translationAxis = translationAxis;
            return result;
        }

        /// <summary>
        /// Verifica si este rectangulo esta colisionando o va a colisionar
        /// con otro rectangulo dado.
        /// Esta verificación se realiza utilizando SAT (Separating Axis Theorem) la cual explica
        /// que, si existe una linea en el plano que pueda separar dos distintas figuras en diferentes
        /// lados por completo, entonces estas figuras no están colisionando.
        /// Nota: Este teorema solo debería aplicarse ante poligonos concavos, de otra forma el algoritmo
        /// no daria una respuesta correcta ante poligonos convexos.
        /// </summary>
        /// <param name="box">Segundo rectangulo con el cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera el rectangulo principal (this)</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected override CollisionResult IntersectWithBox(Box box, ref Vector2 distance)
        {
            CollisionResult result = new CollisionResult();
            result.triggeringBody = this;
            result.affectedBody = box;
            // Se asume que ambos rectangulos intersectan hasta que
            // se pueda demostrar lo contrario
            result.intersect = true;
            result.willIntersect = true;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            Line currentSide;
            Vector2 axis = Vector2.Zero;

            // Se realiza una verificacion por dos lados perpendiculares de cada rectangulo
            // hasta que pueda encontrarse alguna separación entre ambos o hasta que se hayan
            // verificado todos los lados
            for (int edgeIndex = 0; edgeIndex < 4; edgeIndex++)
            {
                if (edgeIndex < 2)
                {
                    if (edgeIndex == 0)
                    {
                        currentSide = this.sides[0];
                    }
                    else
                    {
                        currentSide = this.sides[1];
                    }
                }
                else
                {
                    if (edgeIndex - 2 == 0)
                    {
                        currentSide = box.sides[0];
                    }
                    else
                    {
                        currentSide = box.sides[1];
                    }
                }

                // Se obtiene un Vector perpendicular al lado actual con valores unitarios,
                // esto servira para obtener el eje sobre el cual deben hacerse las proyecciones
                axis.X = -currentSide.Edge.Y;
                axis.Y = currentSide.Edge.X;
                axis.Normalize();

                // Se proyectan ambos rectangulos sobre el mismo eje
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                this.Project(ref axis, out minA, out maxA);
                box.Project(ref axis, out minB, out maxB);

                // Se obtiene el intervalo entre ambos rectangulos sobre el eje,
                // si el intervalo (separación) es mayor a 0 entonces los rectangulos
                // no están intersectando
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.intersect = false;

                // Luego se realizan los mismos calculos pero sumando el vector
                // de velocidad al rectangulo en (posible) movimiento
                float velocityProjection;
                Vector2.Dot(ref axis, ref distance, out velocityProjection);

                //if (velocityProjection < 0)
                //{
                    minA += velocityProjection;
                //}
                //else
                //{
                    maxA += velocityProjection;
                //}

                // Si el intervalo de distancia es menor a 0 con el rectangulo en movimiento,
                // entonces las rectangulos tambien intersectaran al moverse
                intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.willIntersect = false;

                // Si ya sabemos que los rectangulos estan intersectando y van a intersectar,
                // no hay mucho mas que hacer aquí asi que terminamos las verificaciones
                if (result.intersect == false && result.willIntersect == false)
                {
                    break;
                }

                // Si el intervalo de distancia es el minimo, se guarda
                // junto con el eje donde fue encontrado para asi poder separar
                // a los rectangulos en esa dirección
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d;
                    Vector2 secondBodyCenter = box.Center;
                    Vector2.Subtract(ref this.center, ref secondBodyCenter, out d);

                    float dot;
                    Vector2.Dot(ref d, ref translationAxis, out dot);
                    if (dot < 0)
                    {
                        Vector2.Multiply(ref translationAxis, -1, out translationAxis);
                    }
                }
            }

            // El vector minimo de transición
            // servira para separar ambos rectangulos en caso de que colisionen.
            if (result.willIntersect)
            {
                Vector2.Multiply(ref translationAxis, minIntervalDistance, out result.minimumTranslationVector);
            }

            result.translationAxis = translationAxis;
            return result;
        }

        /// <summary>
        /// Verifica si este rectangulo esta colisionando o va a colisionar
        /// con una esféra dada.
        /// Para esto se ubica el punto centro de la esfera en la región de voronoi
        /// que le corresponde según el rectangulo, de forma que obtenemos el punto
        /// mas cercano de la esfera al rectangulo junto al respectivo eje.
        /// </summary>
        /// <param name="sphere">Esféra con la cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera este poligono</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected override CollisionResult IntersectWithSphere(Sphere sphere, ref Vector2 distance)
        {
            CollisionResult result = new CollisionResult();
            result.triggeringBody = this;
            result.affectedBody = sphere;
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
                Line currentSide = this.sides[sideIndex];

                // Se crea un vector paralelo al lado donde puedan proyectase
                // ambos puntos del lado actual mas el centro de la esfera
                Vector2 axis = new Vector2(currentSide.Edge.X, currentSide.Edge.Y);
                axis.Normalize();

                float minA = Vector2.Dot(axis, currentSide.StartPoint);
                float maxA = Vector2.Dot(axis, currentSide.EndPoint);
                float centerB = Vector2.Dot(axis, sphere.Center);

                float velocityProjection = Vector2.Dot(axis, distance);

                // Se realiza un chequeo preliminar antes de sumar el vector
                // de distancia
                #region Verificaciones de intersección actual

                // Si el punto centro se encuentra perpendicular a algun
                // punto del lado actual, entonces la esfera puede encontrarse en esa region
                if (minA <= centerB && maxA >= centerB)
                {
                    // Creamos un eje perpendicular a la linea para obtener la distancia
                    // entre un punto de la linea y el centro de la esfera
                    axis = new Vector2(-currentSide.Edge.Y, currentSide.Edge.X);
                    axis.Normalize();

                    // Ya que el eje es perpendicular, tanto el punto inicial de la linea
                    // como el final terminan en la misma posicion al ser proyectados
                    float pointA = Vector2.Dot(axis, currentSide.EndPoint);
                    float pointB = Vector2.Dot(axis, sphere.Center);

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
                if (velocityProjection < 0)
                {
                    minA += velocityProjection;
                }
                else
                {
                    maxA += velocityProjection;
                }

                // Si el punto centro se encuentra perpendicular a algun
                // punto del lado actual, entonces la esfera puede encontrarse en esa region
                if (minA <= centerB && maxA >= centerB)
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
                    float pointA = Vector2.Dot(axis, currentSide.EndPoint) + velocityProjection;
                    float pointB = Vector2.Dot(axis, sphere.Center);

                    // Se obtiene el intervalo y se guarda en caso de que sea menor
                    // al intervalo anterior (La esfera se encontrara en la region de voronoi
                    // que tenga el punto mas cercano desde la esfera hacia el rectangulo)
                    float intervalDistance = Math.Abs(pointA - pointB);
                    if (intervalDistance < minIntervalDistanceAfterMove)
                    {
                        minIntervalDistanceAfterMove = intervalDistance;
                        translationAxis = axis;

                        Vector2 d = this.Center - sphere.Center;
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
                Vector2 currentPoint = this.points[pointIndex];
                // Creamos una linea que vaya desde el vertice hasta el centro
                // de la esfera, sumandole el vector de distancia al vertice
                Line line = new Line(currentPoint + distance, sphere.Center);

                Vector2 axis = new Vector2(line.Edge.X, line.Edge.Y);
                axis.Normalize();

                if (line.Lenght < minIntervalDistanceAfterMove)
                {
                    minIntervalDistanceAfterMove = line.Lenght;
                    translationAxis = axis;

                    Vector2 d = this.Center - sphere.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                    {
                        translationAxis = -translationAxis;
                    }
                }

                // Misma verificacion sin sumar el vector de distancia
                #region Verificaciones de intersección actual
                line = new Line(currentPoint, sphere.Center);

                axis = new Vector2(line.Edge.X, line.Edge.Y);
                axis.Normalize();

                if (line.Lenght < minIntervalDistance)
                {
                    minIntervalDistance = line.Lenght;
                }
                #endregion
            }

            // Se verifica si el poligono intersecta
            bool isInside = this.PointInBody(sphere.Center);
            if (isInside || minIntervalDistance < sphere.Radius)
            {
                result.intersect = true;
            }

            // Se verifica si intersectaran y aplica un vector de transicion
            // diferente dependiendo de si el centro de la esfera se encuentra
            // dentro o fuera del rectangulo
            isInside = this.PointInBody(sphere.Center - distance);
            if (isInside)
            {
                result.minimumTranslationVector =
                       translationAxis * (sphere.Radius + minIntervalDistanceAfterMove);
                result.willIntersect = true;
            }
            else if (minIntervalDistanceAfterMove < sphere.Radius)
            {
                result.minimumTranslationVector =
                       translationAxis * Math.Abs(sphere.Radius - minIntervalDistanceAfterMove);
                result.willIntersect = true;
            }

            result.translationAxis = translationAxis;
            return result;
        } 
        #endregion

        #region Public Methods
        /// <summary>
        /// Crea un rectangulo que sea capaz de contener todo el rectangulo por dentro
        /// modificando los valores minimos y maximos pasados por referencia.
        /// Nota: El rectangulo principal puede haber sido rotado asi que igual tiene que
        /// hacerse este otro rectangulo recto
        /// </summary>
        /// <param name="minX">La coordenada en x del lado izquierdo</param>
        /// <param name="minY">La coordenada en y del lado superior</param>
        /// <param name="maxX">La coordenada en x del lado derecho</param>
        /// <param name="maxY">La coordenada en y del lado inferior</param>
        public override void BuildAABB(out float minX, out float minY, out float maxX, out float maxY)
        {
            /*minX = this.points[0].X;
            minY = this.points[0].Y;
            maxX = this.points[0].X;
            maxY = this.points[0].Y;
            for (int i = 0; i < 4; i++)
            {
                Vector2 rectanglePoint = this.points[i];
                if (rectanglePoint.X < minX)
                {
                    minX = rectanglePoint.X;
                }
                if (rectanglePoint.X > maxX)
                {
                    maxX = rectanglePoint.X;
                }
                if (rectanglePoint.Y < minY)
                {
                    minY = rectanglePoint.Y;
                }
                if (rectanglePoint.Y > maxY)
                {
                    maxY = rectanglePoint.Y;
                }
            }*/
            Vector2 firstPoint = points[0];
            Vector2 thirdPoint = points[2];
            minX = firstPoint.X;
            maxX = thirdPoint.X;
            minY = firstPoint.Y;
            maxY = thirdPoint.Y;
        }

        /// <summary>
        /// Invierte la posicion de todos los puntos de este body
        /// en el eje horizontal como si de un espejo se tratase
        /// </summary>
        /// <param name="referencePoint">Punto de referencia para invertir</param>
        public override void MirrorHorizontal(Vector2 referencePoint)
        {
            this.x = referencePoint.X - (this.x - referencePoint.X) - this.width;
            this.BuildPoints();
            this.BuildEdges();
            this.CalculateCenter();
            this.BuildAABB(); 
        }

        /// <summary>
        /// Invierte la posicion de todos los puntos de este body
        /// en el eje vertical como si de un espejo se tratase
        /// </summary>
        /// <param name="referencePoint">Punto de referencia para invertir</param>
        public override void MirrorVertical(Vector2 referencePoint)
        {
            this.y = referencePoint.Y - (this.y - referencePoint.Y) - this.height;
            this.BuildPoints();
            this.BuildEdges();
            this.CalculateCenter();
            this.BuildAABB(); 
        }

        /// <summary>
        /// Desplaza el rectangulo completo según una distancia
        /// </summary>
        /// <param name="x">Distancia a desplazar en x</param>
        /// <param name="y">Distancia a desplazar en y</param>
        public override void Offset(float x, float y)
        {
            this.x += x;
            this.y += y;
            points[0].X = points[0].X + x;
            points[0].Y = points[0].Y + y;
            points[1].X = points[1].X + x;
            points[1].Y = points[1].Y + y;
            points[2].X = points[2].X + x;
            points[2].Y = points[2].Y + y;
            points[3].X = points[3].X + x;
            points[3].Y = points[3].Y + y;
            this.BuildEdges();
            this.CalculateCenter();
            this.BuildAABB(); 
        }

        /// <summary>
        /// Determina si el punto dado se encuentra dentro de este rectangulo
        /// </summary>
        /// <param name="point">Vector2 con las coordenadas del punto</param>
        /// <returns>True si el punto se encuentra dentro del rectangulo</returns>
        public override bool PointInBody(Vector2 point)
        {
            if (point.X < this.x) return false;
            if (point.X > this.x + this.width) return false;
            if (point.Y < this.y) return false;
            if (point.Y > this.y + this.height) return false;
            return true;
        }

        /// <summary>
        /// Se realiza una proyección de este rectangulo sobre un eje dado
        /// y cambia las referencias del valor minimo y maximo del rectangulo en el plano
        /// </summary>
        /// <param name="axis">Eje sobre el cual se hara la proyección</param>
        /// <param name="min">Float que contendra el valor minimo</param>
        /// <param name="max">Float que contendra el valor maximo</param>
        public override void Project(ref Vector2 axis, out float min, out float max)
        {
            float dotProduct = Vector2.Dot(axis, this.points[0]);
            min = dotProduct;
            max = dotProduct;

            for (int i = 0; i < 4; i++)
            {
                dotProduct = Vector2.Dot(this.points[i], axis);
                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else
                {
                    if (dotProduct > max)
                    {
                        max = dotProduct;
                    }
                }
            }
        }

        public override void Draw(PrimitiveBatch primitiveBatch)
        {
            for (int pointA = 0, pointB = 0; pointA < 4; pointA++)
            {
                if (pointA + 1 == this.points.Length)
                {
                    pointB = 0;
                }
                else
                {
                    pointB = pointA + 1;
                }

                primitiveBatch.AddVertex(this.points[pointA], Color.Blue);
                primitiveBatch.AddVertex(this.points[pointB], Color.Blue);
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

            Box body = (Box)obj;

            return this.owner.Id == body.owner.Id &&
                this.id == body.id &&
                this.x == body.x &&
                this.y == body.y &&
                this.width == body.width &&
                this.height == body.width;
        }

        public override int GetHashCode()
        {
            const int multiplier = 8;
            if (hashCode == 0)
            {
                int code = 13;
                code = multiplier * code + owner.Id.GetHashCode();
                code = multiplier * code + id.GetHashCode();
                code = multiplier * code + width.GetHashCode();
                code = multiplier * code + height.GetHashCode();
                code = multiplier * code;
                hashCode = code;
            }
            return hashCode;
        }
        #endregion
    }
}