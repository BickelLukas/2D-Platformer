using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    public abstract class Entity
    {
        protected Texture2D image;
        public Color color = Color.White;

        
        public Vector2 Position, Velocity;
        public float Orientation;
        public float Radius = 20;
        public bool IsExpired;

        public virtual Rectangle Bounds { get { return new Rectangle((int)(Position.X),(int)(Game1.ScreenSize.Y - Position.Y - Size.Y),(int)Size.X,(int)Size.Y);} }

        public virtual Vector2 Size
        {
            get
            {
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(SpriteBatch spriteBatch,float scale = 1f)
        {
            spriteBatch.Draw(image, new Vector2(Position.X * scale, Game1.ScreenSize.Y - (Position.Y * scale)), null, color, Orientation, new Vector2(0, Size.Y), scale, 0, 0);
            //spriteBatch.Draw(Art.GetContent<Texture2D>("art/rect"), Bounds, Color.White);
        }

        public virtual Entity Clone(Vector2 pos)
        {
            return this;
        }
    }
}
