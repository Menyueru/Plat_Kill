using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace plat_kill.Components.Camera
{
    public class Camera
    {
        #region Fields

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        public Vector3 transformedReference;
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;

        private CameraState camState;
        private Vector3 cameraReference;
        public Vector3 thirdPersonReference;
        private Vector3 targetPosition;
        private Vector3 targetHeadOffSet;
        private float aspectRatio;
        private float rotationSpeed;
        private float forwardSpeed;
        private float viewAngle;
        private float nearClip;
        private float farClip;
       
        #endregion

        #region Public Propierties

        public CameraState CamState
        {
            get { return camState; }
            set { camState = value; }
        }
        
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
            set { viewMatrix = value; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
            set { projectionMatrix = value; }
        }
        
        public Vector3 CameraReference
        {
            get { return cameraReference; }
            set { cameraReference = value; }
        }
        
        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }
        public Vector3 ThirdPersonReference
        {
            get { return thirdPersonReference; }
            set { thirdPersonReference = value; }
        }
        public float FarClip
        {
            get { return farClip; }
            set { farClip = value; }
        }
        public Vector3 TargetPosition
        {
            get { return targetPosition; }
            set { targetPosition = value; }
        }

        public Vector3 TargetHeadOffSet
        {
            get { return targetHeadOffSet; }
            set { targetHeadOffSet = value; }
        }
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }
        public float ForwardSpeed
        {
            get { return forwardSpeed; }
            set { forwardSpeed = value; }
        }
        public float ViewAngle
        {
            get { return viewAngle; }
            set { viewAngle = value; }
        }
        public float NearClip
        {
            get { return nearClip; }
            set { nearClip = value; }
        }
        #endregion

        #region Constructors
        public Camera(float aspectRatio)
        {
            this.rotationSpeed = 5f/60f;
            this.forwardSpeed = 50f / 60f;
            this.nearClip = 1.0f;
            this.farClip = 2000.0f;
            this.aspectRatio = aspectRatio;
            this.cameraRotation = Vector3.Zero;
            this.camState = CameraState.FirstPersonCamera;
            SetCameraReferences();
            
        }

        public Camera(float rotationSpeed, float fowardSpeed, float nearClip, float farClip) 
        {
            this.rotationSpeed = rotationSpeed;
            this.forwardSpeed = fowardSpeed;
            this.nearClip = nearClip;
            this.farClip = farClip;

            SetCameraReferences();
        }
        #endregion

        #region Private Methods
        private void SetCameraReferences()
        {            
            this.cameraReference = new Vector3(0, 0, -10);
            this.thirdPersonReference = new Vector3(0, 10, 50);
            this.viewAngle = MathHelper.PiOver4;
        }
        #endregion

        #region Public Methods
        public void SetTargetToChase(Vector3 targetPosition, Vector3 targetHeadOffSet) 
        {
            this.targetPosition = targetPosition;
            this.targetHeadOffSet = targetHeadOffSet;
        }


        public void UpdateCameraFirstPerson()
        {
            Matrix rotationMatrix = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y);

            // Transform the head offset so the camera is positioned properly relative to the avatar.
            Vector3 headOffset = Vector3.Transform(targetHeadOffSet, rotationMatrix);

            // Calculate the camera's current position.
            Vector3 cameraPosition = targetPosition + headOffset;

            // Create a vector pointing the direction the camera is facing.
            transformedReference = Vector3.Transform(cameraReference, rotationMatrix);

            // Calculate the position the camera is looking at.
            Vector3 cameraLookat = transformedReference + cameraPosition;

            // Set up the view matrix and projection matrix.
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookat, new Vector3(0.0f, 1.0f, 0.0f));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio, nearClip, farClip);

        }

        public void UpdateCameraThirdPerson()
        {

            Matrix rotationMatrix = Matrix.CreateRotationX(cameraRotation.X)
                                    * Matrix.CreateRotationY(cameraRotation.Y);

            // Create a vector pointing the direction the camera is facing.
           transformedReference = Vector3.Transform(thirdPersonReference, rotationMatrix);

            // Calculate the position the camera is looking from.
           cameraPosition = transformedReference + targetPosition;

            // Set up the view matrix and projection matrix.
            viewMatrix = Matrix.CreateLookAt(cameraPosition, targetPosition, new Vector3(0.0f, 1.0f, 0.0f));

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio, nearClip, farClip);
        }
        #endregion

    }
}
