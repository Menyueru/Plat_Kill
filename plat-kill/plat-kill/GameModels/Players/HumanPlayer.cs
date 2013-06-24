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
using plat_kill.Helpers;
using plat_kill.GameModels.Projectiles;

namespace plat_kill.GameModels.Players
{
    class HumanPlayer : Player
    {
        #region Property

        private InputManager inputManager;
        private int cameraDistance;
        private PKGame game;
        #endregion


        #region Getter-Setter
        public int CameraDistance
        {
            get { return cameraDistance; }
            set { cameraDistance = value; }
        }
        #endregion 

        #region Constructor

        public HumanPlayer(long id, long health, long stamina, long defense, long meleePower, long rangePower, long speed, long jumpSpeed, Vector3 position, float rotationSpeed, float mass, float width, float height, float length,PKGame game)
            : base(id, health, stamina, defense, meleePower, rangePower, speed, jumpSpeed, position, rotationSpeed, mass,width,height,length)
        {
            this.inputManager = new InputManager();
            this.cameraDistance = -200;
            this.game = game;
        }

        #endregion

        #region Method

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float dt = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentVelocity = new Vector3(0, Body.LinearVelocity.Y, 0);

            inputManager.Update();

            #region Keyboard Input
            if (inputManager.IsKeyPressed(Keys.W))
            {
                MoveForward(dt);
            }
            else if (inputManager.IsKeyPressed(Keys.S))
            {
                MoveForward(-dt);
            }

            if (inputManager.IsKeyPressed(Keys.D))
            {
                MoveRight(dt);
            }
            else if (inputManager.IsKeyPressed(Keys.A))
            {
                MoveRight(-dt);
            }

            Move();

            if (inputManager.IsKeyPressed(Keys.Space) && !Airborne)
            {
                jump();
            }

            if (inputManager.MouseLeftIsPressed())
            {
                Projectile bullet = new Projectile(500, Position + World.Forward, 0, 0.1f, .25f, .25f, .25f,game);
                bullet.LoadContent(game.Content, "Models//Objects//bullet");
                bullet.Shoot(World.Forward);
                game.space.Add(bullet.Body);
            }
            #endregion

            #region Mouse Input
            if (inputManager.IsMouseMovingUp())
            {
                rotation.Y += RotationSpeed;
            }
            else if (inputManager.IsMouseMovingDown())
            {
                rotation.Y -= RotationSpeed;
            }

            if (inputManager.IsMouseMovingLeft())
            {
                rotation.X += RotationSpeed;
            }
            else if (inputManager.IsMouseMovingRight())
            {
                rotation.X -= RotationSpeed;
            }

            if(inputManager.IsMouseScrollingUp())
            {
                cameraDistance += 5;
            }
            else if (inputManager.IsMouseScrollingDown())
            {
                cameraDistance -= 5;
            }
            else 
            {
                cameraDistance = 0;
            }
            #endregion

        }
        
        #endregion
    }
}
