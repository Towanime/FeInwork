using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AlchemistDemo.core.Base
{
	class Particle
	{
        Texture2D particleBase;
        Vector2 position;
        Vector2 direction;
        float radius;
        float color;
        float life;
        
        public Particle(Texture2D particleBase, Vector2 position, Vector2 direction,
                        float radius, float color, float life)
        {
            this.particleBase = particleBase;
            this.position = position;
            this.direction = direction;
            this.radius = radius;
            this.color = color;
            this.life = life;
        }
 
        public bool update(float dt)
        {
            this.position += this.direction * dt;
            this.life -= dt;
            if(this.life > 0)
            {
                return true;
            }
            return false;
        }
 
        public void draw(SpriteBatch spriteBatch)
        {
            // Draw the sprite with the right scale on the right position
        }
	}
}
