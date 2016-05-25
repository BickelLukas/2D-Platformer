using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    public class EnemyAnimation
    {
        public string type;

        public string direction;

        private bool _dead = false;

        public bool Dead
        {
            get { return _dead = false; }
            set { _dead = value; }
        }

        public Vector2 Size
        {
            get
            {
                return texture == null ? Vector2.Zero : new Vector2(texture.Width, texture.Height);
            }
        }
        

        private Texture2D texture;

        public float frameTime = 1f;
        //public int frames { get; set; }
        //public int width { get; set; }
        float elapsed;
        int currentFrame = 1;

        public EnemyAnimation()
        {
            //texture = Art.GetContent<Texture2D>("art/enemies/fish/right/anim1");
        }

        public EnemyAnimation(string type, string direction)//Texture2D texture, int frameTime, int frames, int width)
        {
            //this.Texture = texture;
            this.type = type;
            this.direction = direction;

            texture = Art.GetContent<Texture2D>("art/enemies/" + type + "/" + direction + "/anim1");

            this.frameTime = 0.1f;//frameTime;
            //this.frames = frames;
            //this.width = width;
        }

        public void Update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= frameTime)
            {
                currentFrame = currentFrame==2?1:2;

                elapsed -= frameTime;
            }

            if (!_dead)
                texture = Art.GetContent<Texture2D>("art/enemies/" + type + "/" + direction + "/anim" + currentFrame);
            else
                texture = Art.GetContent<Texture2D>("art/enemies/" + type + "/" + direction + "/dead");

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 Position, Vector2 Size, Color color, float scale, float Orientation)
        {
            if (texture == null)
                texture = Art.GetContent<Texture2D>("art/enemies/" + type + "/" + direction + "/anim1");
            spriteBatch.Draw(texture, new Vector2(Position.X * scale, Game1.ScreenSize.Y - (Position.Y * scale)), null, color, Orientation, new Vector2(0, texture.Height), scale, 0, 0);
        }
    }
}
