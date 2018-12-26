using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ThreeDee
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// class modified from https://github.com/prasadjay/Rotatable-Camera/blob/master/Camera.cs
    /// </summary>
    public class Camera : GameComponent
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public Vector3 cameraPosition { get; protected set; }

        private int fov = 70;
        private float pitch = 0;
        private float yaw = 0;

        private Vector3 cameraDirection;
        private Vector3 cameraUp;

        private Point mouseCenter;

        //defines speed of camera movement
        private float movementSpeed = 0.25F;

        // higher = SLOWER - reduce to increase speed
        private float camRotSensitivity = -0.005f;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // TODO: Construct any child components here

            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            //UpdateVectors();
            CreateLookAt();
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1, 100);
        }
        
        private void CenterMouse()
        {
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            MouseState mState = Mouse.GetState();
            mouseCenter = new Point(mState.X, mState.Y);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            // Set mouse position and do initial get state

            CenterMouse();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            // Move forward and backward
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cameraPosition += cameraDirection * movementSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPosition -= cameraDirection * movementSpeed;

            // Left and right
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * movementSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * movementSpeed;

            MouseState currState = Mouse.GetState();

            // Rotation in the world
            int elapsedMillis = gameTime.ElapsedGameTime.Milliseconds;
            float mDeltaX = currState.X - mouseCenter.X;
            float mDeltaY = currState.Y - mouseCenter.Y;
            mDeltaX *= camRotSensitivity * elapsedMillis;
            mDeltaY *= camRotSensitivity * elapsedMillis;

            // TODO: THESE ARE BACKWARDS FOR SOME REASON, putting them in the "correct" place causing vertical
            //       and horizontal mouse movements to be reversed. Probably has something to do with the camera
            //       direction update below, but not sure.
            pitch += mDeltaY;
            yaw += mDeltaX;

            // bind pitch and yaw

            // loop around to prevent potential over(under) flow issues.
            yaw %= 360;

            // can't look any further that straight up or down
            if (pitch >= 90f)
            {
                pitch = 90f;
            }

            if (pitch <= -90)
            {
                pitch = -90;
            }

            Console.WriteLine(pitch + ", " + yaw);

            //UpdateVectors();
            
            // z axis points "backwards" in a RH coord system, negate to point forwards.
            // (if we don't negate, the other axes are reversed)
            cameraDirection = Vector3.Transform(-Vector3.UnitZ, Matrix.CreateRotationX(MathHelper.ToRadians(pitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(yaw)));


            //cameraDirection = Vector3.Transform(
            //    cameraDirection, Matrix.CreateFromAxisAngle(
            //        cameraUp, mDeltaX));

            //cameraDirection = Vector3.Transform(
            //    cameraDirection, Matrix.CreateFromAxisAngle(
            //        Vector3.Cross(cameraUp, cameraDirection),
            //        -mDeltaY)); // negative to reverse direction of rotation

            // Reset prevMouseState
            CenterMouse();

            //UpdateVectors();

            CreateLookAt();

            base.Update(gameTime);
        }

        // taken from https://learnopengl.com/Getting-started/Camera - but integration not completed
        // TODO: Probably not needed, but here just in case
        private void UpdateVectors()
        {
            Vector3 front = new Vector3();
            front.X = (float) (Math.Cos(MathHelper.ToRadians(yaw)) * Math.Cos(MathHelper.ToRadians(pitch)));
            front.Y = (float)  Math.Sin(MathHelper.ToRadians(pitch));
            front.Z = (float) (Math.Sin(MathHelper.ToRadians(yaw)) * Math.Cos(MathHelper.ToRadians(pitch)));
            cameraDirection = Vector3.Normalize(front);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

    }
}
