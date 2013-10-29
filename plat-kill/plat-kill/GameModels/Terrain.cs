using BEPUphysics.BroadPhaseEntries;
using BEPUutilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace plat_kill.GameModels
{
    class Terrain
    {
        public struct VertexMultitextured
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector4 TextureCoordinate;
            public Vector4 TexWeights;

            public static int SizeInBytes = (3 + 3 + 4 + 4) * sizeof(float);

            public static VertexElement[]
                VertexElements = new[]
                                     {
                                         new VertexElement(0, VertexElementFormat.Vector3,
                                                           VertexElementUsage.Position, 0),
                                         new VertexElement(sizeof (float)*3,
                                                           VertexElementFormat.Vector3,
                                                           VertexElementUsage.TextureCoordinate, 0),
                                         new VertexElement(sizeof (float)*6,
                                                           VertexElementFormat.Vector4,
                                                           VertexElementUsage.TextureCoordinate, 1),
                                         new VertexElement(sizeof (float)*10,
                                                           VertexElementFormat.Vector4,
                                                           VertexElementUsage.TextureCoordinate, 2),
                                     };
        }

        private StaticMesh mesh;

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Effect effect;
        private Texture2D bitmap;
        private Texture2D[] textures;
        private string asset;
        private string[] textureAssets;

        private float[,] data;
        private int minheight;
        private int maxheight;
        private VertexMultitextured[] vertices;
        private int[] indices;
        private VertexDeclaration vertexDeclaration;

        private Matrix world;

        public StaticMesh Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }

        public Terrain(string heightmap, string[] textures)
        {
            if (textures.Length < 4)
                throw new ArgumentException("Need four terrain textures.");
            this.asset = heightmap;
            this.textureAssets = textures;
        }

        public void LoadContent(ContentManager content)
        {
            this.effect = content.Load<Effect>("Effects//terrain");
            this.bitmap = content.Load<Texture2D>(this.asset);

            this.textures = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                textures[i] = content.Load<Texture2D>(this.textureAssets[i]);

            this.LoadHeightData(); // Width & Height are available from this point.
            this.SetUpVertices();
            this.SetUpIndices();
            this.InitializeNormals();

            Vector3[] verts = new Vector3[vertices.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = vertices[i].Position;
            }
            
            world = Matrix.CreateTranslation(-Width / 2.0f, 0, Height / 2.0f);
            mesh = new StaticMesh(verts, indices,new AffineTransform(world.Translation));
            mesh.ImproveBoundaryBehavior = false;
            mesh.IgnoreShapeChanges = true;
        }

        private void LoadHeightData()
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            Color[] pixels = new Color[Width * Height];
            bitmap.GetData(pixels);

            data = new float[Width, Height];

            minheight = int.MaxValue;
            maxheight = int.MinValue;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    data[x, y] = pixels[x + y * Width].G / 3.1f; 
                    minheight = (int)Math.Min(data[x, y], minheight);
                    maxheight = (int)Math.Max(data[x, y], maxheight);
                }
            }
        }

        private void SetUpVertices()
        {
            if (data == null)
                throw new InvalidOperationException("Call LoadHeightData() first!");

            float step = (maxheight - minheight) / 3;

            vertices = new VertexMultitextured[Width * Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    vertices[x + y * Width].Position = new Vector3(x, data[x, y], -y);
                    vertices[x + y * Width].TextureCoordinate.X = x/25.5f;
                    vertices[x + y * Width].TextureCoordinate.Y = y/25.5f;

                    vertices[x + y * Width].TexWeights = Vector4.Zero;

                    vertices[x + y * Width].TexWeights.X =
                        MathHelper.Clamp(1.0f - Math.Abs(data[x, y]) / step, 0, 1);
                    vertices[x + y * Width].TexWeights.Y =
                        MathHelper.Clamp(1.0f - Math.Abs(data[x, y] - step) / step, 0, 1);
                    vertices[x + y * Width].TexWeights.Z =
                        MathHelper.Clamp(1.0f - Math.Abs(data[x, y] - 2 * step) / step, 0, 1);
                    vertices[x + y * Width].TexWeights.W =
                        MathHelper.Clamp(1.0f - Math.Abs(data[x, y] - 3 * step) / step, 0, 1);

                    float total = vertices[x + y * Width].TexWeights.X;
                    total += vertices[x + y * Width].TexWeights.Y;
                    total += vertices[x + y * Width].TexWeights.Z;
                    total += vertices[x + y * Width].TexWeights.W;

                    vertices[x + y * Width].TexWeights.X /= total;
                    vertices[x + y * Width].TexWeights.Y /= total;
                    vertices[x + y * Width].TexWeights.Z /= total;
                    vertices[x + y * Width].TexWeights.W /= total;
                }
            }

            vertexDeclaration = new VertexDeclaration(VertexMultitextured.VertexElements);
        }

        private void SetUpIndices()
        {
            if (vertices == null)
                throw new InvalidOperationException("Call SetUpVertices() first!");
            indices = new int[(Width - 1) * (Height - 1) * 6];
            int counter = 0;
            for (int y = 0; y < Height - 1; y++)
            {
                for (int x = 0; x < Width - 1; x++)
                {
                    int lowerLeft = x + y * Width;
                    int lowerRight = x + y * Width + 1;
                    int topLeft = x + (y + 1) * Width;
                    int topRight = (x) + (y + 1) * Width + 1;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }

        private void InitializeNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = Vector3.Zero;

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index0 = indices[i * 3];
                int index1 = indices[i * 3 + 1];
                int index2 = indices[i * 3 + 2];

                Vector3 side0 = vertices[index0].Position - vertices[index2].Position;
                Vector3 side1 = vertices[index0].Position - vertices[index1].Position;
                Vector3 normal = Vector3.Cross(side0, side1);

                vertices[index0].Normal += normal;
                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }



        public void Draw(GraphicsDevice device, Matrix View, Matrix Projection)
        {
            effect.CurrentTechnique = effect.Techniques["Multitextured"];
            effect.Parameters["World"].SetValue(this.world);

            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);

            effect.Parameters["LightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));
            effect.Parameters["Ambient"].SetValue(0.4f);

            for (int i = 0; i < 4; i++)
                effect.Parameters["Texture" + i].SetValue(this.textures[i]);

            //*/
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            device.DepthStencilState = DepthStencilState.Default;
            //*/

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, this.vertexDeclaration);
            }
        }
    }
}
