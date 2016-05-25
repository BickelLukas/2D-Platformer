using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    public delegate void GameOverEventHandler(bool succesfull);

    public class EntityManager
    {
        #region Eigenschaften
        public static EntityManager currentEntityManager { get; private set; }

        public Vector2 cameraPosition {get; set;}
        private List<KeyValuePair<Vector2,double>> cloudPos;

        private List<KeyValuePair<Rectangle,List<Block>>> sections = new List<KeyValuePair<Rectangle,List<Block>>>();

        public List<KeyValuePair<Vector2, double>> CloudPos
        {
            get {
                if (cloudPos == null)
                {
                    cloudPos = new List<KeyValuePair<Vector2, double>>();
                    for (int i = 0; i < rnd.Next(5,10); i++)
                    {
                        cloudPos.Add(new KeyValuePair<Vector2, double>(new Vector2(rnd.Next((int)Game1.ScreenSize.X), rnd.Next((int)Game1.ScreenSize.Y / 2)), (double)rnd.Next(10, 20)/10));
                    }
                }
                return cloudPos; 
            }
        }
        
        #endregion

        #region Felder
        //public List<Block> level = new List<Block>();
        public List<Entity> entities = new List<Entity>();
        //public List<Enemy> enemies = new List<Enemy>();
        static Random rnd = new Random();
        #endregion

        #region Methoden
        public EntityManager()
        {
            currentEntityManager = this;
            cameraPosition = Vector2.Zero;
        }

        public void DivideIntoSections()
        {
            sections.Clear();
            int maxY = entities.OfType<Block>().Max(x => (int)(x.Position.Y+x.Size.Y));
            int maxPosX = entities.OfType<Block>().Max(x => (int)(x.Position.X+x.Size.X));
            int posX = 0;
            while (posX < maxPosX)
            {
                sections.Add(new KeyValuePair<Rectangle, List<Block>>(new Rectangle(posX, (int)Game1.ScreenSize.Y - maxY, 350, maxY), new List<Block>()));
                posX += 350;
            }

            foreach (KeyValuePair<Rectangle, List<Block>> pair in sections)
            {
                pair.Value.AddRange(entities.OfType<Block>().ToList().Where(x => x.Bounds.Intersects(pair.Key)));
            }
        }

        public void addEntity(Entity entity)
        {
            /*if (entity is Block)
                level.Add((Block)entity);*/

            entities.Add(entity);
        }

        public void RemoveAll(Predicate<Entity> match)
        {
            //level.RemoveAll(match);
            entities.RemoveAll(match);
            
        }

        public Vector2 availableMovement(Vector2 position, Vector2 destination, Rectangle bounds)
        {
            Vector2 movementToTry =  destination -position;
            Vector2 furthestAvailableLocationSoFar = position;
            int numberOfStepsToBreakMovementInto = (int)(movementToTry.Length() * 2) + 1;
            Vector2 oneStep = movementToTry / numberOfStepsToBreakMovementInto;

            for (int i = 1; i <= numberOfStepsToBreakMovementInto; i++)
            {
                Vector2 positionToTry = position + oneStep * i;
                Rectangle newBoundary =
                    CreateRectangleAtPosition(positionToTry, bounds.Width, bounds.Height);
                if (hasRoom(newBoundary, null)) { 
                    furthestAvailableLocationSoFar = positionToTry; 
                }
                else {
                    bool isDiagonalMove = movementToTry.X != 0 && movementToTry.Y != 0;
                    if (isDiagonalMove)
                    {
                        int stepsLeft = numberOfStepsToBreakMovementInto - (i - 1);
                        Vector2 remainingHorizontalMovement = oneStep.X * Vector2.UnitX * stepsLeft;
                        Vector2 finalPositionIfMovingHorizontally = furthestAvailableLocationSoFar + remainingHorizontalMovement;
                        furthestAvailableLocationSoFar = availableMovement(furthestAvailableLocationSoFar, finalPositionIfMovingHorizontally, bounds);

                        Vector2 remainingVerticalMovement = oneStep.Y * Vector2.UnitY * stepsLeft;
                        Vector2 finalPositionIfMovingVertically = furthestAvailableLocationSoFar + remainingVerticalMovement;
                        furthestAvailableLocationSoFar = availableMovement(furthestAvailableLocationSoFar, finalPositionIfMovingVertically, bounds);
                    }
                    break; 
                }
            }
            return furthestAvailableLocationSoFar;
        }

        private Rectangle CreateRectangleAtPosition(Vector2 positionToTry, int width, int height)
        {
            return new Rectangle((int)positionToTry.X, (int)(Game1.ScreenSize.Y - positionToTry.Y - height), width, height);
        }

        public bool hasRoom(Rectangle rect, Character character)
        {
            bool returnValue = true;
            List<Block> list = GetNearbyLevel(new Vector2(rect.X, Game1.ScreenSize.Y - rect.Center.Y), 150);

            //if (character != null)
            //    entities.OfType<Block>().ToList().ForEach(x => x.color = Color.White);
            foreach (Block block in list)
            {
                //if (character != null)
                //    block.color = Color.Blue;

                if (block.Bounds.Intersects(rect))
                {
                    //if (character != null)
                    //    block.color = Color.Yellow;

                    if (block.isInteractive && character != null)
                    {
                        //(entities.ElementAt(level.IndexOf(block)) as Block).Interact(character);
                        if (!block.isUsed)
                            block.Interact(character);
                    }
                    if (block.isExit && character != null)
                    {
                        if (onGameOver != null)
                            onGameOver(true);
                        //return false;
                    }

                    if (!block.walkThourgh)
                    {
                        if (block.isDeadly && character != null)
                        {
                            if (!character.Hurt)
                            {
                                character.Hit(2);
                                character.Velocity.Y = block.Position.Y > character.Position.Y ? -1 : 10;
                                character.color = Color.Red;
                            }
                            //character.IsExpired = true;
                        }
                        returnValue = false;
                        //return false;
                    }
                    if (block.canFloat&&character != null)
                    {
                        Rectangle rectHalf = new Rectangle();
                        rectHalf = block.Bounds;
                        rectHalf.Y += block.Bounds.Height / 2;
                        if (character.Bounds.Intersects(rectHalf))
                            character.inWater = true;
                        if (block.blockAbove())
                            character.floating = true;
                    }

                }
                
            }
            return returnValue;
        }

        public List<Block> GetNearbyLevel(Vector2 position, float radius)
        {
            if (sections.Count > 0)
            {
                List<Block> blocks = new List<Block>();
                foreach (KeyValuePair<Rectangle,List<Block>> pair in sections.Where(x => x.Key.Intersects(new Rectangle((int)position.X, (int)(Game1.ScreenSize.Y - position.Y), 200, 100))))
                {
                    blocks.AddRange(pair.Value.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius));
                }
                return blocks;
            }
            else
                return entities.OfType<Block>().Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius).ToList();
        }

        public bool BlockHasRoom(Rectangle rect, Block block)
        {
            foreach (Block entity in GetNearbyLevel(new Vector2(rect.X,Game1.ScreenSize.Y - rect.Y),100))
            {
                if (entity.Bounds.Intersects(rect) && block.walkThourgh == entity.walkThourgh)
                    return false;
            }
            return true;
        }

        public void Update(GameTime gameTime)
        {
            List<Character> characters = new List<Character>(entities.OfType<Character>());

            foreach (Character character in characters.Where(x => x.isActive).ToList<Character>())
            {
                foreach (Enemy enemy in new List<Enemy>(entities.OfType<Enemy>().ToList<Enemy>()))
                {
                    if (enemy.isActive && character.Bounds.Intersects(enemy.Bounds))
                    {
                        if (enemy.Position.Y + enemy.Size.Y >= character.Position.Y+20)
                        {
                            if (!character.Hurt) {
                                character.Hit(enemy.Strength);
                                character.Velocity.X = enemy.Position.X >= character.Position.X ? -10 : 10;
                            }
                            //character.Velocity.Y = 3;
                            //enemy.IsExpired = true;
                        }
                        //character.IsExpired = true;
                        else
                        {
                            character.Velocity.Y = 10;
                            enemy.IsExpired = true;
                        }
                    }
                }

                foreach (Coin coin in new List<Coin>(entities.OfType<Coin>().Where(x => !x.collected && x.Collectable)))
                {
                    if (coin.Bounds.Intersects(character.Bounds))
                    {
                        coin.collected = true;
                        character.Score++;
                    }
                }
            }

            foreach (Character character in characters.Where(x => !x.isActive))
            {
                if (Keyboard.GetState().IsKeyDown((Keys)character.keyJump) || GamePad.GetState((PlayerIndex) character.playerIndex).IsButtonDown(Buttons.A))
                    character.isActive = true;
            }


            foreach (Entity entity in entities.Where(x => (x as Character)!= null?(x as Character).isActive:true).ToList())
            {
                entity.Update(gameTime);
            }

            entities = entities.Where(x => !x.IsExpired).ToList();

            if (characters.Where(x => x.isActive).ToList().Count <= 0)
            {
                if (onGameOver != null)
                    onGameOver(false);
            }

            cameraPosition = Vector2.Zero;
            if (characters.Where(x => x.isActive).Count() > 0){
                float maxX = characters.Where(x => x.isActive).Max(x => x.Position.X);
                float maxY = characters.Find(x => x.Position.X == maxX).Position.Y;
                cameraPosition = new Vector2((maxX - Game1.ScreenSize.X / 2) > cameraPosition.X?(maxX- Game1.ScreenSize.X / 2):cameraPosition.X,(maxY - Game1.ScreenSize.Y / 2) > cameraPosition.Y?(maxY- Game1.ScreenSize.Y / 2):cameraPosition.Y);
                //foreach (Entity entity in characters.Where(x => x.isActive))
                //    if (entity.Position.X > Game1.ScreenSize.X / 2 || entity.Position.Y > Game1.ScreenSize.Y / 2)
                //        cameraPosition = (entity.Position.X - Game1.ScreenSize.X / 2) > cameraPosition.X ? new Vector2((entity.Position.X - Game1.ScreenSize.X / 2),(entity.Position.Y - Game1.ScreenSize.Y / 2)) : cameraPosition;
            }
            for (int i = 0; i < CloudPos.Count; i++)
            {
                KeyValuePair<Vector2, double> pair = CloudPos.ElementAt(i);
                if (pair.Key.X + (Art.Cloud.Width * pair.Value) < (cameraPosition.X / 2))
                {
                    CloudPos.Remove(pair);
                    CloudPos.Add(new KeyValuePair<Vector2, double>(pair.Key + new Vector2((float)(Game1.ScreenSize.X + Art.Cloud.Width * pair.Value), 0), pair.Value));
                }
                else if (pair.Key.X > (Game1.ScreenSize.X + cameraPosition.X / 2))
                {
                    CloudPos.Remove(pair);
                    CloudPos.Add(new KeyValuePair<Vector2, double>(pair.Key - new Vector2((float)(Game1.ScreenSize.X + Art.Cloud.Width * pair.Value), 0), pair.Value));
                }
            }
        }

        private static bool IsColliding(Entity a, Entity b)
        {
            return !a.IsExpired && !b.IsExpired && new Rectangle((int)a.Position.X, (int)(Game1.ScreenSize.Y - a.Position.Y), (int)a.Size.X, (int)a.Size.Y).Intersects(new Rectangle((int)b.Position.X, (int)(Game1.ScreenSize.Y -b.Position.Y ), (int)b.Size.X, (int)b.Size.Y));
        }

        public void Draw(SpriteBatch spriteBatch,float scale = 1f)
        {
            Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition.X, cameraPosition.Y, 0.0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, cameraTransform * Game1.Instance._camera.GetViewTransformationMatrix());

            foreach(KeyValuePair<Vector2, double> pair in CloudPos)
                spriteBatch.Draw(Art.Cloud, new Rectangle((int)(pair.Key.X + cameraPosition.X / 2), (int)(pair.Key.Y), (int)(Art.Cloud.Width * pair.Value), (int)(Art.Cloud.Height * pair.Value)), Color.White);
            
            //foreach (Entity entity in level.Where(x => /*!(x is Character) && */(x.Position.X + x.Size.X >= cameraPosition) && (x.Position.X - x.Size.X <= Game1.ScreenSize.X + cameraPosition)))
            //    entity.Draw(spriteBatch,scale);

            List<Entity> drawnEntities = entities.Where(x => x.Bounds.Right >= cameraPosition.X && x.Bounds.Left - x.Bounds.Width <= Game1.ScreenSize.X + cameraPosition.X || (x is Character)).ToList();

            foreach (Block entity in drawnEntities.OfType<Block>())
                entity.Draw(spriteBatch, scale);

            foreach (Enemy entity in drawnEntities.OfType<Enemy>())
                entity.Draw(spriteBatch, scale);

            foreach (Character entity in drawnEntities.OfType<Character>().Where(x => (x as Character).isActive))
                entity.Draw(spriteBatch, scale);

            foreach (Coin entity in drawnEntities.OfType<Coin>())
                entity.Draw(spriteBatch, scale);

            //foreach (KeyValuePair<Rectangle, List<Block>> pair in sections)
            //    spriteBatch.Draw(Art.GetContent<Texture2D>("art/rect"), pair.Key, Color.White);
        }
        #endregion

        #region Events
        public event GameOverEventHandler onGameOver;
        #endregion
    }
}
