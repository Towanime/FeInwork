using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeInwork.core.collision.bodies;
using FeInwork.Core.Interfaces;
using Microsoft.Xna.Framework;
using FeInwork.Core.Util;
using FeInwork.FeInwork.util;

namespace FeInwork.core.collision
{
    public static class ShapeFactory
    {
        /// <summary>
        /// Recibe un string delimitado por espcio [campos] y , [registros]
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        public static List<CollisionBody> CreateShapeFromString(String instructions, IEntity owner)
        {
            List<CollisionBody> collisionsResolved = new List<CollisionBody>();
            String[] registers = instructions.Split(',');
            for (int i = 0; i < registers.Length; i++)
            {
                String[] fields = registers[i].Split(' ');
                if (fields.Length > 5)
                {
                    switch (fields[0])
                    { 
                        case "Rectangle":
                            collisionsResolved.Add(CreateRectangle(fields[1], owner, Boolean.Parse(fields[2]), true,
                                float.Parse(fields[3]), float.Parse(fields[4]), new Vector2(float.Parse(fields[5]), 
                                    float.Parse(fields[6])) + owner.getVectorProperty(EntityProperty.Position), Boolean.Parse(fields[7])));
                            break;
                        case "Circle":
                            collisionsResolved.Add(CreateCircle(fields[1], owner, Boolean.Parse(fields[2]), true, float.Parse(fields[3]),
                                new Vector2(float.Parse(fields[4]), float.Parse(fields[5])) + owner.getVectorProperty(EntityProperty.Position), Boolean.Parse(fields[6])));
                            break;
                        case "Polygon":
                            collisionsResolved.Add(CreatePolygon(fields[1], owner, Boolean.Parse(fields[2]), true, getPointListFromStringArray(fields, 3, fields.Length - 3),
                                new Vector2(float.Parse(fields[fields.Length - 3]), float.Parse(fields[fields.Length - 2])) + owner.getVectorProperty(EntityProperty.Position),
                                Boolean.Parse(fields[fields.Length - 1])));
                            break;
                    }
                }
                else //Crea shapes no relativas a owner
                {
                    switch (fields[0])
                    {
                        case "Rectangle":
                            collisionsResolved.Add(CreateRectangle(fields[1], owner, Boolean.Parse(fields[2]), true,
                                float.Parse(fields[3]), float.Parse(fields[4])));
                            break;
                        case "Circle":
                            collisionsResolved.Add(CreateCircle(fields[1], owner, Boolean.Parse(fields[2]), true, float.Parse(fields[3])));
                            break;
                        case "Polygon":
                            collisionsResolved.Add(CreatePolygon(fields[1], owner, Boolean.Parse(fields[2]), true, 
                                getPointListFromStringArray(fields, 3, fields.Length)));
                            break;
                    }
                }   
            }

            return collisionsResolved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="beginIdx"></param>
        /// <param name="endIdx"></param>
        /// <returns></returns>
        public static List<Vector2> getPointListFromStringArray(String[] fields, int beginIdx, int endIdx)
        {
            //Check se creeen bien
            List<Vector2> points = new List<Vector2>();

            for (int i = beginIdx; i < endIdx; i+=2)
            {
                points.Add(new Vector2(float.Parse(fields[i]), float.Parse(fields[i+1])));
            }

            return points;
        }

        /// <summary>
        /// Construye un rectangulo según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="width">Ancho del rectangulo</param>
        /// <param name="height">Alto del rectangulo</param>
        /// <returns>Rectangulo generado</returns>
        public static Box CreateRectangle(string id, IEntity owner, bool isSolid, bool relativeToFacing, float width, float height)
        {
            Box box = new Box(id, 0, 0, width, height, owner, isSolid);

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    box.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return box;
        }

        /// <summary>
        /// Construye un rectangulo según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="width">Ancho del rectangulo</param>
        /// <param name="height">Alto del rectangulo</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren 
        /// al primer punto del body o al centro de este</param>
        /// <returns>Rectangulo generado</returns>
        public static Box CreateRectangle(string id, IEntity owner, bool isSolid, bool relativeToFacing, float width, float height, Vector2 startPosition, bool startCenter)
        {
            if (startCenter)
            {
                startPosition.X -= width / 2;
                startPosition.Y -= height / 2;
            }
            Box box = new Box(id, startPosition.X, startPosition.Y, width, height, owner, isSolid);

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    box.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return box;
        }

        /// <summary>
        /// Construye un rectangulo según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="width">Ancho del rectangulo</param>
        /// <param name="height">Alto del rectangulo</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren 
        /// al primer punto del body o al centro de este</param>
        /// <param name="layer">Capa de colisión donde se encontrará el body</param>
        /// <returns>Rectangulo generado</returns>
        public static Box CreateRectangle(string id, IEntity owner, bool isSolid, bool relativeToFacing, float width, float height, Vector2 startPosition, bool startCenter, float layer)
        {
            if (startCenter)
            {
                startPosition.X -= width / 2;
                startPosition.Y -= height / 2;
            }
            Box box = new Box(id, startPosition.X, startPosition.Y, width, height, owner, isSolid);
            box.Layer = layer;

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    box.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return box;
        }

        /// <summary>
        /// Construye un rectangulo según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="width">Ancho del rectangulo</param>
        /// <param name="height">Alto del rectangulo</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren 
        /// al primer punto del body o al centro de este</param>
        /// <param name="layer">Capa de colisión donde se encontrará el body</param>
        /// <param name="colorTag">Tag de color especifico para responses que solo se activen ante dicho Tag</param>
        /// <returns>Rectangulo generado</returns>
        public static Box CreateRectangle(string id, IEntity owner, bool isSolid, bool relativeToFacing, float width, float height, Vector2 startPosition, bool startCenter, float layer, Color colorTag)
        {
            if (startCenter)
            {
                startPosition.X -= width / 2;
                startPosition.Y -= height / 2;
            }

            Box box = new Box(id, startPosition.X, startPosition.Y, width, height, owner, isSolid);
            box.Layer = layer;
            box.ColorTag = colorTag;

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    box.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return box;
        }

        /// <summary>
        /// Construye un poligono según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="polygonPoints">Coordenadas de cada punto del poligono</param>
        /// <returns>Poligono generado</returns>
        public static Polygon CreatePolygon(string id, IEntity owner, bool isSolid, bool relativeToFacing, List<Vector2> polygonPoints)
        {
            Polygon polygon = new Polygon(id, polygonPoints, owner, isSolid);

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    polygon.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return polygon;
        }

        /// <summary>
        /// Construye un poligono según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="polygonPoints">Coordenadas de cada punto del poligono</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren
        /// al punto de inicio desde donde se crea el body o al centro del mismo</param>
        /// <returns>Poligono generado</returns>
        public static Polygon CreatePolygon(string id, IEntity owner, bool isSolid, bool relativeToFacing, List<Vector2> polygonPoints, Vector2 startPosition, bool startCenter)
        {
            Polygon polygon = new Polygon(id, polygonPoints, owner, isSolid, startPosition);

            if (startCenter)
            {
                polygon.Offset(startPosition.X - polygon.Center.X, startPosition.Y - polygon.Center.Y);
            }

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    polygon.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return polygon;
        }

        /// <summary>
        /// Construye un poligono según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="polygonPoints">Coordenadas de cada punto del poligono</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren
        /// al punto de inicio desde donde se crea el body o al centro del mismo</param>
        /// <returns>Poligono generado</returns>
        /// <param name="layer">Capa de colisión donde se encontrará el body</param>
        /// <returns>Poligono generado</returns>
        public static Polygon CreatePolygon(string id, IEntity owner, bool isSolid, bool relativeToFacing, List<Vector2> polygonPoints, Vector2 startPosition, bool startCenter, float layer)
        {
            Polygon polygon = new Polygon(id, polygonPoints, owner, isSolid, startPosition);
            polygon.Layer = layer;

            if (startCenter)
            {
                polygon.Offset(startPosition.X - polygon.Center.X, startPosition.Y - polygon.Center.Y);
            }

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    polygon.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return polygon;
        }

        /// <summary>
        /// Construye un poligono según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="polygonPoints">Coordenadas de cada punto del poligono</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren
        /// al punto de inicio desde donde se crea el body o al centro del mismo</param>
        /// <returns>Poligono generado</returns>
        /// <param name="layer">Capa de colisión donde se encontrará el body</param>
        /// <param name="colorTag">Tag de color especifico para responses que solo se activen ante dicho Tag</param>
        /// <returns>Poligono generado</returns>
        public static Polygon CreatePolygon(string id, IEntity owner, bool isSolid, bool relativeToFacing, List<Vector2> polygonPoints, Vector2 startPosition, bool startCenter, float layer, Color colorTag)
        {
            Polygon polygon = new Polygon(id, polygonPoints, owner, isSolid, startPosition);
            polygon.Layer = layer;
            polygon.ColorTag = colorTag;

            if (startCenter)
            {
                polygon.Offset(startPosition.X - polygon.Center.X, startPosition.Y - polygon.Center.Y);
            }

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    polygon.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return polygon;
        }

        /// <summary>
        /// Construye un circulo según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="radius">Radio del circulo</param>
        /// <returns>Circulo generado</returns>
        public static Sphere CreateCircle(string id, IEntity owner, bool isSolid, bool relativeToFacing, float radius)
        {
            Sphere sphere = new Sphere(id, Vector2.Zero, radius, owner, isSolid);

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    sphere.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return sphere;
        }

        /// <summary>
        /// Construye un circulo según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="radius">Radio del circulo</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren
        /// al punto de inicio desde donde se crea el body o al centro del mismo</param>
        /// <returns>Circulo generado</returns>
        public static Sphere CreateCircle(string id, IEntity owner, bool isSolid, bool relativeToFacing, float radius, Vector2 startPosition, bool startCenter)
        {
            if (startCenter)
            {
                startPosition.X -= radius;
                startPosition.Y -= radius;
            }
            Sphere sphere = new Sphere(id, startPosition, radius, owner, isSolid);

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    sphere.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return sphere;
        }

        /// <summary>
        /// Construye un circulo según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="radius">Radio del circulo</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren
        /// al punto de inicio desde donde se crea el body o al centro del mismo</param>
        /// <param name="layer">Capa de colisión donde se encontrará el body</param>
        /// <returns>Circulo generado</returns>
        public static Sphere CreateCircle(string id, IEntity owner, bool isSolid, bool relativeToFacing, float radius, Vector2 startPosition, bool startCenter, float layer)
        {
            if (startCenter)
            {
                startPosition.X -= radius;
                startPosition.Y -= radius;
            }

            Sphere sphere = new Sphere(id, startPosition, radius, owner, isSolid);
            sphere.Layer = layer;

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    sphere.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return sphere;
        }

        /// <summary>
        /// Construye un circulo según las opciones dadas
        /// </summary>
        /// <param name="id">Id del body</param>
        /// <param name="owner">Owner del body</param>
        /// <param name="isSolid">Si el body es sólido</param>
        /// <param name="relativeToFacing">Determina si se debe tomar en cuenta la dirección 
        /// a la que está mirando la entidad para posicionar el body 
        /// relativo a esa dirección</param>
        /// <param name="radius">Radio del circulo</param>
        /// <param name="startPosition">Coordenadas de inicio del body</param>
        /// <param name="startCenter">Determina si las coordenadas de inicio se refieren
        /// al punto de inicio desde donde se crea el body o al centro del mismo</param>
        /// <param name="layer">Capa de colisión donde se encontrará el body</param>
        /// <param name="colorTag">Tag de color especifico para responses que solo se activen ante dicho Tag</param>
        /// <returns>Circulo generado</returns>
        public static Sphere CreateCircle(string id, IEntity owner, bool isSolid, bool relativeToFacing, float radius, Vector2 startPosition, bool startCenter, float layer, Color colorTag)
        {
            if (startCenter)
            {
                startPosition.X -= radius;
                startPosition.Y -= radius;
            }
            Sphere sphere = new Sphere(id, startPosition, radius, owner, isSolid);
            sphere.Layer = layer;
            sphere.ColorTag = colorTag;

            if (relativeToFacing)
            {
                bool facingRight = owner.getState(EntityState.FacingRight);
                if (facingRight != true)
                {
                    sphere.MirrorHorizontal(owner.getVectorProperty(EntityProperty.Position));
                }
            }

            return sphere;
        }
    }
}
