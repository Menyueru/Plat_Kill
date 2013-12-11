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

        public HumanPlayer(long id, long health, long stamina, long defense, long meleePower, long rangePower, long speed, long jumpSpeed, Vector3 position, float rotationSpeed, float mass, float width, float height, float length, bool isLocal, PKGame game)
            : base(id, health, stamina, defense, meleePower, rangePower, speed, jumpSpeed, position, rotationSpeed, mass,width,height,length, isLocal)
        {
            this.inputManager = new InputManager(game);
            this.cameraDistance = -200;
            this.game = game;
        }

        #endregion

        #region Method

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float dt = 1;
            inputManager.Update();
            if (game.IsActive)
            {
                #region Keyboard Input
                bool speedModify = false;
                if(inputManager.IsKeyPressed(Keys.LeftShift))
                {
                    speedModify = true;
                }
                if (inputManager.IsKeyPressed(Keys.W))
                {
                    MoveForward(-dt, speedModify);
                }
                else if (inputManager.IsKeyPressed(Keys.S))
                {
                    MoveForward(dt, false);
                }

                if (inputManager.IsKeyPressed(Keys.D))
                {
                    MoveRight(-dt, speedModify);
                }
                else if (inputManager.IsKeyPressed(Keys.A))
                {
                    MoveRight(dt, speedModify);
                }

                if (inputManager.IsKeyPressed(Keys.Space))
                {
                    jump();
                }

                if(inputManager.IsKeyPressed(Keys.R))
                {
                    this.EquippedWeapons[ActiveWeaponIndex].ReloadWeapon(this);
                }

                if(inputManager.IsKeyPressed(Keys.E))
                {
                    this.Dodge();
                }

                #endregion

                #region Mouse Input
                if (inputManager.IsMouseMovingLeft())
                {

                    rotation.Y += RotationSpeed;
                }
                else if (inputManager.IsMouseMovingRight())
                {
                    rotation.Y -= RotationSpeed;
                }

               /* if (inputManager.IsMouseMovingUp())
                {
                    rotation.X += RotationSpeed;
                }
                if (inputManager.IsMouseMovingDown())
                {
                    rotation.X -= RotationSpeed;
                }
                */
                if (inputManager.IsMouseScrollingUp())
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

                if (inputManager.MouseLeftIsPressed())
                {
                    this.EquippedWeapons[ActiveWeaponIndex].Shoot(game.ProjectileManager, this);
                    //game.ProjectileManager.FireProjectile(ProjectileType.Bullet, this);
                }
                else 
                {
                    this.IsShooting = false;
                }

                #endregion
            } 
        }
        
        #endregion
    }
}
