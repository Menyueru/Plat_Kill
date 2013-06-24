using BEPUphysics.Collidables;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.GameModels
{
    class Terrain
    {
        #region Properties
        private StaticMesh mesh;
        private Model model;
        private Matrix world;
        private Vector3 position;
        #endregion
        #region Methods
        #region Initialize
        public void LoadContent(ContentManager content, String path)
        {
            position=new Vector3(0, -10, 0);
            model = content.Load<Model>(path);
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);
            mesh = new StaticMesh(vertices, indices, new AffineTransform(position));
            mesh.Tag=model;
        }
        #endregion

        public void Draw(Matrix view, Matrix projection)
        {
            world =  Matrix.CreateTranslation(position);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = projection;
                    effect.View = view;
                    effect.World = world;
                }
            }
        }
        #endregion
    }
}
