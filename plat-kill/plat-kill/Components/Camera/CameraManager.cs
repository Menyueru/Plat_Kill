using Microsoft.Xna.Framework;
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
        private const int zoomInLimit = -200;

        private const int zoomOutLimit = -50;

        private Camera activeCamera;

        #endregion

        #region Public Propierties
        public Camera ActiveCamera
        {
            get { return activeCamera; }
        }
      
        #endregion

        #region Constructor
        public CameraManager(Camera camera) 
        {
            this.activeCamera = camera;
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
        public void UpdateAllCameras(Vector3 targetPosition, Vector3 targetHeadOffSet)
        {
            activeCamera.SetTargetToChase(targetPosition,  targetHeadOffSet);
        
            switch (this.activeCamera.CamState)
            {
                case CameraState.FirstPersonCamera:
                    activeCamera.UpdateCameraFirstPerson();
                    break;
                case CameraState.ThirdPersonCamera:
                    activeCamera.UpdateCameraThirdPerson();
                    break;
                default:
                    throw new Exception("Invalid Camera Type.");
            }
         
        }

        #endregion

    }
}
