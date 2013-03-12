using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FeInwork.core.collision.bodies;

namespace FeInwork.core.collision
{
    public struct CollisionResult
    {
        // Si los poligonos intersectarán
        public bool willIntersect;
        // Si los poligonos están intersectando
        public bool intersect;
        // Cuerpo que está causando la colisión
        public CollisionBody triggeringBody;
        // Cuerpo afectado por la colisión
        public CollisionBody affectedBody;
        // La minima distancia necesaria a aplicar al primer poligono para poder apartarlos
        public Vector2 minimumTranslationVector;
        // Eje de transición
        public Vector2 translationAxis;
    }
}
