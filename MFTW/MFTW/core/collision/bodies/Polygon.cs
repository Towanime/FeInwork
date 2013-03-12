using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FeInwork.Core.Base;
using FeInwork.Core.Interfaces;
using FeInwork.core.util;

namespace FeInwork.core.collision.bodies
{
    public class Polygon : CollisionBody
    {
        #region Variables
        /// <summary>
        /// Vertices de los cuales constituye el poligono
        /// </summary>
        private List<Vector2> vertices = new List<Vector2>();
        /// <summary>
        /// Lados de los cuales constituye el poligono
        /// </summary>
        private List<Line> sides = new List<Line>();
        #endregion

        #region Constructors
        /// <summary>
        /// Crea un nuevo poligono de n lados utilizando la posicion dada
        /// por los vertices
        /// </summary>
        /// <param name="vertices">Lista de vertices de las cuales 
        /// se constituye el poligono</param>
        /// <param name="owner">Entidad dueña de este poligono</param>
        /// <param name="isSolid">Determina si el poligono es sólido o no</param>
        public Polygon(string id, List<Vector2> vertices, IEntity owner, bool isSolid)
            : base(owner, id, isSolid)
        {
            this.vertices = vertices;
            this.BuildSides();
            this.CalculateCenter();
            this.BuildAABB();
        }

