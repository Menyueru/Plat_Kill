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
using plat_kill.Components.Camera;
using plat_kill.GameModels.Weapons;

namespace plat_kill.GameModels.Players
{
    class HumanPlayer : Player
    {
        #region Property

        private InputManager inputManager;

        public Camera activeCamera;
        private PKGame game;

        #endregion

        #region Getter-Setter
        #endregion 

        #region Constructor

        public HumanPlayer(long id, long health, long stamina, long defense, long meleePower, long rangePower, long speed, long jumpSpeed, Vector3 position, float rotationSpeed, float mass, float width, float height, float length, bool isLocal, PKGame game, Camera camera)
            : base(id, health, stamina, defense, meleePower, rangePower, speed, jumpSpeed, position, rotationSpeed, mass,width,height,length, isLocal)
        {
            this.inputManager = new InputManager(game);
            this.game = game;
            this.activeCamera = camera;
        }

        #endregion

        #region Method

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float dt = 1;
            bool weaponchanged=inputManager.IsKeyPressed(Keys.Tab);
            inputManager.Update();
            if (game.IsActive)
            {
                #region Keyboard Input
                if(inputManager.IsKeyPressed(Keys.Escape))
                {
                    game.Exit();
                }

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
                if (inputManager.IsKeyPressed(Keys.Tab) && !weaponchanged)
                {
                    changeToNextWeapon();
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

                if (inputManager.IsMouseMovingUp())
                {
                    double maxAngle;
                    if(activeCamera.CamState.Equals(CameraState.FirstPersonCamera))
                        maxAngle = MathHelper.PiOver4 / 4;
                    else
                        maxAngle = MathHelper.PiOver4 / 6;
                    if (activeCamera.cameraRotation.X <= (maxAngle))
                        activeCamera.cameraRotation.X += 0.04f;
                }
                if (inputManager.IsMouseMovingDown())
                {
                    if (activeCamera.cameraRotation.X > (-MathHelper.PiOver4 / 2))
                        activeCamera.cameraRotation.X -= 0.04f; 
                }

                activeCamera.cameraRotation.Y = Rotation.Y;

                if (inputManager.MouseLeftIsPressed())
                {
                    this.EquippedWeapons[ActiveWeaponIndex].Shoot(game.ProjectileManager, this);
                }
                else if(this.EquippedWeapons[ActiveWeaponIndex].WeaponType.Equals(WeaponType.Range))
                {
                    this.IsShooting = false;
                }

                if(inputManager.MouseRightIsPressed())
                {
                    activeCamera.CamState = CameraState.FirstPersonCamera;
                    
                }
                else
                {
                    activeCamera.CamState = CameraState.ThirdPersonCamera;
                }

                #endregion
            } 
        }
        
        #endregion
    }
}
