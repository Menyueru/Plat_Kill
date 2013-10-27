using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUutilities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using plat_kill.GameModels.Players.Helpers;
using plat_kill.Helpers;
using System;

namespace plat_kill.GameModels.Players
{
     public class Player : AnimatedModel
    {
        #region Properties

        private Cylinder body;
        private bool airborne;
        private float radius;
        private bool isLocal;

    #region helpers
        private CharacterState charecterState;
        /// <summary>
        /// Gets the manager responsible for finding places for the character to step up and down to.
        /// </summary>
        public StepManager StepManager { get; private set; }

        /// <summary>
        /// Gets the manager responsible for crouching, standing, and the verification involved in changing states.
        /// </summary>
        public StanceManager StanceManager { get; private set; }

        /// <summary>
        /// Gets the support system which other systems use to perform local ray casts and contact queries.
        /// </summary>
        public QueryManager QueryManager { get; private set; }

        /// <summary>
        /// Gets the constraint used by the character to handle horizontal motion.  This includes acceleration due to player input and deceleration when the relative velocity
        /// between the support and the character exceeds specified maximums.
        /// </summary>
        public HorizontalMotionConstraint HorizontalMotionConstraint { get; private set; }

        /// <summary>
        /// Gets the constraint used by the character to stay glued to surfaces it stands on.
        /// </summary>
        public VerticalMotionConstraint VerticalMotionConstraint { get; private set; }

        /// <summary>
        /// Gets the support finder used by the character.
        /// The support finder analyzes the character's contacts to see if any of them provide support and/or traction.
        /// </summary>
        public SupportFinder SupportFinder { get; private set; }

    #endregion

        private long defense;
        private long health;
        private long id;
        private float jumpSpeed;
        private long meleePower;
        private long rangePower;
        private float speed;
        private long stamina;
        protected float maxjump;
        protected float lastyposition;

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

        void RemoveFriction(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair)
        {
            var collidablePair = pair as CollidablePairHandler;
            if (collidablePair != null)
            {
                //The default values for InteractionProperties is all zeroes- zero friction, zero bounciness.
                //That's exactly how we want the character to behave when hitting objects.
                collidablePair.UpdateMaterialProperties(new InteractionProperties());
            }
        }

        #endregion

        #region Movers

        /*protected void Move()
        {
            Body.LinearVelocity = currentVelocity;
        }*/

        protected void MoveForward(float dt)
        {
            float currentVelocity =(dt * speed);
            float currentMoveSpeed = Vector3.Dot(World.Forward, Body.LinearVelocity);
            Body.LinearVelocity += World.Forward * (currentVelocity - currentMoveSpeed);
            this.charecterState = CharacterState.Walk;
        }

        protected void MoveRight(float dt)
        {
            float currentVelocity = (dt * speed);
            float currentMoveSpeed = Vector3.Dot(World.Right, Body.LinearVelocity);
            Body.LinearVelocity += World.Right * (currentVelocity - currentMoveSpeed);
            this.charecterState = CharacterState.Walk;
        }

        protected void jump()
        {
            Vector3 impulse = new Vector3(0, jumpSpeed, 0);
            Body.ApplyLinearImpulse(ref impulse);
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
            this.maxjump = 20;
            this.lastyposition = 0;
            this.charecterState = CharacterState.Idle;
        }


        public new void Load(ContentManager content, String path)
        {
            base.Load(content, path);
            float h, r;
            CalculateHeightRadius(out h, out r);
            body = new Cylinder(Position, height * h, radius * r, mass);
            body.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
            body.Tag = Model;
            body.LocalInertiaTensorInverse = new Matrix3x3();
            body.LinearDamping =0;
            body.IgnoreShapeChanges = true;
            body.CollisionInformation.Events.DetectingInitialCollision += RemoveFriction;
        }
        #endregion
        
         public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Position = body.Position;
            if ((Position.Y - lastyposition) >= maxjump)
            {
               // Vector3 fall = new Vector3(0, -jumpSpeed, 0);
                //body.ApplyLinearImpulse(ref fall);
                float currentVelocity = (-25);
                float currentMoveSpeed = Vector3.Dot(World.Up, Body.LinearVelocity);
                Body.LinearVelocity += World.Up * (currentVelocity - currentMoveSpeed);
            }
            Body.LinearVelocity = new Vector3(0,Body.LinearVelocity.Y,0);
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
            airborne = check_support();
            if (!airborne)
            {
                lastyposition = Position.Y;
            }

        }
        #endregion
    }
}