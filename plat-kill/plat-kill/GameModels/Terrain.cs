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
        #endregion
        #region Methods
        #region Initialize
        public void LoadContent(ContentManager content, String path)
        {
            model = content.Load<Model>(path);
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);
            mesh = new StaticMesh(vertices, indices, new AffineTransform(new Vector3(0, -10, 0)));
            mesh.Tag=model;
        }
        #endregion
        #endregion
    }
}
