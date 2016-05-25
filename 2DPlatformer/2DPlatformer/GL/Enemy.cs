using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2DPlatformer.GL
{
    public class Enemy : Entity
    {
        public EnemyAnimation enemyAnimation;

        public bool fly;
        public string type;
        private bool dead;
        private float elapsed = 0;

        public bool isActive { get { return !dead; } }
        public int Strength { get; set; }

        public override Rectangle Bounds { get { return new Rectangle((int)(Position.X), (int)(Game1.ScreenSize.Y - Position.Y - enemyAnimation.Size.Y /*Size.Y*/), (int)enemyAnimation.Size.X, (int)enemyAnimation.Size.Y); } }
        public override Vector2 Size { get { return enemyAnimation.Size; } }

        static Random rnd = new Random();

        private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();

        public Enemy()
        {

        }

        public Enemy(string type, Vector2 position)
        {
            this.type = type;
            this.Position = position;
            Init();
        }

        private void AddBehaviour(IEnumerable<int> behaviour)
        {
            behaviours.Add(behaviour.GetEnumerator());
        }

        private void ApplyBehaviours()
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                if (!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);
            }
        }

        /*public static Enemy CreateFish(Vector2 position)
        {
            Enemy fish = new Enemy();
            fish.type = "fish";
            fish.image = Art.GetContent<Texture2D>("art/enemies/fish/right/anim1");
            fish.Position = position;
            fish.enemyAnimation = new EnemyAnimation("fish","right");
            fish.fly = true;
            fish.Strength = 1;
            return fish;
        }

        public static Enemy CreateFly(Vector2 position)
        {
            Enemy fly = new Enemy();
            fly.type = "fly";
            fly.image = Art.GetContent<Texture2D>("art/enemies/fly/right/anim1");
            fly.Position = position;
            fly.enemyAnimation = new EnemyAnimation("fly", "right");
            fly.fly = true;
            fly.Strength = 1;
            return fly;
        }

        public static Enemy CreateSlime(Vector2 position)
        {
            Enemy slime = new Enemy();
            slime.type = "slime";
            slime.image = Art.GetContent<Texture2D>("art/enemies/slime/right/anim1");
            slime.Position = position;
            slime.enemyAnimation = new EnemyAnimation("slime", "right");
            slime.fly = false;
            slime.Strength = 1;
            return slime;
        }

        public static Enemy CreateSpinner(Vector2 position)
        {
            Enemy spinner = new Enemy();
            spinner.type = "spinner";
            spinner.image = Art.GetContent<Texture2D>("art/enemies/spinner/right/anim1");
            spinner.Position = position;
            spinner.enemyAnimation = new EnemyAnimation("spinner", "right");
            spinner.fly = false;
            spinner.Strength = 1;
            return spinner;
        }*/

        private void Init()
        {
            this.image = Art.GetContent<Texture2D>("art/enemies/" + type + "/right/anim1");
            this.enemyAnimation = new EnemyAnimation(type, "right");
            if (type == "fly" || type == "fish")
                this.fly = true;
            else
                this.fly = false;

            this.Strength = 1;

            switch (type)
            {
                case "fish":
                    AddBehaviour(MoveSideways(2, 5));
                    break;
                case "fly":
                    AddBehaviour(MoveSideways(3, 6));
                    break;
                case "slime":
                    AddBehaviour(MoveSideways(1, 3));
                    break;
                case "spinner":
                    AddBehaviour(MoveSideways(5, 8));
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            /*Rectangle enemy = new Rectangle((int)(Position.X - 30 + Velocity.X), (int)(Game1.ScreenSize.Y - Position.Y - Size.Y), (int)(Size.X + 60), (int)(Size.Y));
            Rectangle rightDown = new Rectangle((int)(Position.X - 30 + Velocity.X+70), (int)(Game1.ScreenSize.Y - Position.Y - Size.Y+10), (int)(Size.X + 60), (int)(Size.Y));
            Rectangle leftDown = new Rectangle((int)(Position.X - 30 + Velocity.X-70), (int)(Game1.ScreenSize.Y - Position.Y - Size.Y+10), (int)(Size.X + 60), (int)(Size.Y));

            if (!EntityManager.currentEntityManager.hasRoom(enemy, null) || (!fly && (EntityManager.currentEntityManager.hasRoom(rightDown, null) || EntityManager.currentEntityManager.hasRoom(leftDown, null))))
            {
                Velocity.X = -Velocity.X;
                enemyAnimation.direction = Velocity.X > 0 ? "right" : "left";
            }*/
            if (behaviours.Count == 0)
                Init();

            if (!dead)
                ApplyBehaviours();

            Position += Velocity;

            if (IsExpired && !dead)
            {
                IsExpired = false;
                Velocity.X = 0;
                dead = true;
                image = Art.GetContent<Texture2D>("art/enemies/" + type + "/" + enemyAnimation.direction + "/dead");
            }

            if (dead)
            {
                elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsed >= 0.5)
                {
                    IsExpired = true;
                }
            }

            enemyAnimation.Update(gameTime);
        }

        IEnumerable<int> MoveSideways(int minVelocity, int maxVelocity)
        {
            Velocity.X = rnd.Next(minVelocity, maxVelocity);
            enemyAnimation.frameTime = 1 / (Math.Abs(Velocity.X)*3);

            while (true)
            {
                //Rectangle rightDown = new Rectangle((int)(Position.X - 30 + Velocity.X + 70), (int)(Game1.ScreenSize.Y - Position.Y - Size.Y + 10), (int)(Size.X + 60), (int)(Size.Y));
                //Rectangle leftDown = new Rectangle((int)(Position.X - 30 + Velocity.X - 70), (int)(Game1.ScreenSize.Y - Position.Y - Size.Y + 10), (int)(Size.X + 60), (int)(Size.Y));
                Rectangle rightDown = new Rectangle((int)(Position.X + Velocity.X + Size.X), (int)(Game1.ScreenSize.Y - Position.Y), (int)(1), (int)(1));
                Rectangle leftDown = new Rectangle((int)(Position.X + Velocity.X), (int)(Game1.ScreenSize.Y - Position.Y), (int)(1), (int)(1));
            
                if (Position.X == 0 || !EntityManager.currentEntityManager.hasRoom(Bounds, null) || (!fly && (EntityManager.currentEntityManager.hasRoom(rightDown, null) || EntityManager.currentEntityManager.hasRoom(leftDown, null))))
                {
                    Velocity.X = -Velocity.X;
                    enemyAnimation.direction = Velocity.X > 0 ? "right" : "left";
                }

                yield return 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float scale = 1f)
        {
            if (!dead)
                enemyAnimation.Draw(spriteBatch, Position, Size, color, scale, Orientation);
            else
                base.Draw(spriteBatch, scale);
            //spriteBatch.DrawString(Art.Font, "EnemyPosition: " + Position.X + ", " + Position.Y + "   Rectangle: " + Bounds.X + ", " + Bounds.Y + ", " + Bounds.Height + ", " + Bounds.Width, new Vector2(0, 0), Color.Black);
            //base.Draw(spriteBatch, scale);
        }

        public override Entity Clone(Vector2 pos)
        {
            Enemy clone = new Enemy(type, pos);
            return clone;
        }
    }
}