        /// <summary>
        /// Crea un nuevo poligono de n lados a partir de una posicion
        /// inicial obtenida
        /// </summary>
        /// <param name="vertices">Lista de vertices de las cuales 
        /// se constituye el poligono</param>
        /// <param name="owner">Entidad dueña de este poligono</param>
        /// <param name="isSolid">Determina si el poligono es sólido o no</param>
        /// <param name="startCoordinates">Posicion inicial desde la cual se creará el poligono</param>
        public Polygon(string id, List<Vector2> vertices, IEntity owner, bool isSolid, Vector2 startCoordinates)
            : base(owner, id, isSolid)
        {
            this.vertices = vertices;
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] += startCoordinates;
            }
            this.BuildSides();
            this.CalculateCenter();
            this.BuildAABB();
        } 
        #endregion

        #region Properties
        public List<Line> Sides
        {
            get { return sides; }
        }

        public List<Vector2> Vertices
        {
            get { return vertices; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Construye los lados del poligono
        /// en base a los vertices.
        /// Nota: Es necesario llamar cada vez que se 
        /// altere alguno de los vertices del poligono
        /// </summary>
        private void BuildSides()
        {
            Vector2 p1;
            Vector2 p2;
            this.sides.Clear();
            for (int i = 0; i < this.vertices.Count; i++)
            {
                p1 = this.vertices[i];
                if (i + 1 >= this.vertices.Count)
                {
                    p2 = this.vertices[0];
                }
                else
                {
                    p2 = this.vertices[i + 1];
                }
                this.sides.Add(new Line(p1, p2));
            }
        }

        /// <summary>
        /// Calcula el centro del polígono
        /// </summary>
        private void CalculateCenter()
        {
            float totalX = 0;
            float totalY = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                totalX += vertices[i].X;
                totalY += vertices[i].Y;
            }

            center = new Vector2(totalX / (float)vertices.Count, totalY / (float)vertices.Count);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Verifica si este poligono esta colisionando o va a colisionar
        /// con otro poligono dado.
        /// Esta verificación se realiza utilizando SAT (Separating Axis Theorem) la cual explica
        /// que, si existe una linea en el plano que pueda separar dos distintas figuras en diferentes
        /// lados por completo, entonces estas figuras no están colisionando.
        /// Nota: Este teorema solo debería aplicarse ante poligonos concavos, de otra forma el algoritmo
        /// no daria una respuesta correcta ante poligonos convexos.
        /// </summary>
        /// <param name="polygon2">Segundo poligono con el cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera el poligono principal (this)</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected override CollisionResult IntersectWithPolygon(Polygon polygon2, ref Vector2 distance)
        {
            CollisionResult result = new CollisionResult();
            result.triggeringBody = this;
            result.affectedBody = polygon2;
            // Se asume que ambos poligonos intersectan hasta que
            // se pueda demostrar lo contrario
            result.intersect = true;
            result.willIntersect = true;
            // Se obtiene la cantidad de lados del poligono principal
            // y del segundo poligono
            int sideCountA = this.sides.Count;
            int sideCountB = polygon2.Sides.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            // Lado actual a analizar
            Line currentSide;

            // Se realiza una verificacion por cada uno de los lados de ambos poligonos
            // hasta que pueda encontrarse alguna separación entre ambos o hasta que se hayan
            // verificado todos los lados
            for (int sideIndex = 0; sideIndex < sideCountA + sideCountB; sideIndex++)
            {
                if (sideIndex < sideCountA)
                {
                    currentSide = this.sides[sideIndex];
                }
                else
                {
                    currentSide = polygon2.Sides[sideIndex - sideCountA];
                }
                // Se obtiene un Vector perpendicular al lado actual con valores unitarios,
                // esto servira para obtener el eje sobre el cual deben hacerse las proyecciones
                Vector2 axis = new Vector2(-currentSide.Edge.Y, currentSide.Edge.X);
                axis.Normalize();

                // Se proyectan ambos poligonos sobre el mismo eje
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                this.Project(ref axis, out minA, out maxA);
                polygon2.Project(ref axis, out minB, out maxB);

                // Se obtiene el intervalo entre ambos poligonos sobre el eje,
                // si el intervalo (separación) es mayor a 0 entonces los poligonos
                // no están intersectando
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.intersect = false;

                // Luego se realizan los mismos calculos pero sumando el vector
                // de velocidad al poligono en (posible) movimiento
                float velocityProjection = Vector2.Dot(axis, distance);

                //if (velocityProjection < 0)
                //{
                    minA += velocityProjection;
                //}
                //else
                //{
                    maxA += velocityProjection;
                //}

                // Si el intervalo de distancia es menor a 0 con el poligono en movimiento,
                // entonces los poligonos tambien intersectaran al moverse
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
                // a los poligonos en esa dirección
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = this.Center - polygon2.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                    {
                        translationAxis = -translationAxis;
                    }
                }
            }

            // El vector minimo de transición
            // servira para separar ambos poligonos en caso de que colisionen.
            if (result.willIntersect)
            {
                result.minimumTranslationVector =
                       translationAxis * minIntervalDistance;
            }

            result.translationAxis = translationAxis;
            return result;
        }

        /// <summary>
        /// Verifica si este poligono esta colisionando o va a colisionar
        /// con una caja (rectangulo) dada
        /// Esta verificación se realiza utilizando SAT (Separating Axis Theorem) la cual explica
        /// que, si existe una linea en el plano que pueda separar dos distintas figuras en diferentes
        /// lados por completo, entonces estas figuras no están colisionando.
        /// Nota: Este teorema solo debería aplicarse ante poligonos concavos, de otra forma el algoritmo
        /// no daria una respuesta correcta ante poligonos convexos.
        /// </summary>
        /// <param name="box">Caja con la cual se hará la verificacion</param>
        /// <param name="distance">Distancia por la cual se movera este poligono</param>
        /// <param name="result">Referencia del objeto que contendrá la información necesaria sobre 
        /// si se esta intersectando, si se intersectará y la distancia necesaria 
        /// para evitar una intersección</param>
        protected override CollisionResult IntersectWithBox(Box box, ref Vector2 distance)
        {
            CollisionResult result = new CollisionResult();
            result.triggeringBody = this;
            result.affectedBody = box;
            // Se asume que ambas figuras intersectan hasta que
            // se pueda demostrar lo contrario
            result.intersect = true;
            result.willIntersect = true;
            // Se obtiene la cantidad de lados del poligono y del
            // rectangulo (Para el rectangulo solo es necesario estudiar dos lados)
            int sideCountPolygon = this.sides.Count;
            int sideCountRectangle = 2;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            // Lado actual a analizar
            Line currentSide;

            // Se realiza una verificacion por cada uno de los lados del poligono y por dos lados
            // perpendiculares del rectangulo hasta que pueda encontrarse alguna separación entre 
            // ambas o hasta que se hayan verificado todos los lados
            for (int sideIndex = 0; sideIndex < sideCountPolygon + sideCountRectangle; sideIndex++)
            {
                if (sideIndex < sideCountPolygon)
                {
                    currentSide = this.sides[sideIndex];
                }
                else
                {
                    if (sideIndex - sideCountPolygon == 0)
                    {
                        currentSide = box.Sides[0];
                    }
                    else
                    {
                        currentSide = box.Sides[1];
                    }
                }

                // Se obtiene un Vector perpendicular al lado actual con valores unitarios,
                // esto servira para obtener el eje sobre el cual deben hacerse las proyecciones
                Vector2 axis = new Vector2(-currentSide.Edge.Y, currentSide.Edge.X);
                axis.Normalize();

                // Se proyectan ambas figuras sobre el mismo eje
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                this.Project(ref axis, out minA, out maxA);
                box.Project(ref axis, out minB, out maxB);

                // Se obtiene el intervalo entre ambas figuras sobre el eje,
                // si el intervalo (separación) es mayor a 0 entonces las figuras
                // no están intersectando
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.intersect = false;

                // Luego se realizan los mismos calculos pero sumando el vector
                // de velocidad al poligono en (posible) movimiento
                float velocityProjection = Vector2.Dot(axis, distance);

                //if (velocityProjection < 0)
                //{
                    minA += velocityProjection;
                //}
                //else
                //{
                    maxA += velocityProjection;
                //}

                // Si el intervalo de distancia es menor a 0 con el poligono en movimiento,
                // entonces las figuras tambien intersectaran al moverse
                intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.willIntersect = false;

                // Si ya sabemos que las figuras estan intersectando y van a intersectar,
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

                    Vector2 d = this.Center - box.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                    {
                        translationAxis = -translationAxis;
                    }
                }
            }

            // El vector minimo de transición
            // servira para separar ambas figuras en caso de que colisionen.
            if (result.willIntersect)
            {
                result.minimumTranslationVector =
                       translationAxis * minIntervalDistance;
            }

            result.translationAxis = translationAxis;
            return result;
        }

        /// <summary>
        /// Verifica si este poligono esta colisionando o va a colisionar
        /// con una esféra dada.
        /// Para esto se ubica el punto centro de la esfera en la región de voronoi
        /// que le corresponde según el poligono, de forma que obtenemos el punto
        /// mas cercano de la esfera al poligono junto al respectivo eje.
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
            // Se toman tanto la cantidad de puntos
            // como la cantidad de lados del poligono
            int sideCountPolygon = this.sides.Count;
            int pointCountPolygon = this.vertices.Count;
            // Dos valores distintos para guardar 
            float minIntervalDistanceAfterMove = float.PositiveInfinity;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();

            // Ahora se estudia cada lado del poligono para verificar si el centro
            // de la esfera se encuentra perpendicular a algun punto de ese lado
            for (int sideIndex = 0; sideIndex < sideCountPolygon; sideIndex++)
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
                    // que tenga el punto mas cercano desde la esfera hacia el poligono)
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
                    // que tenga el punto mas cercano desde la esfera hacia el poligono)
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

            // Luego se estudia la distancia entre cada vertice del poligono
            // contra el centro de la esfera, si se encuentra alguna distancia
            // menor que las ya guardadas entonces se guarda
            for (int pointIndex = 0; pointIndex < pointCountPolygon; pointIndex++)
            {
                Vector2 currentPoint = this.vertices[pointIndex];
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
            // dentro o fuera del poligono
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
        /// Crea un rectangulo que sea capaz de contener todo el poligono por dentro
        /// modificando los valores minimos y maximos pasados por referencia
        /// </summary>
        /// <param name="minX">La coordenada en x del lado izquierdo</param>
        /// <param name="minY">La coordenada en y del lado superior</param>
        /// <param name="maxX">La coordenada en x del lado derecho</param>
        /// <param name="maxY">La coordenada en y del lado inferior</param>
        public override void BuildAABB(out float minX, out float minY, out float maxX, out float maxY)
        {
            Vector2 firstVertice = this.vertices[0];
            minX = firstVertice.X;
            minY = firstVertice.Y;
            maxX = firstVertice.X;
            maxY = firstVertice.Y;
            for (int i = 0; i < this.vertices.Count; i++)
            {
                Vector2 polygonPoint = this.vertices[i];
                if (polygonPoint.X < minX)
                {
                    minX = polygonPoint.X;
                }
                if (polygonPoint.X > maxX)
                {
                    maxX = polygonPoint.X;
                }
                if (polygonPoint.Y < minY)
                {
                    minY = polygonPoint.Y;
                }
                if (polygonPoint.Y > maxY)
                {
                    maxY = polygonPoint.Y;
                }
            }
        }

        /// <summary>
        /// Invierte la posicion de todos los puntos de este body
        /// en el eje horizontal como si de un espejo se tratase
        /// </summary>
        /// <param name="referencePoint">Punto de referencia para invertir</param>
        public override void MirrorHorizontal(Vector2 referencePoint)
        {
            for (int i = 0; i < this.vertices.Count; i++)
            {
                float posX = referencePoint.X - (this.vertices[i].X - referencePoint.X);
                float posY = this.vertices[i].Y;
                this.vertices[i] = new Vector2(posX, posY);
            }
            this.BuildSides();
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
            for (int i = 0; i < this.vertices.Count; i++)
            {
                float posY = referencePoint.Y - (this.vertices[i].Y - referencePoint.Y);
                float posX = this.vertices[i].X;
                this.vertices[i] = new Vector2(posX, posY);
            }
            this.BuildSides();
            this.CalculateCenter();
            this.BuildAABB(); 
        }

        /// <summary>
        /// Desplaza el poligono completo según una distancia
        /// </summary>
        /// <param name="x">Distancia a desplazar en x</param>
        /// <param name="y">Distancia a desplazar en y</param>
        public override void Offset(float x, float y)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 p = vertices[i];
                vertices[i] = new Vector2(p.X + x, p.Y + y);
            }
            this.BuildSides();
            this.CalculateCenter();
            this.BuildAABB(); 
        }

        /// <summary>
        /// Determina si el punto dado se encuentra dentro de este poligono
        /// </summary>
        /// <param name="point">Vector2 con las coordenadas del punto</param>
        /// <returns>True si el punto se encuentra dentro del poligono</returns>
        public override bool PointInBody(Vector2 point)
        {
            bool inside = false;

            // Se obtiene un rectangulo el cual contenga toda el area del poligono
            // para asi realizar una verificacion preliminar sobre si el punto se encuentra
            // dentro de ese rectangulo
            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            this.BuildAABB(out minX, out minY, out maxX, out maxY);

            // Si el punto si se encuentra dentro de la caja, se realiza una verificacion
            // mas especifica para saber si el punto se encuentra dentro del area del poligono
            if (!(point.X < minX || point.X > maxX || point.Y < minY || point.Y > maxY))
            {
                int j = this.Vertices.Count - 1;
                for (int i = 0; i < this.Vertices.Count; i++)
                {
                    if ((this.Vertices[i].Y < point.Y && this.Vertices[j].Y >= point.Y || this.Vertices[j].Y < point.Y && this.Vertices[i].Y >= point.Y)
                        && (this.Vertices[i].X <= point.X || this.Vertices[j].X <= point.X))
                    {
                        inside ^= (this.Vertices[i].X + (point.Y - this.Vertices[i].Y) / (this.Vertices[j].Y - this.Vertices[i].Y) * (this.Vertices[j].X - this.Vertices[i].X) < point.X);
                    }

                    j = i;

                    // NOTA: Mala implementacion, aun hay que arreglar esta parte
                    /* (point.Y > Math.Min(this.Sides[i].StartPoint.Y, this.Sides[i].EndPoint.Y))
                    {
                        if (point.Y <= Math.Max(this.Sides[i].StartPoint.Y, this.Sides[i].EndPoint.Y))
                        {
                            if (point.X <= Math.Max(this.Sides[i].StartPoint.X, this.Sides[i].EndPoint.X))
                            {
                                float xIntersection = this.Sides[i].StartPoint.X + ((point.Y - this.Sides[i].StartPoint.Y) / (this.Sides[i].EndPoint.Y - this.Sides[i].StartPoint.Y)) * (this.Sides[i].EndPoint.X - this.Sides[i].StartPoint.X);
                                if (point.X <= xIntersection)
                                {
                                    inside = true;
                                    break;
                                }
                            }
                        }
                    }*/
                }
            }

            return inside;
        }

        /// <summary>
        /// Se realiza una proyección de este poligono sobre un eje dado
        /// y cambia las referencias del valor minimo y maximo del poligono en el plano
        /// </summary>
        /// <param name="axis">Eje sobre el cual se hara la proyección</param>
        /// <param name="min">Float que contendra el valor minimo</param>
        /// <param name="max">Float que contendra el valor maximo</param>
        public override void Project(ref Vector2 axis, out float min, out float max)
        {
            float dotProduct = Vector2.Dot(axis, this.vertices[0]);
            min = dotProduct;
            max = dotProduct;
            for (int i = 0; i < this.vertices.Count; i++)
            {
                dotProduct = Vector2.Dot(this.vertices[i], axis);
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

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < vertices.Count; i++)
            {
                if (result != "") result += " ";
                result += "{" + vertices[i].ToString() + "}";
            }

            return result;
        }

        public override void Draw(PrimitiveBatch primitiveBatch)
        {
            for (int pointA = 0, pointB = 0; pointA < this.Vertices.Count; pointA++)
            {
                if (pointA + 1 == this.Vertices.Count)
                {
                    pointB = 0;
                }
                else
                {
                    pointB = pointA + 1;
                }

                primitiveBatch.AddVertex(this.Vertices[pointA], Color.Red);
                primitiveBatch.AddVertex(this.Vertices[pointB], Color.Red);
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

            Polygon body = (Polygon)obj;

            bool equals = this.owner.Id == body.owner.Id &&
                this.id == body.id && this.center == body.center &&
                this.vertices.Count == body.vertices.Count;

            if (equals)
            {
                for (int i = 0; i < this.vertices.Count; i++)
                {
                    if (!this.vertices[i].Equals(body.vertices[i])) return false;
                }
            }

            return equals;
        }

        public override int GetHashCode()
        {
            const int multiplier = 4;
            if (hashCode == 0)
            {
                int code = 6;
                code = multiplier * code + owner.Id.GetHashCode();
                code = multiplier * code + id.GetHashCode();
                code = multiplier * code + vertices.Count.GetHashCode();
                for (int i = 0; i < this.vertices.Count; i++)
                {
                    code = multiplier * code + vertices[i].GetHashCode();
                }
                code = multiplier * code;
                hashCode = code;
            }
            return hashCode;
        }
        #endregion
    }
}
