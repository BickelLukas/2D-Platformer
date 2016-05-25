using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    class Button
    {
        private string texture;

        public Texture2D Texture
        {
            get { return Art.GetContent<Texture2D>(texture + (clicked ? "_Click" : "")); }
        }
        public Rectangle Rect { get; set; }
        public bool Animating { get; set; }
        public Screen Target { get; set; }

        public bool clicked { get; set; }

        public Button (string texture, Rectangle rect, Screen target, bool animating)
	    {
            this.texture = texture;
            Rect = new Rectangle((rect.X - Texture.Width) / 2,rect.Y,rect.Width,rect.Height);
            Target = target;
            Animating = animating;
	    }
    }

    class MainMenu : Screen
    {
        //int selected = 0;

        //KeyboardState oldState;

        //List<MenuItem> menuItems = new List<MenuItem>() { new MenuItem("Start Game", new GameScreen()), new MenuItem("Level Editor", new LvlEditor()), new MenuItem("Exit", null) };

        List<Button> lstButtons = new List<Button>() { 
            new Button("art/UI/btnStart", new Rectangle((int)Game1.ScreenSize.X, 224, 461, 104), new LevelSelector(),true), 
            new Button("art/UI/btnEditor", new Rectangle((int)Game1.ScreenSize.X, 340, 461, 80),new LvlEditor(),false), 
            new Button("art/UI/btnCredits", new Rectangle((int)Game1.ScreenSize.X, 432, 461, 80),new SplashScreen(),false) 
        };

        MouseState mouseState;
        MouseState oldMouseState;

        public MainMenu()
        {
        }
        
        public override void Initialize()
        {

        }

        public override void LoadContent(ContentManager content)
        {

        }

        public override void Update(GameTime gameTime)
        {
            /*if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                if (menuItems.ElementAt(selected).Screen != null)
                    ScreenManager.PushScreen(menuItems.ElementAt(selected).Screen);
                else
                    Game1.Instance.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !oldState.IsKeyDown(Keys.Down))
                selected = (selected < menuItems.Count -1 ?selected+1:0);

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))
                selected = (selected > 0? selected - 1 : menuItems.Count -1);

            oldState = Keyboard.GetState();*/
            mouseState = Mouse.GetState();
            Vector2 mousePos = Game1.Instance._resolutionIndependence.ScaleMouseToScreenCoordinates(new Vector2(mouseState.X, mouseState.Y));
            Rectangle mouseRect = new Rectangle((int)mousePos.X,(int)mousePos.Y, 1, 1);
            if (mouseState.LeftButton == ButtonState.Pressed)
                lstButtons.ForEach(x => x.clicked = x.Rect.Intersects(mouseRect));

            if (mouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
            {
                Button btn = lstButtons.Find(x => x.clicked);
                if (btn != null && btn.Rect.Intersects(mouseRect))
                    ScreenManager.PushScreen(btn.Target,btn.Animating);

                Rectangle exit = new Rectangle((int)Game1.ScreenSize.X - 215, (int) Game1.ScreenSize.Y - 310, 173, 173);
                if (exit.Intersects(mouseRect))
                    Game1.Instance.Exit();
            }

            if (mouseState.LeftButton == ButtonState.Released)
                lstButtons.ForEach(x => x.clicked = false);

            oldMouseState = mouseState;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteFont currentFont = Art.Font;
            /*string text = "Main Menu";
            spriteBatch.DrawString(currentFont, text, new Vector2((Game1.Instance.GraphicsDevice.Viewport.Width - currentFont.MeasureString(text).X) / 2, 100), Color.Black);
            
            for (int i = 0; i < menuItems.Count; i++)
            {
                text = menuItems.ElementAt(i).Text;
                if (selected == i)
                    currentFont = Art.BoldFont;
                else
                    currentFont = Art.Font;
                spriteBatch.DrawString(currentFont, text, new Vector2((Game1.Instance.GraphicsDevice.Viewport.Width - currentFont.MeasureString(text).X) / 2, 150 + 20 * i), Color.Black);
            }*/
            Texture2D texture = Art.GetContent<Texture2D>("art/UI/btnBack");
            spriteBatch.Draw(texture, new Vector2((Game1.ScreenSize.X - texture.Width) / 2, 123), Color.White);

            texture = Art.GetContent<Texture2D>("art/UI/bg");
            spriteBatch.Draw(texture, new Vector2(0, Game1.ScreenSize.Y - texture.Height), Color.White);

            foreach (Button btn in lstButtons)
            {
                spriteBatch.Draw(btn.Texture, btn.Rect, Color.White);
            }
            Vector2 mousePos = Game1.Instance._resolutionIndependence.ScaleMouseToScreenCoordinates(new Vector2(mouseState.X, mouseState.Y));
            spriteBatch.Draw(Art.Cursor, mousePos, Color.White);
        }
    }
}
