using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using _2DPlatformer.GL;

namespace _2DPlatformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private static Game1 _instance;
        public static Game1 Instance { get { return _instance; } }

        private static Vector2 _screenSize;
        public static Vector2 ScreenSize { get { return _screenSize; } }

        public ResolutionIndependentRenderer _resolutionIndependence;
        public Camera2D _camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _instance = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.IsFullScreen = true;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();

            _resolutionIndependence = new ResolutionIndependentRenderer(this);
            // TODO: Add your initialization logic here

            _resolutionIndependence.VirtualWidth = 1920;
            _resolutionIndependence.VirtualHeight = (int)(1920/((float)graphics.PreferredBackBufferWidth/graphics.PreferredBackBufferHeight));//1080;
            _resolutionIndependence.ScreenWidth = graphics.PreferredBackBufferWidth;
            _resolutionIndependence.ScreenHeight = graphics.PreferredBackBufferHeight;
            _resolutionIndependence.BackgroundColor = Color.Black;
            _resolutionIndependence.Initialize();

            _camera = new Camera2D(_resolutionIndependence);
            _camera.Zoom = 1f;
            _camera.Position = new Vector2(_resolutionIndependence.VirtualWidth / 2, _resolutionIndependence.VirtualHeight / 2);
            _camera.RecalculateTransformationMatrices();

            _screenSize = new Vector2(_resolutionIndependence.VirtualWidth, _resolutionIndependence.VirtualHeight);

            Art.LoadContent(Content);
            Sound.Load(Content);

            ScreenManager.Initialize(Content);
            ScreenManager.PushScreen(new MainMenu(),false);
            ScreenManager.PushScreen(new SplashScreen(), false);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    this.Exit();

            // TODO: Add your update logic here
            ScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(new Color(208,244,247));

            _resolutionIndependence.BeginDraw();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, _camera.GetViewTransformationMatrix());
            //spriteBatch.Begin();
            spriteBatch.Draw(Art.GetContent<Texture2D>("art/bg"), new Rectangle(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y), Color.White);
            ScreenManager.Draw(spriteBatch);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
