using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace _2DPlatformer.GL
{
    class Level
    {
        public Texture2D Texture
        {
            get { return Art.GetContent<Texture2D>("art/UI/level/btnBg"); }
        }
        public Rectangle Rect { get; set; }

        public string Name { get; set; }

        public bool clicked { get; set; }

        public Level(string name, Vector2 position)
        {
            Name = name;
            Rect = new Rectangle((int)position.X,(int)position.Y,(int)Texture.Width,(int)Texture.Height);
        }
    }

    public class Score
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    class LevelSelector : Screen
    {
        List<Level> levels = new List<Level>();

        List<Score> scores = new List<Score>();

        MouseState mouseState;
        MouseState oldMouseState;

        int currentPage = 0;
        int pages = 0;

        Rectangle arrowRight = new Rectangle(1715, 693, 59, 47);
        Rectangle arrowLeft = new Rectangle(1628, 693, 59, 47);

        public LevelSelector()
        {

        }

        public override void Initialize()
        {
            List<string> levelnames = Directory.GetFiles("Levels").ToList();
            pages = (int) Math.Floor((double)((levelnames.Count-1) / 15));
            int left = 117;
            int top = 117;
            int levelsInRow = 0;
            levels.Clear();
            for (int i = currentPage * 15; i < (levelnames.Count > 15 + 15 * currentPage ? 15 + 15 * currentPage : levelnames.Count); i++)
			{
                if (levelsInRow >= 5)
                {
                    left = 117;
                    top += 180;
                    levelsInRow = 0;
                }

                Vector2 pos = new Vector2(left,top);
			    levels.Add(new Level(levelnames.ElementAt(i),pos));
                levelsInRow ++;
                left += 346;
			}

            XmlSerializer xmlSerializer = new XmlSerializer(scores.GetType(),new Type[] { typeof(Score) });
            if (File.Exists("Scores.xml"))
            {
                XmlReader xmlReader = XmlReader.Create("Scores.xml");
                if (xmlSerializer.CanDeserialize(xmlReader))
                    scores = (List<Score>)xmlSerializer.Deserialize(xmlReader);
                xmlReader.Close();
            }
            //Load();
        }

        public override void LoadContent(ContentManager content)
        {

        }

        public override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState(); 
            Vector2 mousePos = Game1.Instance._resolutionIndependence.ScaleMouseToScreenCoordinates(new Vector2(mouseState.X, mouseState.Y));
            Rectangle mouseRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1);
            if (mouseState.LeftButton == ButtonState.Pressed)
                levels.ForEach(x => x.clicked = x.Rect.Intersects(mouseRect));

            if (mouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed)
            {
                Level level = levels.Find(x => x.clicked);
                if (level != null && level.Rect.Intersects(mouseRect))
                    ScreenManager.PushScreen(new GameScreen(level.Name),false);

                Rectangle back = new Rectangle(16, (int)Game1.ScreenSize.Y - 310, 173, 173);
                if (back.Intersects(mouseRect))
                    ScreenManager.PopScreen(true);

                if (arrowLeft.Intersects(mouseRect) && currentPage > 0)
                {
                    currentPage--;
                    Initialize();
                }
                if (arrowRight.Intersects(mouseRect) && currentPage < pages)
                {
                    currentPage++;
                    Initialize();
                }
            }

            if (mouseState.LeftButton == ButtonState.Released)
                levels.ForEach(x => x.clicked = false);

            oldMouseState = mouseState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture =  Art.GetContent<Texture2D>("art/UI/level/bg");
            spriteBatch.Draw(texture, new Vector2(0, Game1.ScreenSize.Y - texture.Height), Color.White);

            SpriteFont font = Art.Font;
            foreach (Level level in levels)
            {
                spriteBatch.Draw(level.Texture, level.Rect, Color.White);
                string name = level.Name.Remove(level.Name.Length - 4).Remove(0, 7);
                if (name.Length > 18)
                    name = name.Remove(18) + "...";
                spriteBatch.DrawString(font, name, new Vector2(level.Rect.X + ((level.Rect.Width - font.MeasureString(name).X) / 2), level.Rect.Y + 24), Color.Black);
                
                int score = 0;
                if (scores.Find(x => x.Name == level.Name) != null)
                    score = scores.Find(x => x.Name == level.Name).Value;
                int left = level.Rect.X + 34;
                for (int i = 0; i < 3; i++)
                {
                    if (score > 0)
                        texture = Art.GetContent<Texture2D>("art/UI/level/StarActive");
                    else
                        texture = Art.GetContent<Texture2D>("art/UI/level/StarInactive");

                    spriteBatch.Draw(texture, new Vector2(left, level.Rect.Y + 55), Color.White);

                    left += 77;
                    score--;
                }
            }

            if (pages > 0 && currentPage < pages)
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/UI/level/ArrowRight"), arrowRight, Color.White);
            if (currentPage > 0)
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/UI/level/ArrowLeft"), arrowLeft, Color.White);

            Vector2 mousePos = Game1.Instance._resolutionIndependence.ScaleMouseToScreenCoordinates(new Vector2(mouseState.X, mouseState.Y));
            spriteBatch.Draw(Art.Cursor, mousePos, Color.White);
        }
    }
}
