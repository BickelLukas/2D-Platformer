using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    public class Coin : Entity
    {
        public bool collected { get; set; }

        private const float frameTime = 0.05f;
        private float elapsed = 0;
        private int currentFrame = 0;

        private bool collectable = true;

        public bool Collectable
        {
            get { return collectable; }
            set { collectable = value; }
        }

        public Coin()
        {
            this.image = Art.Coin;
        }

        public Coin(Vector2 location)
            : this()
        {
            Position = location;
        }

        public override void Update(GameTime gameTime)
        {
            if (Velocity.X > 0 )
                Velocity.X -= 0.1f;
            if (Velocity.Y > 0)
                Velocity.Y -= 0.1f;

            if (Velocity.X < 0)
                Velocity.X = 0;

            if (Velocity.Y < 0)
                Velocity.Y = 0;

            Position += Velocity;

            if (!collected)
            {
                if (collectable == false)
                {
                    elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (elapsed >= 0.5f)
                    {
                        Collectable = true;
                    }
                }
            }
            else
            {
                if (currentFrame < 5)
                {
                    elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (elapsed >= frameTime)
                    {
                        currentFrame++;
                        //if (currentFrame == 1)
                        //    Sound.coin.Play();
                        if (currentFrame < 5)
                            this.image = Art.GetContent<Texture2D>("art/items/coin0" + currentFrame);
                        elapsed -= frameTime;
                    }
                }
                else
                    IsExpired = true;
            }
        }

        public override Entity Clone(Vector2 pos)
        {
            Coin clone = new Coin(pos);
            return clone;
        }
    }
}
