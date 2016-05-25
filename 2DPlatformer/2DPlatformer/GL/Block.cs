using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    public class Block : Entity
    {
        public enum Materials { Grass = 0, Sand = 1, Spikes = 2, Water = 3, Stone = 4, Exit = 5, SignLeft = 6, SignRight = 7, Block = 8, Spring = 9, Box = 10, BoxItem = 11, BoxCoin = 12, Bush = 13, Rock = 14, Plant = 15}

        public int Material;

        public bool isDeadly;
        public bool walkThourgh;
        public bool isExit;
        public bool isInteractive;
        public bool canFloat;
        public bool holdsCoins;

        public bool isUsed;
        private bool hasInteracted;

        private bool upsideDown;

        float elapsed;
        //public string imgPath;

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(base.Bounds.X,base.Bounds.Y - (upsideDown?35:0),base.Bounds.Width,base.Bounds.Height);
            }
        }

        public override Vector2 Size
        {
            get
            {
                /*if (image == null)
                    getImage();
                    //image = Art.GetContent<Texture2D>(imgPath);
                return base.Size;// */
                return new Vector2(70, (Materials)Material == Materials.Spikes || ((Materials)Material == Materials.Spring && !isUsed) ? 35 : 70);
            }
        }

        public void getImage()
        {
            string pathCenter = "";
            string pathMid = "";
            string pathCliffLeft = "";
            string pathCliffRight = "";
            switch ((Materials)Material)
            {
                case Materials.Grass:
                    pathCenter = "art/level/grassCenter";
                    pathMid = "art/level/grassMid";
                    pathCliffLeft = "art/level/grassCliffLeft";
                    pathCliffRight = "art/level/grassCliffRight";
                    break;
                case Materials.Sand:
                    pathCenter = "art/level/sandCenter";
                    pathMid = "art/level/sandMid";
                    pathCliffLeft = "art/level/sandCliffLeft";
                    pathCliffRight = "art/level/sandCliffRight";
                    break;
                case Materials.Stone:
                    pathCenter = "art/level/stoneCenter";
                    pathMid = "art/level/stoneMid";
                    pathCliffLeft = "art/level/stoneCliffLeft";
                    pathCliffRight = "art/level/stoneCliffRight";
                    break;
                case Materials.Water:
                    pathMid = pathCliffLeft = pathCliffRight = "art/level/liquidWaterTop_mid";
                    pathCenter = "art/level/liquidWater";
                    break;
                case Materials.Spikes:
                    image = Art.GetContent<Texture2D>("art/level/spikes" + ((upsideDown = blockAbove()) ? "_alt" : ""));
                    return;
                case Materials.Exit:
                    image = Art.GetContent<Texture2D>("art/level/signExit");
                    return;
                case Materials.SignLeft:
                    image = Art.GetContent<Texture2D>("art/level/signLeft");
                    return;
                case Materials.SignRight:
                    image = Art.GetContent<Texture2D>("art/level/signRight");
                    return;
                case Materials.Block:
                    image = Art.GetContent<Texture2D>("art/level/block");
                    return;
                case Materials.Spring:
                    image = Art.GetContent<Texture2D>("art/level/springboard" + (isUsed ? "Up" : "Down"));
                    return;
                case Materials.Box:
                    image = Art.GetContent<Texture2D>("art/level/boxEmpty");
                    return;
                case Materials.BoxItem:
                    image = Art.GetContent<Texture2D>("art/level/boxItemAlt" + (isUsed ? "_disabled" : ""));
                    return;
                case Materials.BoxCoin:
                    image = Art.GetContent<Texture2D>("art/level/boxCoinAlt" + (isUsed ? "_disabled" : ""));
                    return;
                case Materials.Bush:
                    image = Art.GetContent<Texture2D>("art/level/bush");
                    return;
                case Materials.Rock:
                    image = Art.GetContent<Texture2D>("art/level/rock");
                    return;
                case Materials.Plant:
                    image = Art.GetContent<Texture2D>("art/level/plant");
                    return;
                default:
                    image = Art.GetContent<Texture2D>("art/level/block");
                    return;
            }

            if (!blockAbove() && !blockBelow() && !blockLeft() && blockRight())
                image = Art.GetContent<Texture2D>(pathCliffLeft);
            else if (!blockAbove() && !blockBelow() && !blockRight() && blockLeft())
                image = Art.GetContent<Texture2D>(pathCliffRight);
            else if (blockAbove())
                image = Art.GetContent<Texture2D>(pathCenter);
            else
                image = Art.GetContent<Texture2D>(pathMid);
        }

        public Block()
        {

        }

        public bool blockAbove()
        {
            Rectangle onePixelHigher = new Rectangle((int)Position.X, (int)(Game1.ScreenSize.Y - Position.Y - Size.Y -70), (int)Size.X, (int)Size.Y);
            return !EntityManager.currentEntityManager.BlockHasRoom(onePixelHigher, this);
        }

        public bool blockBelow()
        {
            Rectangle onePixelHigher = new Rectangle((int)Position.X, (int)(Game1.ScreenSize.Y - Position.Y), (int)Size.X, (int)Size.Y);
            return (!EntityManager.currentEntityManager.BlockHasRoom(onePixelHigher, this) || Position.Y == 0);
        }

        public bool blockLeft()
        {
            Rectangle onePixelHigher = new Rectangle((int)(Position.X - Size.X), (int)(Game1.ScreenSize.Y - Position.Y - Size.Y), (int)Size.X, (int)Size.Y);
            return !EntityManager.currentEntityManager.BlockHasRoom(onePixelHigher, this) || Position.X == 0;
        }

        public bool blockRight()
        {
            Rectangle onePixelHigher = new Rectangle((int)(Position.X + Size.X), (int)(Game1.ScreenSize.Y - Position.Y - Size.Y), (int)Size.X, (int)Size.Y);
            return !EntityManager.currentEntityManager.BlockHasRoom(onePixelHigher, this);
        }

        /*public override bool Equals(object obj)
        {
            Block block = obj as Block;
            if (block != null)
                return Position.Equals(Position) && Size.Equals(Size);
            return false;
            //return base.Equals(obj);
        }*/

        public Block(Vector2 position, Materials material)//string imagePath)
        {
            Position = position;
            Material = (int)material;
            if (material == Materials.Spikes)
                isDeadly = true;
            if (material == Materials.SignLeft || material == Materials.SignRight || material == Materials.Water || material == Materials.Exit || material == Materials.Spring || material == Materials.Bush || material == Materials.Rock || material == Materials.Plant)
                walkThourgh = true;
            if (material == Materials.Spring || material == Materials.BoxCoin || material == Materials.BoxItem)
                isInteractive = true;
            if (material == Materials.BoxCoin)
                holdsCoins = true;
            if (material == Materials.Exit)
                isExit = true;
            if (material == Materials.Water)
                canFloat = true;


            //imgPath = imagePath;
            //Image = Game1.Instance.Content.Load<Texture2D>(imgPath);
        }

        public override void Draw(SpriteBatch spriteBatch, float scale = 1f)
        {
            if (image == null)
                getImage();
            //image = Art.GetContent<Texture2D>(imgPath);
            if (upsideDown)
                Position.Y += 35;
            base.Draw(spriteBatch, scale);

            if (upsideDown)
                Position.Y -= 35;
        }

        public override void Update(GameTime gameTime)
        {
            if (isInteractive && isUsed)
            {
                if ((Materials)Material == Materials.Spring)
                {
                    elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (elapsed >= 1.5f)
                    {
                        isUsed = false;
                        elapsed = 0;
                        getImage();
                    }
                }
                else if ((Materials)Material == Materials.BoxCoin && !hasInteracted)
                {
                    EntityManager.currentEntityManager.addEntity(new Coin(this.Position + new Vector2(0, 70)) /*{ collected = true }*/);
                    hasInteracted = true;
                }
            }
        }

        public void Interact(Character character)
        {

            if ((Materials)Material == Materials.Spring && !isUsed)
            {
                if (character.Position.Y > this.Position.Y)
                {
                    character.Velocity.Y = 17;
                    character.Flip = true;
                    this.isUsed = true;
                }
            }
            else if ((Materials)Material == Materials.BoxCoin && !isUsed && character.Position.Y < Position.Y && character.Position.X + character.Size.X > this.Position.X+8 && character.Position.X < Position.X + Size.X-8)
            {
                //character.Score++;
                this.isUsed = true;
                this.holdsCoins = false;
            }

            getImage();
        }

        public override Entity Clone(Vector2 pos)
        {
            Block clone = new Block(pos,(Materials)Material);
            return clone;
        }
    }
}
