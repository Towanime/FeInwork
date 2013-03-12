using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FeInwork.FeInwork.util
{
    public struct DrawStringParameters
    {
        private SpriteFont spriteFont;
        private string text;
        private StringBuilder sbText;
        private Vector2 position;
        private Color color;
        private float rotation;
        private Vector2 origin;
        private Vector2 scale;
        private SpriteEffects effects;
        private float layerDepth;
        private bool draw;

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
            set { spriteFont = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public StringBuilder SbText
        {
            get { return sbText; }
            set { sbText = value; }
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

        public bool Draw
        {
            get { return draw; }
            set { draw = value; }
        }
    }
}
