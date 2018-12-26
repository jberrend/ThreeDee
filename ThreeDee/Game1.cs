using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThreeDee
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model cube;

        Model grass;

        Texture2D character;

        Camera camera;

        float rotation = 0;
        float zPos = 0;
        float xPos = 0;
        float scale = 1;
        float rotationSpeed = 1f / 50f;
        float camZ = 5f;
        float camXRot = 0;
        Point mousePosition;
        Point originalMousePosition;

        float camYRot = 0;

        float mouseX, mouseY;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            Content.RootDirectory = "Content";
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
            graphics.IsFullScreen = true;
            IsMouseVisible = false;
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            originalMousePosition = Mouse.GetState().Position;

            var cameraPosition = new Vector3(0, 2, 5); //  up 2 units and back 5
            var cameraLookAtVector = cameraPosition + new Vector3(0, 0, -100);
            var cameraUpVector = new Vector3(0, 1, 0);

            camera = new Camera(this, cameraPosition, cameraLookAtVector, cameraUpVector);

            Components.Add(camera);

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
            cube = Content.Load<Model>("MonoCube");
            grass = Content.Load<Model>("SmallCube");
            character = Content.Load<Texture2D>("Character");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                xPos +=.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                xPos -=.1f;
            }

            //var newMouseState = Mouse.GetState();
            //var curPos = newMouseState.Position;

            //if (newMouseState.Position != originalMousePosition)
            //{
            //    // do rotations
            //    var mDeltaX = curPos.X - originalMousePosition.X;
            //    var mDeltaY = curPos.Y - originalMousePosition.Y;

            //    Debug.WriteLine("delta (" + mDeltaX + ", " + mDeltaY + ")");

            //    camXRot += mDeltaX * rotationSpeed * gameTime.ElapsedGameTime.Milliseconds;
            //    camYRot += mDeltaY * rotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

            //    Debug.WriteLine("rotations (" + camXRot + ", " + camYRot + ")");

            //    //Mouse.SetPosition(originalMousePosition.X, originalMousePosition.Y);
            //    //mousePosition = Mouse.GetState().Position;

            //    Debug.WriteLine(xPos);
            //}

            //Mouse.SetPosition(originalMousePosition.X, originalMousePosition.Y);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        { 
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            //DrawModel(new Vector3(-4, 0, 0));
            DrawModel(grass, new Vector3(0, 0, 0));
            DrawModel(cube, new Vector3(xPos, 0, 0));

            base.Draw(gameTime);
        }

        void DrawModel(Model model, Vector3 modelPosition)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    //effect.LightingEnabled = true;
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f); // a red light
                    //effect.DirectionalLight0.Direction = new Vector3(0, 0, -1);  // coming along the x-axis
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.CreateTranslation(modelPosition);

                    effect.Alpha = 1; // THIS LINE IS 100% NEEDED!!!!!!

                    //effect.View = Matrix.CreateLookAt(cameraPosition, cameraLookAtVector, cameraUpVector)
                    //    * Matrix.CreateRotationY(MathHelper.ToRadians(camXRot))
                    //    * Matrix.CreateRotationX(MathHelper.ToRadians(camYRot));

                    effect.View = camera.view;

                    float aspectRatio =
                        graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
                    float fieldOfView = MathHelper.PiOver4;
                    float nearClipPlane = 1;
                    float farClipPlane = 200;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
                }
                // Now that we've assigned our properties on the effects we can
                // draw the entire mesh
                mesh.Draw();
            }
        }
    }
}
