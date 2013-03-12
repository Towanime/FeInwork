using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FeInwork.FeInwork.util
{
    public struct DrawParameters
    {
        private Texture2D texture;
        private Rectangle sourceRectangle;
        private Vector2 position;
        private Color color;
        private float rotation;
        private Vector2 origin;
        private Vector2 scale;
        private SpriteEffects effects;
        /// <summary>
        /// Tambien se usa para parallax effect en los objetos ParallaxLayer.
        /// </summary>
        private float layerDepth;
        private float alpha;
        private bool draw;
        /// <summary>
        /// Esta es solo para parallax por ahora.
        /// </summary>
        private bool loop;
        private float moveSpeed;

        public DrawParameters(Texture2D texture)
        {
            this.texture = texture;
            this.sourceRectangle = Rectangle.Empty;
            this.position = Vector2.Zero;
            this.color = Color.White;
            this.rotation = 0;
            this.origin = Vector2.Zero;
            this.scale = Vector2.One;
            this.effects = SpriteEffects.None;
            this.layerDepth = GameLayers.MIDDLE_PLAY_AREA;
            this.alpha = 1;
            this.draw = true;
            this.loop = false;
            this.moveSpeed = 1;
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Rectangle SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public SpriteEffects Effects
        {
            get { return effects; }
            set { effects = value; }
        }

        public float LayerDepth
        {
            get { return layerDepth; }
            set { layerDepth = value; }
        }

        public float Alpha
        {
            get { return alpha; }
            set { alpha = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        public bool Draw
        {
            get { return draw; }
            set { draw = value; }
        }

        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        public float MoveSpeed
        {
            get { return this.moveSpeed; }
            set { this.moveSpeed = value; }
        }
    }
}
