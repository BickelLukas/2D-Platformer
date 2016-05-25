using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    class CharacterAnimation
    {
        public enum Animations { stand = 1, duck = 2, jump = 3, walk = 4, swim = 5 }

        private Animations _currentAnimation;

        public Animations CurrentAnimation
        {
            get { return _currentAnimation; }
            set { _currentAnimation = value; currentFrame = 1; }
        }

        public bool right { set { _texture = Art.GetContent<Texture2D>(value ? "art/character/p" + playerIndex + "_spritesheet" : "art/character/p" + playerIndex + "_spritesheet_left"); } }
        

        private Texture2D _texture;
        string spriteSheet;

        int playerIndex;
        //public Texture2D Texture { get { return _texture; } set { _texture = value; currentFrame = 0; } }

        public float frameTime { get; set; }
        //public int frames { get; set; }
        //public int width { get; set; }
        float elapsed;
        int currentFrame = 1;
        Rectangle sourceRect;
        bool skipFrame;

        public CharacterAnimation(int playerIndex)//Texture2D texture, int frameTime, int frames, int width)
        {
            this.playerIndex = playerIndex;
            //this.Texture = texture;
            _texture = Art.GetContent<Texture2D>("art/character/p" + playerIndex + "_spritesheet");

            this.frameTime = 0.05f;//frameTime;
            //this.frames = frames;
            //this.width = width;

            Stream stream = TitleContainer.OpenStream("p" + playerIndex + "_spritesheet.txt");
            StreamReader reader = new StreamReader(stream);
            spriteSheet = reader.ReadToEnd();

            sourceRect = new Rectangle(0, 0, 66, 92);
        }

        public void Update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= frameTime)
            {
                if (CurrentAnimation == Animations.walk)
                {
                    currentFrame = currentFrame == 11 ? 1 : currentFrame + 1;
                }
                if (CurrentAnimation == Animations.swim && !skipFrame)
                {
                    currentFrame = currentFrame == 2 ? 1 : currentFrame + 1;
                    skipFrame = true;
                }
                else
                {
                    skipFrame = false;
                }
                
                
                elapsed -= frameTime;
            }

            if (CurrentAnimation == Animations.walk)
            {
                int index = spriteSheet.IndexOf(String.Format("p" + playerIndex + "_walk{0:00} = ", currentFrame)) + 12;
                int nextIndex = spriteSheet.IndexOf(" ", index);
                int x = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = spriteSheet.IndexOf(" ", index + 1);
                int y = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = spriteSheet.IndexOf(" ", index + 1);
                int width = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = index + 3;
                int height = int.Parse(spriteSheet.Substring(index, nextIndex - index));

                sourceRect = new Rectangle(x, y, width, height);
                //sourceRect = new Rectangle(width * currentFrame, 0, width, _texture.Height);
            }
            if (CurrentAnimation == Animations.jump)
            {
                int index = spriteSheet.IndexOf(String.Format("p" + playerIndex + "_jump = ")) + 10;
                int nextIndex = spriteSheet.IndexOf(" ", index);
                int x = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = spriteSheet.IndexOf(" ", index + 1);
                int y = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = spriteSheet.IndexOf(" ", index + 1);
                int width = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = index + 3;
                int height = int.Parse(spriteSheet.Substring(index, nextIndex - index));

                sourceRect = new Rectangle(x, y, width, height);
                //sourceRect = new Rectangle(width * currentFrame, 0, width, _texture.Height);
            }
            if (CurrentAnimation == Animations.stand)
            {
                int index = spriteSheet.IndexOf("p" + playerIndex + "_stand = ") + 11;
                int nextIndex = spriteSheet.IndexOf(" ",index);
                int x = int.Parse(spriteSheet.Substring(index,nextIndex-index));
                index = nextIndex;
                nextIndex = spriteSheet.IndexOf(" ",index+1);
                int y = int.Parse(spriteSheet.Substring(index,nextIndex-index));
                index = nextIndex;
                nextIndex = spriteSheet.IndexOf(" ",index+1);
                int width = int.Parse(spriteSheet.Substring(index,nextIndex-index));
                index = nextIndex;
                nextIndex = index+3;
                int height = int.Parse(spriteSheet.Substring(index,nextIndex-index));

                sourceRect = new Rectangle(x, y, width, height);
            }
            if (CurrentAnimation == Animations.swim)
            {
                int index = spriteSheet.IndexOf(String.Format("p" + playerIndex + "_swim{0:00} = ", currentFrame)) + 12;
                int nextIndex = spriteSheet.IndexOf(" ", index);
                int x = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = spriteSheet.IndexOf(" ", index + 1);
                int y = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = spriteSheet.IndexOf(" ", index + 1);
                int width = int.Parse(spriteSheet.Substring(index, nextIndex - index));
                index = nextIndex;
                nextIndex = index + 3;
                int height = int.Parse(spriteSheet.Substring(index, nextIndex - index));

                sourceRect = new Rectangle(x, y, width, height);
                //sourceRect = new Rectangle(width * currentFrame, 0, width, _texture.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 Position, Vector2 Size, Color color, float scale, float Orientation)
        {
            spriteBatch.Draw(_texture, new Vector2(Position.X + Size.X / 2, Game1.ScreenSize.Y - (Position.Y + Size.Y/2)), sourceRect, color, Orientation, Size / 2 /*new Vector2(0, Size.Y)*/, scale, 0, 0);
        }
    }
}
