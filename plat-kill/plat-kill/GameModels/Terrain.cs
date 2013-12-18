using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.NarrowPhaseSystems;
using BEPUutilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using plat_kill.GameModels.Players.Helpers;
using plat_kill.GameModels.Players.Helpers.AI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

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

        private PKGame game;

        private Dictionary<String, SceneObject> models;

        private Matrix world;

        public StaticMesh Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }

        public Terrain(string path, PKGame game)
        {
            this.models = new Dictionary<string, SceneObject>();
            this.asset = path;
            this.game = game;
        }

        public void LoadContent(ContentManager content)
        {
            this.effect = content.Load<Effect>("Effects\\terrain");

            using (XmlTextReader reader = new XmlTextReader(this.asset))
            {
                XmlDocument sceneFile = new XmlDocument();
                sceneFile.Load(reader);

                Load(sceneFile, content);

                reader.Close();
            }

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

            world = Matrix.CreateTranslation(0, 0, Height);
            mesh = new StaticMesh(verts, indices, new AffineTransform(world.Translation));
            mesh.ImproveBoundaryBehavior = false;
            mesh.IgnoreShapeChanges = true;
        }


        public World CreateWorld()
        {
            int conver = Point3D.vectortrans;
            World place = new World((Width / conver)+1, 1, (Height / conver)+1);

            for (int x = 0; x < place.Right; x++)
            {
                for (int y = 0; y < place.Top; y++)
                {
                    for (int z = 0; z < place.Back; z++)
                    {
                        BoundingBox box = new BoundingBox(new Vector3(x * conver, minheight+5, z * conver), new Vector3(((x+1) * conver)-1, minheight+25, ((z+1) * conver)-1));
                        List<BroadPhaseEntry> list = new List<BroadPhaseEntry>(); 
                        game.Space.BroadPhase.QueryAccelerator.GetEntries(box, list);
                        Ray ray;
                        
                        if (list.Count > 0)
                        {
                            for (int k = 0; k<list.Count; k++)
                            {
                                StaticMesh temp = list[k] as StaticMesh;
                                var pair = new CollidablePair(temp, new Box(new Vector3(x * conver, minheight+5, z * conver),conver, 25, conver).CollisionInformation);
                                var pairHandler = NarrowPhaseHelper.GetPairHandler(ref pair);
                                pairHandler.SuppressEvents = true;
                                pairHandler.UpdateCollision(0);
                                pairHandler.SuppressEvents = false;

                                if (pairHandler.Colliding)
                                {
                                    place.MarkPosition(new Point3D(x, y, z), true);
                                    break;
                                }

                                pairHandler.CleanUp();
                                pairHandler.Factory.GiveBack(pairHandler);
                                
                            }
                        }
                        //prevent the starting square from being blocked
                        /*if ((x + y + z) % 3 == 0 && (x + y + z) != 0)
                        {
                            place.MarkPosition(new Point3D(x, y, z), true);
                        }*/
                    }
                }
            }


            return place;
        }

        /// <summary>
        /// Attempts to parse a *.scn file.
        /// </summary>
        public void Load(XmlDocument sceneFile, ContentManager content)
        {

            foreach (XmlNode node in sceneFile.DocumentElement.ChildNodes)
            {
                if (node.Name == "SceneObjects")
                {
                    foreach (XmlNode entityNode in node.ChildNodes)
                    {
                        XmlAttributeCollection entityAttributes = entityNode.Attributes;
                        string name = entityAttributes["Name"].InnerText;
                        string assetPath = entityAttributes["AssetPath"].InnerText;

                        string groupPath = string.Empty;

                        if (entityAttributes["GroupPath"] != null)
                        {
                            groupPath = entityAttributes["GroupPath"].InnerText;
                        }

                        Vector3 position = ParseVector3(entityAttributes["Position"].InnerText);
                        Vector3 rotation = ToEulerAngles(ParseQuaternion(entityAttributes["Rotation"].InnerText));
                        Vector3 scale = ParseVector3(entityAttributes["Scale"].InnerText);

                        //bool visible = bool.Parse(entityAttributes["Visible"].InnerText);

                        bool placeOnTerrain = true;

                        if (entityAttributes["PlaceOnTerrain"] != null)
                        {
                            placeOnTerrain = bool.Parse(entityAttributes["PlaceOnTerrain"].InnerText);
                        }

                        SceneObject entity = new SceneObject(position, rotation, scale);
                        entity.Load(content, assetPath);

                        Add(name, entity);
                    }
                }
                else if (node.Name == "SpawnPoints")
                {
                    List<Vector3> SpawnPoints = new List<Vector3>();
                    foreach (XmlNode entityNode in node.ChildNodes)
                    {
                        XmlAttributeCollection entityAttributes = entityNode.Attributes;
                        string name = entityAttributes["Name"].InnerText;
                        Vector3 Position = ParseVector3(entityAttributes["Position"].InnerText);
                        SpawnPoints.Add(Position);
                    }
                    game.PlayerManager.SpawnPoints = SpawnPoints;
                }
                else if (node.Name == "WeaponPoints")
                {
                    List<Vector3> SpawnPoints = new List<Vector3>();
                    foreach (XmlNode entityNode in node.ChildNodes)
                    {
                        XmlAttributeCollection entityAttributes = entityNode.Attributes;
                        string name = entityAttributes["Name"].InnerText;
                        Vector3 Position = ParseVector3(entityAttributes["Position"].InnerText);
                        SpawnPoints.Add(Position);
                    }
                    game.WeaponManager.SpawnPoints = SpawnPoints;
                }
                else if (node.Name == "Terrain")
                {
                    XmlAttributeCollection terrainAttributes = node.Attributes;
                    String heightMapPath = terrainAttributes["Path"].InnerText;


                    if (string.IsNullOrEmpty(heightMapPath))
                    {
                        continue;
                    }


                    try
                    {
                        this.bitmap = content.Load<Texture2D>(heightMapPath);
                    }
                    catch
                    {
                        // MessageBox.Show("Height map was not found.");
                        continue;
                    }


                    if (node.HasChildNodes)
                    {
                        int counter = 0;
                        this.textureAssets = new String[4];
                        foreach (XmlNode materialNode in node.ChildNodes[0].ChildNodes)
                        {
                            foreach (XmlNode child in materialNode.ChildNodes)
                            {
                                if (child.Name == "Path")
                                {
                                    this.textureAssets[counter++] = Path.ChangeExtension(child.InnerText, null);
                                }
                            }

                        }
                    }
                }
            }
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
                    vertices[x + y * Width].TextureCoordinate.X = x / 25.5f;
                    vertices[x + y * Width].TextureCoordinate.Y = y / 25.5f;

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

        public void AddToSpace(BEPUphysics.Space space)
        {
            space.Add(mesh);
            foreach (String key in models.Keys)
            {
                space.Add(models[key].Mesh);
            }
        }

        public Vector3 ToEulerAngles(Quaternion q)
        {
            // Store the Euler angles in radians
            Vector3 pitchYawRoll = new Vector3();

            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = q.X * q.Y + q.Z * q.W;

            if (test > 0.499f * unit)
            {
                // Singularity at north pole
                pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W);  // Yaw
                pitchYawRoll.X = MathHelper.PiOver2;                // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else if (test < -0.499f * unit)
            {
                // Singularity at south pole
                pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
                pitchYawRoll.X = -MathHelper.PiOver2;               // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }

            pitchYawRoll.Y = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw);       // Yaw
            pitchYawRoll.X = (float)Math.Asin(2 * test / unit);                                             // Pitch
            pitchYawRoll.Z = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw);      // Roll

            return pitchYawRoll;
        }


        public Vector3 ParseVector3(string value)
        {
            string[] attributeSplit = value.Split();

            Vector3 parsedVector = new Vector3(float.Parse(attributeSplit[0], CultureInfo.InvariantCulture),
                                               float.Parse(attributeSplit[1], CultureInfo.InvariantCulture),
                                               float.Parse(attributeSplit[2], CultureInfo.InvariantCulture));

            return parsedVector;
        }
        public Quaternion ParseQuaternion(string value)
        {
            string[] attributeSplit = value.Split();

            Quaternion parsedVector = new Quaternion(float.Parse(attributeSplit[0], CultureInfo.InvariantCulture),
                                                     float.Parse(attributeSplit[1], CultureInfo.InvariantCulture),
                                                     float.Parse(attributeSplit[2], CultureInfo.InvariantCulture),
                                                     float.Parse(attributeSplit[3], CultureInfo.InvariantCulture));

            return parsedVector;
        }

        /// <summary>
        /// Adds a new object to the scene.
        /// </summary>
        public void Add(string name, SceneObject entity)
        {
            if (models.ContainsKey(name))
            {
                //MessageBox.Show("An entity with this name already exists " +
                //"in the scene, please choose another name");
            }
            else
            {
                models.Add(name, entity);
            }
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
            foreach (String key in models.Keys)
            {
                models[key].Draw(View, Projection);
            }
        }
    }
}
