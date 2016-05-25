using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    public class Character : Entity
    {

        public int keyLeft { get; set; }
        public int keyRight { get; set; }
        public int keyJump { get; set; }
        public int keyDown { get; set; }
        public bool Hurt { get; set; }
        public bool Flip { get; set; }
        public int Score { get; set; }
        public bool floating { get; set; }
        public bool inWater { get; set; }

        public int playerIndex { get; set; }

        private Texture2D HUD { get { return Art.GetContent<Texture2D>("art/character/hud_p" + playerIndex); } }

        public override Rectangle Bounds { get { return new Rectangle((int)(Position.X), (int)(Game1.ScreenSize.Y - Position.Y - Size.Y), (int)Size.X, (int)Size.Y); } }

        private int lives;

        private float oritentationStep = 0.1f;

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;

        public bool isActive = true;

        readonly Vector2 gravity = new Vector2(0, -19.8f);

        private Vector2 oldPosition;

        CharacterAnimation characterAnimation;

        bool doubleJumpAvailable = false;

        float scale = 0.8f;
        float step = 0.01f;

        float elapsed;

        static Random rnd = new Random();

        public Character()
        {
            image = Art.CharacterFront;
            lives = 6;
            //if (playerIndex == 0) playerIndex = 1;
            //characterAnimation = new Animation(playerIndex);//Art.CharacterFront, 100, 1, 70);
            //setupInput();
        }

        public void setupInput(int left = (int) Keys.Left, int right = (int) Keys.Right, int jump = (int) Keys.Up, int down = (int) Keys.Down)
        {
            keyLeft = left;
            keyRight = right;
            keyJump = jump;
            keyDown = down;
        }

        public override void Update(GameTime gameTime)
        {
            if (characterAnimation == null)
                characterAnimation = new CharacterAnimation(playerIndex);

            if (keyLeft == 0 || keyRight == 0 || keyJump == 0)
                setupInput();

            keyboardState = Keyboard.GetState();

            if (!Hurt)
            {
                if (keyboardState.IsKeyDown((Keys)keyRight))
                    Velocity.X = 5;

                if (keyboardState.IsKeyDown((Keys)keyLeft))
                    Velocity.X = -5;

                if ((keyboardState.IsKeyDown((Keys)keyDown) || GamePad.GetState((PlayerIndex)playerIndex).Buttons.B == ButtonState.Pressed)  && inWater)
                    Velocity.Y = -3;

                if ((keyboardState.IsKeyDown((Keys)keyJump) || GamePad.GetState((PlayerIndex)playerIndex).Buttons.A == ButtonState.Pressed) && (isGrounded() || (doubleJumpAvailable && oldKeyboardState.IsKeyUp((Keys) keyJump))))
                {
                    Velocity.Y = 12;
                    if (!isGrounded())
                        doubleJumpAvailable = false;
                }
                if ((keyboardState.IsKeyDown((Keys)keyJump) || GamePad.GetState((PlayerIndex)playerIndex).Buttons.A == ButtonState.Pressed) && inWater)
                {
                    Velocity.Y = 9;
                }
                if (GamePad.GetState((PlayerIndex)playerIndex).ThumbSticks.Left.X != 0)
                    Velocity.X = GamePad.GetState((PlayerIndex)playerIndex).ThumbSticks.Left.X * 5;
            }

            oldKeyboardState = keyboardState;

            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!floating)
                Velocity += gravity * time;
            else 
            {
                if (Velocity.Y < 2){
                    Velocity -= gravity * time;
                }
                floating = false;
            }
            oldPosition = new Vector2(Position.X, Position.Y);

            Position += Velocity;// *time;
            if (Position.X < 0)
                Position.X = 0;
            //Position = Vector2.Clamp(Position, Size -Size, Game1.ScreenSize - Size + new Vector2(EntityManager.currentEntityManager.cameraPosition,1000));

            inWater = false;
            if (!EntityManager.currentEntityManager.hasRoom(Bounds,this))
                  Position = EntityManager.currentEntityManager.availableMovement(oldPosition, Position, Bounds);

            StopMovingIfBlocked();

            if (inWater)
            {
                if (characterAnimation.CurrentAnimation != CharacterAnimation.Animations.swim)
                    characterAnimation.CurrentAnimation = CharacterAnimation.Animations.swim;
                
            } else if (!isGrounded())
            {
                if (characterAnimation.CurrentAnimation != CharacterAnimation.Animations.jump)
                    characterAnimation.CurrentAnimation = CharacterAnimation.Animations.jump;
            } else
            {
                //doubleJumpAvailable = true;
                if (Velocity.X != 0)
                {
                    if (characterAnimation.CurrentAnimation != CharacterAnimation.Animations.walk)
                        characterAnimation.CurrentAnimation = CharacterAnimation.Animations.walk;
                    /*if (characterAnimation.Texture != Art.CharacterWalkRight)
                    {
                        characterAnimation.Texture = Art.CharacterWalkRight;
                        characterAnimation.frames = 10;
                        characterAnimation.frameTime = 0.04f;
                    }*/
                }
                else
                {
                    if (characterAnimation.CurrentAnimation != CharacterAnimation.Animations.stand)
                        characterAnimation.CurrentAnimation = CharacterAnimation.Animations.stand;
                    /*characterAnimation.Texture = Art.CharacterFront;
                    characterAnimation.frames = 0;
                    characterAnimation.frameTime = 0.1f;*/
                }
            }

            if (Flip)
            {
                Orientation += oritentationStep;
                if (Math.Abs(Orientation) >= 6.28318531f || Velocity.Y <=0)
                {
                    Flip = false;
                    Orientation = 0;
                }
            }
            else
            {
                if (Velocity.X > 0)
                    oritentationStep = 0.12f;
                else if (Velocity.X < 0)
                    oritentationStep = -0.12f;
            }

            if (Velocity.X > 0)
                characterAnimation.right = true;
            else if (Velocity.X < 0)
                characterAnimation.right = false;
            if (Hurt)
                Velocity.X *= 0.9f;
            else
                Velocity.X = 0;

            if (Math.Abs(Velocity.X) <= 0.3f)
                Velocity.X = 0;

            if (Hurt)
            {
                elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsed >= 0.4f)
                {
                    Hurt = false;
                    color = Color.White;
                    elapsed = 0;
                }
            }

            if (Position.Y <= 0 || lives <= 0)
                IsExpired = true;

            characterAnimation.Update(gameTime);

            if (scale > 1f || scale < 0.8f)
                step = -step;

            scale += step;
        }

        private void StopMovingIfBlocked()
        {
            Vector2 lastMovement = Position - oldPosition;
            if (lastMovement.X == 0) { 
                 Velocity *= Vector2.UnitY; }
            if (lastMovement.Y == 0) {
                Velocity *= Vector2.UnitX; }
        }

        public void Hit(int lives = 1)
        {
            this.lives -= lives;
            Hurt = true;
            color = Color.Red;
            int coins = 0;
            if (Score > 3)
                coins = 3;
            else
                coins = Score;

            Score -= coins;

            for (int i = 0; i < coins; i++)
            {
                EntityManager.currentEntityManager.addEntity(new Coin(Position) { Velocity = new Vector2(i*3,0),Collectable = false });
            }
        }

        public bool isGrounded()
        {
            Rectangle onePixelLower = Bounds;
            onePixelLower.Offset(0, 1);
            return !EntityManager.currentEntityManager.hasRoom(onePixelLower,this);
        }

        public override void Draw(SpriteBatch spriteBatch, float scale = 1f)
        {
            if (characterAnimation == null)
                characterAnimation = new CharacterAnimation(playerIndex);
            characterAnimation.Draw(spriteBatch,Position,Size,color,scale,Orientation);

            if (Position.X + Size.X < EntityManager.currentEntityManager.cameraPosition.X || Position.Y + Size.Y < EntityManager.currentEntityManager.cameraPosition.Y)
                spriteBatch.Draw(HUD, new Vector2(((Position.X-Size.X/2) > EntityManager.currentEntityManager.cameraPosition.X ?Position.X:(EntityManager.currentEntityManager.cameraPosition.X + 40)), ((Position.Y) > EntityManager.currentEntityManager.cameraPosition.Y ? (Game1.ScreenSize.Y - Position.Y - Size.Y / 2) : Game1.ScreenSize.Y-EntityManager.currentEntityManager.cameraPosition.Y-HUD.Height/2)), null, Color.White, 0f, new Vector2(HUD.Width, HUD.Height) / 2, this.scale, 0, 0);

            if (playerIndex == 0)
            {
                spriteBatch.Draw(HUD, new Vector2(63 + EntityManager.currentEntityManager.cameraPosition.X, 63 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/HUD/coin"), new Vector2((148 + EntityManager.currentEntityManager.cameraPosition.X), 69 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/HUD/" + String.Format("{0:000}", Score).Substring(0, 1)), new Vector2((188 + EntityManager.currentEntityManager.cameraPosition.X), 72 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/HUD/" + String.Format("{0:000}", Score).Substring(1, 1)), new Vector2((210 + EntityManager.currentEntityManager.cameraPosition.X), 72 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/HUD/" + String.Format("{0:000}", Score).Substring(2, 1)), new Vector2((232 + EntityManager.currentEntityManager.cameraPosition.X), 72 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                
                int remainingLives = lives;
                for (int i = 0; i < (lives/2>3?lives/2:3); i++)
                {
                    Texture2D texture = Art.HeartEmpty;
                    if (remainingLives > 1)
                        texture = Art.HeartFull;
                    else if (remainingLives == 1)
                        texture = Art.HeartHalf;

                    spriteBatch.Draw(texture, new Vector2((37 * i + 148 + EntityManager.currentEntityManager.cameraPosition.X), 103 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                    remainingLives -= 2;
                }
            }
            else if (playerIndex == 1)
            {
                spriteBatch.Draw(HUD, new Vector2(Game1.ScreenSize.X - 142 + EntityManager.currentEntityManager.cameraPosition.X, 63 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/HUD/coin"), new Vector2((Game1.ScreenSize.X - 179 + EntityManager.currentEntityManager.cameraPosition.X), 69 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/HUD/" + String.Format("{0:000}", Score).Substring(2, 1)), new Vector2((Game1.ScreenSize.X - 208 + EntityManager.currentEntityManager.cameraPosition.X), 72 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/HUD/" + String.Format("{0:000}", Score).Substring(1, 1)), new Vector2((Game1.ScreenSize.X - 230 + EntityManager.currentEntityManager.cameraPosition.X), 72 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                spriteBatch.Draw(Art.GetContent<Texture2D>("art/HUD/" + String.Format("{0:000}", Score).Substring(0, 1)), new Vector2((Game1.ScreenSize.X - 252 + EntityManager.currentEntityManager.cameraPosition.X), 72 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                int remainingLives = lives;
                for (int i = 0; i < (lives / 2 > 3 ? lives / 2 : 3); i++)
                {
                    Texture2D texture = Art.HeartEmpty;
                    if (remainingLives > 1)
                        texture = Art.HeartFull;
                    else if (remainingLives == 1)
                        texture = Art.HeartHalfRight;

                    spriteBatch.Draw(texture, new Vector2((Game1.ScreenSize.X - (37 * i + 184 - EntityManager.currentEntityManager.cameraPosition.X)), 103 - EntityManager.currentEntityManager.cameraPosition.Y), Color.White);
                    remainingLives -= 2;
                }
            }
            //spriteBatch.DrawString(Art.Font, "Player" + playerIndex +"Position: " + Position.X + ", " + Position.Y, new Vector2(0, 50*playerIndex), Color.Black);
            //base.Draw(spriteBatch, scale);
            //spriteBatch.Draw(Art.GetContent<Texture2D>("art/rect"), Bounds, Color.White);
        }
    }
}
