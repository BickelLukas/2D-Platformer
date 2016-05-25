using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    class SplashScreen : Screen
    {
        float elapsed;
        Texture2D texture;

        public SplashScreen()
        {
        }
        
        public override void Initialize()
        {
        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("art/Logo");
        }

        public override void Update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= 3)
                texture = Art.GetContent<Texture2D>("art/kenney");
            if (elapsed >= 5)
            {
                elapsed = 0;
                ScreenManager.PopScreen(false);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, (Game1.ScreenSize - new Vector2(texture.Width, texture.Height)) / 2, Color.White);
        }
    }
}
