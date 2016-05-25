using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    public static class ScreenManager
    {

        static Stack<Screen> screenStack = new Stack<Screen>();
        static ContentManager Content;

        static bool animating;
        static float elapsed;
        static int position;

        public static void Initialize(ContentManager content)
        {
            Content = content;
        }

        public static void PushScreen(Screen screen, bool withAnimation)
        {
            screenStack.Push(screen);
            screenStack.Peek().Initialize();
            screenStack.Peek().LoadContent(Content);
            animating = withAnimation;
            position = (int) Game1.ScreenSize.X;
        }

        public static void PopScreen(bool withAnimation)
        {
            screenStack.Pop();
            screenStack.Peek().Initialize();
            screenStack.Peek().LoadContent(Content);
            if (screenStack.Count <= 0)
                Game1.Instance.Exit();

            animating = withAnimation;
            position = (int)-Game1.ScreenSize.X;
        }

        public static void Update(GameTime gameTime)
        {
            if (animating)
            {
                position += position>0?-100:100;
                if (Math.Abs(position) <= 100)
                    animating = false;

                screenStack.ElementAt(screenStack.Count - 1).Update(gameTime);
                screenStack.Peek().Update(gameTime);
            }
            else
            {
                screenStack.Peek().Update(gameTime);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (animating)
            {

                Matrix cameraTransform = Matrix.CreateTranslation(position >= 0 ? -Game1.ScreenSize.X + position : Game1.ScreenSize.X + position, 0.0f, 0.0f); 
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, cameraTransform * Game1.Instance._camera.GetViewTransformationMatrix());
                screenStack.ElementAt(screenStack.Count - 1).Draw(spriteBatch);

                cameraTransform = Matrix.CreateTranslation(position, 0.0f, 0.0f);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, cameraTransform * Game1.Instance._camera.GetViewTransformationMatrix());
                screenStack.Peek().Draw(spriteBatch);
            
            }
            else
            {
                screenStack.Peek().Draw(spriteBatch);
            }
        }
    }
}
