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

namespace plat_kill.GameModels.Players
{
    class HumanPlayer : Player
    {
        //TODO
        internal HumanPlayer(GraphicsDevice device, ContentManager content) : base (device, content) 
        {

        }

        #region Method
        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            MouseState mouseState = Mouse.GetState();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;


            // Determine rotation amount from input
            Vector2 rotationAmount = -gamePadState.ThumbSticks.Left;
            if (keyboardState.IsKeyDown(Keys.Left))
                rotationAmount.X = 1.0f;
            if (keyboardState.IsKeyDown(Keys.Right))
                rotationAmount.X = -1.0f;
            if (keyboardState.IsKeyDown(Keys.Up))
                rotationAmount.Y = -1.0f;
            if (keyboardState.IsKeyDown(Keys.Down))
                rotationAmount.Y = 1.0f;

            // Scale rotation amount to radians per second
            rotationAmount = rotationAmount * RotationRate * elapsed;

            // Correct the X axis steering when the ship is upside down
            if (Up.Y < 0)
                rotationAmount.X = -rotationAmount.X;


            // Create rotation matrix from rotation amount
            Matrix rotationMatrix =
                Matrix.CreateFromAxisAngle(Right, rotationAmount.Y) *
                Matrix.CreateRotationY(rotationAmount.X);

            // Rotate orientation vectors
            Direction = Vector3.TransformNormal(Direction, rotationMatrix);
            Up = Vector3.TransformNormal(Up, rotationMatrix);

            // Re-normalize orientation vectors
            // Without this, the matrix transformations may introduce small rounding
            // errors which add up over time and could destabilize the ship.
            Direction.Normalize();
            Up.Normalize();

            // Re-calculate Right
            Right = Vector3.Cross(Direction, Up);

            // The same instability may cause the 3 orientation vectors may
            // also diverge. Either the Up or Direction vector needs to be
            // re-computed with a cross product to ensure orthagonality
            Up = Vector3.Cross(Right, Direction);


            // Determine thrust amount from input
            float thrustAmount = gamePadState.Triggers.Right;
            if (keyboardState.IsKeyDown(Keys.Space) || mouseState.LeftButton == ButtonState.Pressed)
                thrustAmount = 1.0f;

            // Calculate force from thrust amount
            Vector3 force = Direction * thrustAmount * ThrustForce;


            // Apply acceleration
            Vector3 acceleration = force / Mass;
            Velocity += acceleration * elapsed;

            // Apply psuedo drag
            Velocity *= DragFactor;

            // Apply velocity
            Position += Velocity * elapsed;


            // Prevent ship from flying under the ground
            position.Y = Math.Max(Position.Y, MinimumAltitude);


            // Reconstruct the ship's world matrix
            World = Matrix.Identity;
            world.Forward = Direction;
            world.Up = Up;
            world.Right = Right;
            world.Translation = Position;
        }
        
        #endregion
    }
}
