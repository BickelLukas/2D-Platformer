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
    public class GameScreen : Screen
    {
        public static GameScreen currentGameScreen { get; private set; }

        EntityManager entityManager;

        private string FileName;

        int startCoins = 0;

        bool GameOver = false;
        int score = 0;

        //private bool selectLevel;

        //private int selectedLevel;

        //KeyboardState oldKeyboardState;

        //string[] levels;

        public GameScreen(string levelName)
        {
            FileName = levelName;

        }

        public override void Initialize()
        {
            currentGameScreen = this;
            Load(FileName);
            GameOver = false;
            startCoins = entityManager.entities.OfType<Coin>().Count() + entityManager.entities.OfType<Block>().Where(x=> x.holdsCoins).Count();
            //try
            //{
            //    levels = Directory.GetFiles("Levels");
            //}
            //catch (Exception e)
            //{
            //    levels = new string[0];
            //}
            //selectLevel = true;
            //selectedLevel = 0;
            //oldKeyboardState = new KeyboardState(new Keys[] { Keys.Enter });
        }

        private void Load(string name)
        {
            entityManager = new EntityManager();
            XmlSerializer xmlSerializer = new XmlSerializer(entityManager.GetType(), new Type[] { typeof(Character), typeof(Block), typeof(Enemy), typeof(Coin) });
            XmlReader xmlReader = XmlReader.Create(name);
            if (xmlSerializer.CanDeserialize(xmlReader))
                entityManager = (EntityManager)xmlSerializer.Deserialize(xmlReader);
            xmlReader.Close();
            entityManager.onGameOver += entityManager_GameOver;
            foreach (Character character in entityManager.entities.Where(x => x is Character).ToList())
            {
                if (character.playerIndex != 0)
                    character.isActive = false;
            }

            entityManager.entities.OfType<Block>().ToList().ForEach(x => x.getImage());
            entityManager.DivideIntoSections();
        }



        public void entityManager_GameOver(bool succesfull)
        {
            if (succesfull)
            {
                entityManager.onGameOver -= entityManager_GameOver;
                GameOver = true;

                List<Score> scores = new List<Score>();
                XmlSerializer xmlSerializer = new XmlSerializer(scores.GetType(), new Type[] { typeof(Score) });
                if (File.Exists("Scores.xml"))
                {
                    XmlReader xmlReader = XmlReader.Create("Scores.xml");
                    if (xmlSerializer.CanDeserialize(xmlReader))
                        scores = (List<Score>)xmlSerializer.Deserialize(xmlReader);
                    xmlReader.Close();
                }

                score = 0;
                int coins = startCoins - (entityManager.entities.OfType<Coin>().Count()+entityManager.entities.OfType<Block>().Where(x => x.holdsCoins).Count());
                if (startCoins > 0)
                    score = (int)Math.Floor((double)(coins / ((double)startCoins / 3)));

                if (scores.Find(x => x.Name == FileName) != null)
                {
                    if (scores.Find(x => x.Name == FileName).Value < score)
                        scores.Remove(scores.Find(x => x.Name == FileName));
                    else
                    {
                        //ScreenManager.PopScreen(false);
                        return;
                    }
                }
                scores.Add(new Score() { Name = FileName, Value = score });

                XmlWriter xmlWriter = XmlWriter.Create("Scores.xml");
                xmlSerializer.Serialize(xmlWriter, scores);
                xmlWriter.Flush();
                xmlWriter.Close();

                //ScreenManager.PopScreen(false);
            }
            else
            {
                Initialize();
            }
            //Initialize();
        }

        public override void LoadContent(ContentManager content)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                ScreenManager.PopScreen(false);

            //if (selectLevel)
            //{
            //    if (Keyboard.GetState().IsKeyDown(Keys.Down) && oldKeyboardState.IsKeyUp(Keys.Down))
            //        selectedLevel = selectedLevel == levels.Length-1 ? 0 : selectedLevel + 1;
            //    if (Keyboard.GetState().IsKeyDown(Keys.Up) && oldKeyboardState.IsKeyUp(Keys.Up))
            //        selectedLevel = selectedLevel == 0 ? levels.Length -1 : selectedLevel - 1;

            //    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && oldKeyboardState.IsKeyUp(Keys.Enter))
            //    {
            //        selectLevel = false;
            //        Load(levels[selectedLevel]);
            //    }

            //    oldKeyboardState = Keyboard.GetState();
            //}
            //else
            //{
            Game1.Instance.IsMouseVisible = false;
            if (!GameOver)
                entityManager.Update(gameTime);
            else
            {

            }
            //}
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.DrawString(Art.Font, "Game", new Vector2(100, 100), Color.White);
            //if (selectLevel)
            //{
            //    for (int i = 0; i < levels.Length; i++)
            //    {
            //        string level = levels[i];
            //        level =level.Remove(level.Length - 4).Remove(0, 7);
            //        spriteBatch.DrawString(i == selectedLevel ? Art.BoldFont : Art.Font, level, new Vector2(100, 100 + 20 * i), Color.Black);
            //    }
            //}
            //else
            //{
            entityManager.Draw(spriteBatch);
            if (GameOver)
            {
                spriteBatch.DrawString(Art.Font, "GameOver",(Game1.ScreenSize - Art.Font.MeasureString("GameOver")) / 2 + new Vector2(entityManager.cameraPosition.X, -entityManager.cameraPosition.Y), Color.Black);
                spriteBatch.DrawString(Art.Font, "Score: " + score, (Game1.ScreenSize - Art.Font.MeasureString("Score: " + score) + new Vector2(0, 50)) / 2 + new Vector2(entityManager.cameraPosition.X, -entityManager.cameraPosition.Y), Color.Black);
            }
            //}
        }
    }
}
