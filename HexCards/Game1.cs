#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace HexCards
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int DEFAULT_WIDTH = 768;
        const int DEFAULT_HEIGHT = 1280;
        float scale;

        SpriteFont myFont;
        Hexboard board;
        PlayerHand hand;

        int currentWidth;
        int currentHeight;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 768;

            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            currentWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            currentHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            IsMouseVisible = true;

            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            scale = (float)GraphicsDevice.Viewport.Width / (float)DEFAULT_WIDTH;
            if (!pid.ToString().Contains("Win"))
            {
                //scale = (float)GraphicsDevice.Viewport.Width / (float)graphics.PreferredBackBufferWidth;
                graphics.PreferredBackBufferWidth = currentWidth;
                graphics.PreferredBackBufferHeight = currentHeight;
                graphics.IsFullScreen = true;
            }

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
            //hex = Content.Load<Texture2D>("");
            myFont = Content.Load<SpriteFont>("Arial");
            board = new Hexboard(Content, new Point(GraphicsDevice.Viewport.Width / 2, (int)(GraphicsDevice.Viewport.Height * 0.4)), scale);
            hand = new PlayerHand(Content, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, scale, board);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            MouseState mouse = Mouse.GetState();
            TouchCollection touchColl = TouchPanel.GetState();
            hand.Update(gameTime, mouse, touchColl);
            base.Update(gameTime);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(51, 30, 21));

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            //spriteBatch.DrawString(myFont, (GraphicsDevice.Viewport.Height / 2).ToString(), new Vector2(0, 0), Color.White);
            board.Draw(spriteBatch);
            hand.Draw(spriteBatch);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
