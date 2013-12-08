using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.DataStructures;
using BEPUutilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace plat_kill.GameModels
{
    class SceneObject : StaticModel
    {
        private StaticMesh mesh;

        public StaticMesh Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }

        public SceneObject(Vector3 position, Vector3 rotation, Vector3 scale)
            : base(position, rotation, 0, 0, scale.X, scale.Y, scale.Z)
        {
        }

        public new void Load(ContentManager content, String path)
        {
            base.Load(content, path);
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(Model, out vertices, out indices);
            mesh = new StaticMesh(vertices, indices, new AffineTransform(new Vector3(width, height, length),
                                Quaternion.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z), World.Translation));
            mesh.ImproveBoundaryBehavior = false;
            mesh.IgnoreShapeChanges = true;
            mesh.Tag = this;
        }
    }
}
