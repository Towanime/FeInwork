using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FeInwork.core.collision
{
    public struct Line
    {
        private Vector2 startPoint;
        private Vector2 endPoint;

        public Line(Vector2 startPoint, Vector2 endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public Vector2 StartPoint
        {
            get
            {
                return this.startPoint;
            }
            set
            {
                this.startPoint = value;
            }
        }

        public Vector2 EndPoint
        {
            get
            {
                return this.endPoint;
            }
            set
            {
                this.endPoint = value;
            }
        }

        public float Lenght
        {
            get
            {
                return Vector2.Distance(this.startPoint, this.endPoint);
            }
        }

        public Vector2 Edge
        {
            get
            {
                Vector2 edge;
                Vector2.Subtract(ref this.endPoint, ref this.startPoint, out edge);
                return edge;
            }
        }
    }
}
