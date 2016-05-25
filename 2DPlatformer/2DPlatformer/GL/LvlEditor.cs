using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace _2DPlatformer.GL
{
    public class Panel
    {
        private string imgName;
        private int index;
        private int selectedItem;
        public Texture2D Image { get { return Art.GetContent<Texture2D>("art/UI/Editor/btn" + imgName + (selected?"_Click":"")); } }
        public Rectangle Rect { get { return new Rectangle(Image.Width * index + (int)EntityManager.currentEntityManager.cameraPosition.X, -(int)EntityManager.currentEntityManager.cameraPosition.Y, Image.Width, Image.Height); } }
        public Entity getSelected { get { return Items.ElementAt(selectedItem); } }
        public List<Entity> Items { get; set; }
        public bool selected { get; set; }

        public Panel(string name,int index, List<Entity> items)
        {
            imgName = name;
            this.index = index;
            Items = items;
            selected = (index == 0);   
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(Image, Rect, Color.White);

            if (selected)
            {
                int thisIndex = 0;
                float x = 10;
                foreach (Entity thisItem in Items)
                {
                    thisItem.Position = new Vector2(x, Game1.ScreenSize.Y - 65 - 70) + EntityManager.currentEntityManager.cameraPosition;
                    thisItem.Draw(spriteBatch);
                    x += thisItem.Bounds.Width + 20;
                    if (selectedItem == thisIndex)
                        spriteBatch.Draw(Art.GetContent<Texture2D>("art/UI/Editor/selected"),new Rectangle((int)thisItem.Position.X,(int)Game1.ScreenSize.Y-(int)thisItem.Position.Y-70,70,70),Color.White);
                    thisIndex++;
                }
            }
        }

        public Entity select(Rectangle pos)
        {
            int thisIndex = 0;
            foreach (Entity thisItem in Items)
            {
                if (pos.Intersects(thisItem.Bounds))
                    selectedItem = thisIndex;
                thisIndex++;
            }
            return Items.ElementAt(selectedItem);
        }

    }

    class LvlEditor : Screen
    {
        float scale = 1f;
        const float gridElemSize = 70;

        int CharacterSelected = 0;

        MouseState mouseState;
        MouseState oldMouseState;

        KeyboardState keyboardState;
        KeyboardState oldkeyboardState;

        Rectangle pointerPos = new Rectangle(0,0,1,1);
        Rectangle menuArea { get { return new Rectangle((int)entityManager.cameraPosition.X, -(int)entityManager.cameraPosition.Y, (int)Game1.ScreenSize.X, 170); } }

        Texture2D panel = Art.GetContent<Texture2D>("art/UI/Editor/panel");

        Rectangle btnSave { get { return new Rectangle((int)Game1.ScreenSize.X - btnSaveTexture.Width + (int)entityManager.cameraPosition.X, -(int)entityManager.cameraPosition.Y, btnSaveTexture.Width, btnSaveTexture.Height); } }
        Rectangle btnImport { get { return new Rectangle((int)Game1.ScreenSize.X - btnSaveTexture.Width - btnImportTexture.Width + (int)entityManager.cameraPosition.X, -(int)entityManager.cameraPosition.Y, btnImportTexture.Width, btnImportTexture.Height); } }
        Rectangle btnNew { get { return new Rectangle((int)Game1.ScreenSize.X - btnSaveTexture.Width - btnImportTexture.Width - btnNewTexture.Width + (int)entityManager.cameraPosition.X, -(int)entityManager.cameraPosition.Y, btnNewTexture.Width, btnNewTexture.Height); } }

        Texture2D btnSaveTexture;
        Texture2D btnImportTexture;
        Texture2D btnNewTexture;

        //List<Block> elems = new List<Block>();
        EntityManager entityManager;

        Entity currentElement;

        //string currentTexture;
        //List<string> textures;
        Block.Materials currentMaterial;
        List<Block.Materials> materials;

        string currentEnemy;
        List<string> enemies;

        int currentElements = 0;

        XmlSerializer xmlSerializer;
        XmlWriter xmlWriter;
        XmlReader xmlReader;

        private string FileName;
        bool saving = false;

        bool importing = false;
        private int selectedLevel;
        string[] levels;

        List<Panel> panels = new List<Panel>() {
            new Panel("Blocks",0, new List<Entity>() {
                new Block(new Vector2(0,0),Block.Materials.Grass),
                new Block(new Vector2(0,0),Block.Materials.Sand),
                new Block(new Vector2(0,0),Block.Materials.Stone),
                new Block(new Vector2(0,0),Block.Materials.Water),
                new Block(new Vector2(0,0),Block.Materials.Block)
            }), 
            new Panel("Decor",1, new List<Entity>() {
                new Block(new Vector2(0,0),Block.Materials.SignLeft),
                new Block(new Vector2(0,0),Block.Materials.SignRight),
                new Block(new Vector2(0,0),Block.Materials.Bush),
                new Block(new Vector2(0,0),Block.Materials.Plant),
                new Block(new Vector2(0,0),Block.Materials.Rock)
            }), 
            new Panel("Dynamic",2, new List<Entity>() {
                new Block(new Vector2(0,0),Block.Materials.Spring),
                new Block(new Vector2(0,0),Block.Materials.Box),
                new Block(new Vector2(0,0),Block.Materials.BoxCoin),
                new Block(new Vector2(0,0),Block.Materials.BoxItem),
                new Block(new Vector2(0,0),Block.Materials.Exit),
                new Coin()
            }), 
            new Panel("Enemys",3, new List<Entity>() {
                new Enemy("fish",new Vector2(0,0)),
                new Enemy("fly",new Vector2(0,0)),
                new Enemy("slime",new Vector2(0,0)),
                new Enemy("spinner",new Vector2(0,0))
            })};
        int currentPanel = 0;

        public override void Initialize()
        {
        //    //textures = new List<string>() { "art/level/grassMid", "art/level/grassCenter", "art/level/block", "art/level/dirtMid", "art/level/dirtCenter", "art/level/sandMid", "art/level/sandCenter", "art/level/spikes" };
        //    materials = new List<Block.Materials>() { Block.Materials.Grass, Block.Materials.Sand, Block.Materials.Stone, Block.Materials.Spikes, Block.Materials.Water, Block.Materials.SignLeft, Block.Materials.SignRight, Block.Materials.Exit, Block.Materials.Block, Block.Materials.Spring, Block.Materials.Box, Block.Materials.BoxCoin, Block.Materials.BoxItem, Block.Materials.Bush, Block.Materials.Plant, Block.Materials.Rock};
        //    enemies = new List<string>() { "fish", "fly", "slime","spinner" };

            //currentTexture = textures.ElementAt(0);
            //SetCurrentBlock(0);
            currentElement = panels.Where(x => x.selected).First().getSelected;

            btnSaveTexture = Art.Save;
            btnImportTexture = Art.Import;
            btnNewTexture = Art.New;

            entityManager = new EntityManager();

            xmlSerializer = new XmlSerializer(entityManager.GetType(), new Type[] { typeof(Character), typeof(Block), typeof(Enemy), typeof(Entity), typeof(Coin) });

            entityManager.addEntity(new Character() { playerIndex = 0, Position = new Vector2(0, gridElemSize) });
            entityManager.addEntity(new Character() { playerIndex = 1, Position = new Vector2(70, gridElemSize), keyLeft = (int)Keys.A, keyRight = (int)Keys.D, keyJump = (int)Keys.W, keyDown = (int)Keys.S });
            //entityManager.addEntity(Enemy.CreateFly(new Vector2(100,500)));
            /*Character Player2 = new Character() { Position = new Vector2(70, gridElemSize) };
            Player2.setupInput((int)Keys.A, (int)Keys.D, (int)Keys.W);
            entityManager.addEntity(Player2);*/

            FileName = "";
        }

        //static private Texture2D CreateRectangle(int width, int height, Color colori)
        //{
        //    Texture2D rectangleTexture = new Texture2D(Game1.Instance.GraphicsDevice, width, height);
        //    Color[] color = new Color[width * height];
        //    for (int i = 0; i < color.Length; i++)
        //    {
        //        color[i] = colori;
        //    }
        //    rectangleTexture.SetData(color);
        //    return rectangleTexture;
        //}

        public override void LoadContent(ContentManager content)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (!saving && !importing)
            {

                if (keyboardState.IsKeyDown(Keys.Escape) && oldkeyboardState.IsKeyUp(Keys.Escape))
                    ScreenManager.PopScreen(false);

                if (keyboardState.IsKeyDown(Keys.D))
                    entityManager.cameraPosition += new Vector2(10f,0);

                if (keyboardState.IsKeyDown(Keys.A))
                    if (entityManager.cameraPosition.X >= 0)
                        entityManager.cameraPosition -= new Vector2(10f,0);

                if (keyboardState.IsKeyDown(Keys.W))
                    entityManager.cameraPosition += new Vector2(0,10f);

                if (keyboardState.IsKeyDown(Keys.S))
                    if (entityManager.cameraPosition.Y >= 0)
                        entityManager.cameraPosition -= new Vector2(0,10f);

            }
            else if (importing)
            {
                if (keyboardState.IsKeyDown(Keys.Down) && oldkeyboardState.IsKeyUp(Keys.Down))
                    selectedLevel = selectedLevel == levels.Length - 1 ? 0 : selectedLevel + 1;
                if (keyboardState.IsKeyDown(Keys.Up) && oldkeyboardState.IsKeyUp(Keys.Up))
                    selectedLevel = selectedLevel == 0 ? levels.Length - 1 : selectedLevel - 1;

                if (keyboardState.IsKeyDown(Keys.Escape) && oldkeyboardState.IsKeyUp(Keys.Escape))
                    importing = false;

                if (keyboardState.IsKeyDown(Keys.Enter) && oldkeyboardState.IsKeyUp(Keys.Enter))
                {
                    importing = false;
                    FileName = levels[selectedLevel].Remove(levels[selectedLevel].Length - 4).Remove(0, 7);
                    Import(levels[selectedLevel]);
                }
            }
            else
            {
                foreach (Keys key in keyboardState.GetPressedKeys())
                {
                    if (key == Keys.Enter && oldkeyboardState.IsKeyUp(Keys.Enter))
                    {
                        Save();
                        saving = false;
                    }
                    else if (key == Keys.Back && oldkeyboardState.IsKeyUp(key) && FileName.Length > 0)
                        FileName = FileName.Substring(0, FileName.Length - 1);
                    else if (key == Keys.Escape)
                        saving = false;
                    if (oldkeyboardState.IsKeyUp(key))
                    {
                        if (key == Keys.Space)
                            FileName += " ";
                        else if (Convert.ToInt32((char)key) >= Convert.ToInt32('A') && Convert.ToInt32((char)key) <= Convert.ToInt32('z'))
                            FileName += key.ToString();

                        try
                        {
                            if (Convert.ToInt32(key.ToString().Substring(1, 1)) >= 0 && Convert.ToInt32(key.ToString().Substring(1, 1)) <= 9)
                                FileName += key.ToString().Substring(1, 1);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            oldkeyboardState = keyboardState;

            if (!saving)
            {
                mouseState = Mouse.GetState();

                Vector2 mousePos = Game1.Instance._resolutionIndependence.ScaleMouseToScreenCoordinates(new Vector2(mouseState.X, mouseState.Y));
                pointerPos.X = (int)mousePos.X + (int)entityManager.cameraPosition.X;
                pointerPos.Y = (int)mousePos.Y - (int)entityManager.cameraPosition.Y;

                if (pointerPos.Intersects(btnSave))
                    btnSaveTexture = Art.SaveMO;
                else
                    btnSaveTexture = Art.Save;

                if (pointerPos.Intersects(btnImport))
                    btnImportTexture = Art.ImportMO;
                else
                    btnImportTexture = Art.Import;

                if (pointerPos.Intersects(btnNew))
                    btnNewTexture = Art.NewMO;
                else
                    btnNewTexture = Art.New;

                List<Rectangle> characterRects = new List<Rectangle>();
                List<Entity> characters = entityManager.entities.Where(x => x is Character).ToList();
                foreach (Entity character in characters)
                {
                    characterRects.Add(new Rectangle((int)(character.Position.X * scale), (int)(Game1.ScreenSize.Y - (character.Position.Y + character.Size.Y) * scale), (int)(character.Size.X * scale), (int)(character.Size.Y * scale)));
                }

                if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                {
                    CharacterSelected = 0;
                    for (int i = 0; i < characterRects.Count; i++)
                    {
                        if (characterRects.ElementAt(i).Intersects(pointerPos))
                            CharacterSelected = i + 1;
                    }

                    foreach (Panel thisPanel in panels)
                    {
                        if (pointerPos.Intersects(thisPanel.Rect))
                        {
                            panels.ForEach(x => x.selected = false);
                            thisPanel.selected = true;
                        }

                        if (thisPanel.selected)
                        {
                            currentElement = thisPanel.select(pointerPos);
                        }

                    }

                    if (pointerPos.Intersects(btnImport))
                    {
                        levels = Directory.GetFiles("Levels");
                        importing = true;
                    }
                    //Import();

                    if (pointerPos.Intersects(btnSave))
                        saving = true;
                    //Save();

                    if (pointerPos.Intersects(btnNew))
                        Initialize();
                }

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    bool intersects = false;
                    characterRects.ForEach(x => intersects = x.Intersects(pointerPos) ? true : intersects);

                    if (CharacterSelected != 0)
                        characters.ElementAt(CharacterSelected - 1).Position = (toGrid(new Vector2(pointerPos.X, Game1.ScreenSize.Y - pointerPos.Y)) / scale);
                    else if (!pointerPos.Intersects(menuArea) && !intersects)
                    {


                        Vector2 Position = toGrid(new Vector2(pointerPos.X, Game1.ScreenSize.Y - pointerPos.Y)) / scale;
                        //if (currentElements == 2)
                        //    Position = toGrid(new Vector2(pointerPos.X, Game1.ScreenSize.Y - pointerPos.Y),35) / scale;

                        entityManager.RemoveAll(x => (((x as Block) != null && (currentElement as Block) != null) || ((x as Coin) != null && (currentElement as Coin) != null) || ((x as Enemy) != null && (currentElement as Enemy) != null)) && x.Position == Position);
                        entityManager.addEntity(currentElement.Clone(Position));
                        //if (currentElements == 1)
                        //    entityManager.addEntity(new Enemy(currentEnemy, Position));//currentEnemy == "fish" ? Enemy.CreateFish(Position) : (currentEnemy == "fly" ? Enemy.CreateFly(Position) : (currentEnemy == "slime" ?Enemy.CreateSlime(Position):Enemy.CreateSpinner(Position)))); //new Block(toGrid(new Vector2(pointerPos.X, Game1.ScreenSize.Y - pointerPos.Y)) / scale, currentMaterial));//currentTexture
                        //else if (currentElements == 0)
                        //    entityManager.addEntity(new Block(Position, currentMaterial));
                        //else if (currentElements == 2)
                        //    entityManager.addEntity(new Coin(Position));

                        entityManager.GetNearbyLevel(Position,200).ForEach(x => x.getImage());// .entities.OfType<Block>().ToList().ForEach(x => x.getImage());
                    }
                }
                else
                    CharacterSelected = 0;

                if (mouseState.RightButton == ButtonState.Pressed)
                {
                    bool intersects = false;
                    characterRects.ForEach(x => intersects = x.Intersects(pointerPos) ? true : intersects);

                    if (!pointerPos.Intersects(menuArea) && !intersects)
                        entityManager.RemoveAll(x => x.Position == toGrid(new Vector2(pointerPos.X, Game1.ScreenSize.Y - pointerPos.Y)) / scale);

                    entityManager.GetNearbyLevel(toGrid(new Vector2(pointerPos.X, Game1.ScreenSize.Y - pointerPos.Y)) / scale, 200).ForEach(x => x.getImage());
                }

                oldMouseState = mouseState;
            }


            //currentElement.Position.X = Game1.ScreenSize.X - 100 + entityManager.cameraPosition.X;
            //currentElement.Position.Y = Game1.ScreenSize.Y - 105 + entityManager.cameraPosition.Y;
        }

        private Vector2 toGrid(Vector2 position, float size = gridElemSize)
        {
            return new Vector2((float)Math.Floor((position.X) / size) * size, (float)Math.Floor((position.Y) / size) * size);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!saving && !importing)
            {
                entityManager.Draw(spriteBatch, scale);

                spriteBatch.Draw(panel, new Rectangle(0 + (int)entityManager.cameraPosition.X, 49 - (int)entityManager.cameraPosition.Y, panel.Width, panel.Height), Color.White);
                panels.ForEach(x => x.Draw(spriteBatch));
                

                //spriteBatch.Draw(menuTexture, menuArea, Color.White);

                //currentElement.Draw(spriteBatch);

                spriteBatch.Draw(btnSaveTexture, btnSave, Color.White);
                spriteBatch.Draw(btnImportTexture, btnImport, Color.White);
                spriteBatch.Draw(btnNewTexture, btnNew, Color.White);


                spriteBatch.Draw(Art.Cursor, new Vector2(pointerPos.X, pointerPos.Y), Color.White);
            }
            else if (importing)
            {
                for (int i = 0; i < levels.Length; i++)
                {
                    string level = levels[i];
                    level = level.Remove(level.Length - 4).Remove(0, 7);
                    spriteBatch.DrawString(i == selectedLevel ? Art.BoldFont : Art.Font, level, new Vector2(100, 100 + 20 * i), Color.Black);
                }
            }
            else
            {
                spriteBatch.DrawString(Art.Font, "NAME: " + FileName, new Vector2(200, 200), Color.Black);
            }
        }

        private void Import(string name)
        {
            entityManager = new EntityManager();
            xmlReader = XmlReader.Create(name);
            if (xmlSerializer.CanDeserialize(xmlReader))
                entityManager = (EntityManager)xmlSerializer.Deserialize(xmlReader);
            xmlReader.Close();
            foreach (Block block in new List<Block>(entityManager.entities.OfType<Block>()))
            {
                Vector2 pos = block.Position;
                int material = block.Material;
                entityManager.entities.Remove(block);
                entityManager.addEntity(new Block(pos, (Block.Materials) material));
            }
            Vector2 pos1 = entityManager.entities.OfType<Character>().First().Position;
            Vector2 pos2 = entityManager.entities.OfType<Character>().Where(x => x.playerIndex == 1).First().Position;
            entityManager.entities.RemoveAll(x => x is Character);
            entityManager.addEntity(new Character() { playerIndex = 0, Position = pos1 });
            entityManager.addEntity(new Character() { playerIndex = 1, Position = pos2, keyLeft = (int)Keys.A, keyRight = (int)Keys.D, keyJump = (int)Keys.W, keyDown = (int)Keys.S });
        }

        private void Save()
        {
            if (Directory.Exists("Levels") == false)
            {
                Directory.CreateDirectory("Levels");
            }

            xmlWriter = XmlWriter.Create("Levels/" + FileName + ".xml");
            xmlSerializer.Serialize(xmlWriter, entityManager);
            xmlWriter.Flush();
            xmlWriter.Close();

           
            //entityManager = new EntityManager();
        }
    }
}
