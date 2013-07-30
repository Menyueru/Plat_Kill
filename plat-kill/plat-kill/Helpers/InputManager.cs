using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace plat_kill.Helpers
{
    class InputManager
    {
        #region Fields
        private KeyboardState lastKeyBoardState;
        private KeyboardState currentKeyBoardState;

        private PKGame game;

        private MouseState lastMouseState;
        private MouseState currentMouseState;

        #endregion 
        
        #region Constructors

        public InputManager(PKGame game) 
        {
            this.game = game;
            
        }
        
        
        #endregion

        #region Methods
        
        public void Update()
        {
            if(game.IsActive)
            {
                lastKeyBoardState = currentKeyBoardState;
                currentKeyBoardState = Keyboard.GetState();

                currentMouseState = Mouse.GetState();
                Mouse.SetPosition(200, 200);
                lastMouseState = Mouse.GetState();
            }

        }

        public bool IsKeyPressed(Keys key)
        {
            return currentKeyBoardState.IsKeyDown(key);
        }

        public bool MouseLeftWasClicked()
        {   
            return currentMouseState.LeftButton == ButtonState.Released &&
                    lastMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool MouseRightWasClicked()
        {
            return currentMouseState.RightButton == ButtonState.Released &&
                    lastMouseState.RightButton == ButtonState.Pressed;
        }

        public bool MouseLeftIsPressed()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool MouseRightIsPressed()
        {
            return currentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMouseScrollingUp() 
        {
            return currentMouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue;
        }

        public bool IsMouseScrollingDown() 
        {
            return currentMouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue;
        }

        public bool IsMouseMovingUp() 
        {
            bool isMovingUp = false;

            if(currentMouseState.Y < lastMouseState.Y)
            {
                isMovingUp = true;
            }
        
            return isMovingUp;
        }

        public bool IsMouseMovingDown()
        {
            bool isMovingDown = false;

            if (currentMouseState.Y > lastMouseState.Y)
            {
                isMovingDown = true;
            }

            return isMovingDown;
        }

        public bool IsMouseMovingRight() 
        {
            bool isMovingRight = false;

            if(currentMouseState.X > lastMouseState.X)
            {
                isMovingRight = true;
            }

            return isMovingRight;
        }

        public bool IsMouseMovingLeft()
        {
            bool isMovingLeft = false;

            if (currentMouseState.X < lastMouseState.X)
            {
                isMovingLeft = true;
            }

            return isMovingLeft;
        }


        #endregion

    }
}
