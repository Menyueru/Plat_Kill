using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using plat_kill.GameModels.Players.Helpers;
using plat_kill.Helpers;
using System;
using BEPUphysics;

namespace plat_kill.GameModels.Players
{
     public class Player : AnimatedModel
    {
        #region Properties

        public CharacterController CharacterController;
        private float radius;
        private bool isLocal;

        private CharacterState charecterState;

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

        public CharacterState CharecterState
        {
            get { return charecterState; }
            set { charecterState = value; }
        }

        public bool IsLocal
        {
            get { return isLocal; }
            set { isLocal = value; }
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

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        #endregion

        #region Methods

        #region Checkers
       
        private void CalculateHeightRadius(out float height, out float radius)
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
                        float x = Math.Abs(BitConverter.ToSingle(vertexData, ndx) - Position.X);
                        float y = Math.Abs(BitConverter.ToSingle(vertexData, ndx + sizeof(float)) - Position.Y);
                        float z = Math.Abs(BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2) - Position.Z);
                        float xoffset = Math.Abs(x - Position.X);
                        float yoffset = Math.Abs(y - Position.Y); ;
                        float zoffset = Math.Abs(z - Position.Z); ;
                        maxYOffset = Math.Max(yoffset, maxYOffset);
                        float tempMax = Math.Max(xoffset, zoffset);
                        maxHorizontal = Math.Max(tempMax, maxHorizontal);
                    }
                }
            }
            height = maxYOffset;
            radius = maxHorizontal;
        }


        #endregion

        #region Movers

        /*protected void Move()
        {
            Body.LinearVelocity = currentVelocity;
        }*/

        protected void MoveForward(float dt)
        {
            Vector2 totalMovement = Vector2.Zero;
            Vector3 movementDir;
            movementDir = World.Forward;
            if (dt > 0)
            {
                totalMovement += Vector2.Normalize(new Vector2(movementDir.X, movementDir.Z));
            }
            else
            {
                totalMovement -= Vector2.Normalize(new Vector2(movementDir.X, movementDir.Z));
            }
            if (totalMovement == Vector2.Zero)
                CharacterController.HorizontalMotionConstraint.MovementDirection += Vector2.Zero;
            else
                CharacterController.HorizontalMotionConstraint.MovementDirection += Vector2.Normalize(totalMovement);
            this.charecterState = CharacterState.Walk;
        }

        protected void MoveRight(float dt)
        {
            Vector2 totalMovement = Vector2.Zero;
            Vector3 movementDir;

            if (dt > 0)
            {
                movementDir = World.Right;
                totalMovement += Vector2.Normalize(new Vector2(movementDir.X, movementDir.Z));
            }
            else
            {
                movementDir = World.Left;
                totalMovement += Vector2.Normalize(new Vector2(movementDir.X, movementDir.Z));
            }
            if (totalMovement == Vector2.Zero)
                CharacterController.HorizontalMotionConstraint.MovementDirection += Vector2.Zero;
            else
                CharacterController.HorizontalMotionConstraint.MovementDirection += Vector2.Normalize(totalMovement);
            this.charecterState = CharacterState.Walk;
        }

        protected void jump()
        {
            CharacterController.Jump();
        }

        #endregion

        #region Initialize
        public Player(long id, long health, long stamina, long defense, long meleePower, long rangePower, long speed, long jumpSpeed, Vector3 position, float rotationspeed, float mass, float width, float height, float length, bool isLocal)
            : base(position, rotationspeed, mass, width, height, length)
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
            this.isLocal = isLocal;
            this.radius = Math.Max(width, length)/2;
            this.charecterState = CharacterState.Idle;
        }


        public void Load(ContentManager content, String path,Space ownspace)
        {
            base.Load(content, path);
            float h, r;
            CalculateHeightRadius(out h, out r);
            CharacterController = new CharacterController(Position, height * h, height * h*.7f, radius * r, mass);
            CharacterController.JumpSpeed = jumpSpeed;
            CharacterController.SlidingJumpSpeed= jumpSpeed * .6f;
            CharacterController.HorizontalMotionConstraint.Speed = speed;
            CharacterController.HorizontalMotionConstraint.MaximumForce *= mass;
            CharacterController.VerticalMotionConstraint.MaximumGlueForce /= mass*mass;
            ownspace.Add(CharacterController);
        }
        #endregion
        
         public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Position = CharacterController.Body.Position;
            
            if (this.CharecterState.Equals(CharacterState.Idle))
            {
                this.Refresh = false;
                this.AnimationPlayer.Update(new TimeSpan(0, 0, 0), true, Matrix.Identity);
            }
            else if (this.CharecterState.Equals(CharacterState.Walk))
            {
                this.Refresh = true;
                this.AnimationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            }
            CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
        }
        #endregion
    }
}