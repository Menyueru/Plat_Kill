using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace plat_kill.GameModels
{
    class SkyBox
    {
         #region Fields
         private Effect skyEffect;
         private TextureCube skyTex;
         private Matrix SkyWorld;

         private IndexBuffer indexBuffer;
         private VertexBuffer vertexBuffer;

         private GraphicsDevice graphicsDevice;

         Vector3 originalView = new Vector3(0, 0, 10);
         Vector3 position = Vector3.Zero;
         #endregion

         #region Constants
         private const int numOfVertices = 8;
         private const int numOfIndexes = 36;
         #endregion 

         #region Getters-Setters
         public IndexBuffer IndexBuffer
         {
             get { return indexBuffer; }
             set { indexBuffer = value; }
         }

         public VertexBuffer VertexBuffer
         {
             get { return vertexBuffer; }
             set { vertexBuffer = value; }
         }
         #endregion

         #region Constructor
         public SkyBox(GraphicsDevice graphicsDevice) 
         {
             this.graphicsDevice = graphicsDevice;
         }
         #endregion

         #region Methods

         public void Draw(Matrix view, Matrix proj) 
         {
             graphicsDevice.SetVertexBuffer(this.vertexBuffer);
             graphicsDevice.Indices = this.indexBuffer;

             skyEffect.Parameters["WVP"].SetValue(SkyWorld * Matrix.CreateLookAt(position, originalView, Vector3.Up) 
                 * Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio, 1, 20));
             skyEffect.CurrentTechnique.Passes[0].Apply();

             graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numOfVertices, 0, numOfIndexes / 3);
         }

         public void Load(ContentManager Content, string SkyEffect, string SkyBoxTexture)
         {
             this.skyEffect = Content.Load<Effect>(SkyEffect);
             this.skyTex = Content.Load<TextureCube>(SkyBoxTexture);

             this.skyEffect.Parameters["tex"].SetValue(skyTex);

             this.SkyWorld = Matrix.Identity;

             this.CreateCubeVertexBuffer();
             this.CreateCubeIndexBuffer();
         }

         private void CreateCubeIndexBuffer()
         {
             UInt16[] cubeIndices = new UInt16[numOfIndexes];

             //bottom face
             cubeIndices[0] = 0;
             cubeIndices[1] = 2;
             cubeIndices[2] = 3;
             cubeIndices[3] = 0;
             cubeIndices[4] = 1;
             cubeIndices[5] = 2;

             //top face
             cubeIndices[6] = 4;
             cubeIndices[7] = 6;
             cubeIndices[8] = 5;
             cubeIndices[9] = 4;
             cubeIndices[10] = 7;
             cubeIndices[11] = 6;

             //front face
             cubeIndices[12] = 5;
             cubeIndices[13] = 2;
             cubeIndices[14] = 1;
             cubeIndices[15] = 5;
             cubeIndices[16] = 6;
             cubeIndices[17] = 2;

             //back face
             cubeIndices[18] = 0;
             cubeIndices[19] = 7;
             cubeIndices[20] = 4;
             cubeIndices[21] = 0;
             cubeIndices[22] = 3;
             cubeIndices[23] = 7;

             //left face
             cubeIndices[24] = 0;
             cubeIndices[25] = 4;
             cubeIndices[26] = 1;
             cubeIndices[27] = 1;
             cubeIndices[28] = 4;
             cubeIndices[29] = 5;

             //right face
             cubeIndices[30] = 2;
             cubeIndices[31] = 6;
             cubeIndices[32] = 3;
             cubeIndices[33] = 3;
             cubeIndices[34] = 6;
             cubeIndices[35] = 7;

             indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, numOfIndexes, BufferUsage.WriteOnly);
             indexBuffer.SetData<UInt16>(cubeIndices);

         }

         private void CreateCubeVertexBuffer()
         {
             Vector3[] cubeVertices = new Vector3[numOfVertices];

             cubeVertices[0] = new Vector3(-1, -1, -1);
             cubeVertices[1] = new Vector3(-1, -1, 1);
             cubeVertices[2] = new Vector3(1, -1, 1);
             cubeVertices[3] = new Vector3(1, -1, -1);
             cubeVertices[4] = new Vector3(-1, 1, -1);
             cubeVertices[5] = new Vector3(-1, 1, 1);
             cubeVertices[6] = new Vector3(1, 1, 1);
             cubeVertices[7] = new Vector3(1, 1, -1);

             VertexDeclaration VertexPositionDeclaration = new VertexDeclaration
                 (
                     new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
                 );

             vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionDeclaration, numOfVertices, BufferUsage.WriteOnly);
             vertexBuffer.SetData<Vector3>(cubeVertices);
         }
             
         #endregion
    }
}
