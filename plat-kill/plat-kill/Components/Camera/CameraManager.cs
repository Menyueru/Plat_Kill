using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Components.Camera
{
    class CameraManager
    {
        #region Field
        //private PKGame game;
        private Camera activeCamera;
        private CameraState.State camState;

        #endregion

        #region Public Propierties
        public Camera ActiveCamera
        {
            get { return activeCamera; }
        }
        public CameraState.State CamState
        {
            get { return camState; }
            set { camState = value; }
        }        
        #endregion

        #region Constructor
        public CameraManager(Camera camera, CameraState.State camState, PKGame game) 
        {
            this.activeCamera = camera;
            this.camState = camState;
            //this.game = game;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the current camera state.
        /// </summary>
        private void GetCurrentCamera()
        {            

            
        }

        #endregion

        #region Public Methods
        public void UpdateAllCameras() 
        {
            GetCurrentCamera();
            switch (camState)
            {
                case CameraState.State.FirstPersonCamera:
                    activeCamera.UpdateCamera();
                    break;
                case CameraState.State.FirstPersonCameraOffSet:
                    activeCamera.UpdateCameraFirstPerson();
                    break;
                case CameraState.State.ThirdPersonCamera:
                    activeCamera.UpdateCameraThirdPerson();
                    break;
                default:
                    throw new Exception("Invalid Camera Type.");
            }

        }

        #endregion

    }
}
