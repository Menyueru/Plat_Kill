using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace plat_kill.GameModels.Players
{
    internal class Player : GameModel
    {
        #region Properties

        private Cylinder body;
        private bool airborne;
        private float height;
        private float radius;

        private Vector3 currentVelocity;
        private long defense;
        private long health;
        private long id;
        private float jumpSpeed;
        private long meleePower;
        private long rangePower;
        private float speed;
        private long stamina;


        private Vector3 playerHeadOffset;
        

        #endregion

        #region Getter-Setters
        
        public Cylinder Body
        {
            get { return body; }
            set { body = value; }
        }

        public bool Airborne
        {
            get { return airborne; }
            set { airborne = value; }
        }

        public Vector3 CurrentVelocity
        {
            get { return currentVelocity; }
            set { currentVelocity = value; }
        }

        public long Defense
        {
            get { return defense; }
            set { defense = value; }
        }

        public long Health
        {
            get { return health; }
            set { health = value; }
        }

        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        public float JumpSpeed
        {
            get { return jumpSpeed; }
            set { jumpSpeed = value; }
        }

        public long MeleePower
        {
            get { return meleePower; }
            set { meleePower = value; }
        }

        public Vector3 PlayerHeadOffset
        {
            get { return playerHeadOffset; }
            set { playerHeadOffset = value; }
        }

        public long RangePower
        {
            get { return rangePower; }
            set { rangePower = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public long Stamina
        {
            get { return stamina; }
            set { stamina = value; }
        }

        public float RotationSpeed
        {
            get { return RotationSpeed; }
            set { RotationSpeed = value; }
        }

        public Vector3 Position
        {
            get { return Position; }
            set { Position = value; }
        }

        public Vector3 Rotation
        {
            get { return Rotation; }
            set { Rotation = value; }
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        #endregion

        #region Methods

        #region Checkers
        private bool check_support()
        {
            Vector3 pos = Position;
            Vector3 downDirection = body.OrientationMatrix.Down; //For a cylinder orientation-locked to the Up axis, this is always {0, -1, 0}.  Keeping it generic doesn't cost much.
            var pairs = body.CollisionInformation.Pairs;
            foreach (var pair in pairs)
            {
                if (pair.CollisionRule != CollisionRule.Normal)
                    continue;
                
                foreach (var c in pair.Contacts)
                {
                    //It's possible that a subpair has a non-normal collision rule, even if the parent pair is normal.
                    if (c.Pair.CollisionRule != CollisionRule.Normal)
                        continue;
                    //Compute the offset from the position of the character's body to the contact.
                    Vector3 contactOffset;
                    Vector3.Subtract(ref c.Contact.Position, ref pos, out contactOffset);
                    //Calibrate the normal of the contact away from the center of the object.
                    float dot;
                    Vector3 normal;
                    Vector3.Dot(ref contactOffset, ref c.Contact.Normal, out dot);
                    normal = c.Contact.Normal;
                    if (dot < 0)
                    {
                        Vector3.Negate(ref normal, out normal);
                        dot = -dot;
                    }
                    //Support contacts are all contacts on the feet of the character- a set that include contacts that support traction and those which do not
                    Vector3.Dot(ref normal, ref downDirection, out dot);
                    if (dot > .01f)
                    {
                        return false;
                    }

                }
            }

            return true;
        }

        public void CalculateHeightRadius(out float height, out float radius)
        {
            float maxYOffset = float.MinValue;
            float maxHorizontal = float.MinValue;
            foreach (var mesh in Model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    int stride = part.VertexBuffer.VertexDeclaration.VertexStride;
                    byte[] vertexData = new byte[stride * part.NumVertices];
                    part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1); 
                    for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                    {
                        float x= Math.Abs(BitConverter.ToSingle(vertexData, ndx)-Position.X);
                        float y = Math.Abs(BitConverter.ToSingle(vertexData, ndx + sizeof(float)) - Position.Y);
                        float z= Math.Abs(BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2)-Position.Z);
                        maxYOffset=Math.Max(y, maxYOffset);
                        float tempMax=Math.Max(x, z);
                        maxHorizontal=Math.Max(tempMax, maxHorizontal);
                    }
                }
            }
            height = maxYOffset;
            radius = maxHorizontal;
        }
        #endregion

        #region Movers

        protected void Move()
        {
            Body.LinearVelocity = currentVelocity;
        }

        protected void MoveForward(float dt)
        {
            currentVelocity += World.Forward * (dt * speed);
            currentVelocity.Y = Body.LinearVelocity.Y;
        }

        protected void MoveRight(float dt)
        {
            currentVelocity += World.Right * (dt * speed);
            currentVelocity.Y = Body.LinearVelocity.Y;
        }

        protected void jump()
        {
            Vector3 impulse = new Vector3(0, jumpSpeed, 0);
            Body.ApplyLinearImpulse(ref impulse);
        }

        #endregion

        #region Initialize
        public Player(long id, long health, long stamina, long defense, long meleePower, long rangePower, long speed, long jumpSpeed, float rotationspeed, Vector3 position)
            : base(position,rotationspeed)
        {
            this.id = id;
            this.health = health;
            this.stamina = stamina;
            this.defense = defense;
            this.meleePower = meleePower;
            this.rangePower = rangePower;
            this.speed = speed;
            this.jumpSpeed = jumpSpeed;
            this.playerHeadOffset = new Vector3(0, 10, 0);
            this.currentVelocity = Vector3.Zero;
        }

        public override void Load(ContentManager content, String path)
        {
            base.Load(content, path);
            float height, radius;
            CalculateHeightRadius(out height, out radius);
            body = new Cylinder(Position, height, radius);
            this.height = height;
            this.radius = radius;
        }
        #endregion
        protected void Update(GameTime gameTime)
        {
            Position = body.Position;
            airborne = check_support();
        }
        #endregion
    }
}