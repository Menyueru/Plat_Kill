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
using Xclna.Xna.Animation;
using System.Collections.Generic;
using plat_kill.GameModels.Weapons;

namespace plat_kill.GameModels.Players
{
     public class Player : AnimatedModel
    {
        #region Properties

        public CharacterController CharacterController;
        private float radius;
        private bool isLocal;

        private long defense;
        private long health;
        private long id;
        private float jumpSpeed;
        private long meleePower;
        private long rangePower;
        private float speed;
        private long stamina;

        private int activeWeaponIndex;
        private List<Weapon> equippedWeapons;

        private Vector3 playerHeadOffset;
        
        #endregion

        #region Getter-Setters

        public int ActiveWeaponIndex
        {
            get { return activeWeaponIndex; }
            set { activeWeaponIndex = value; }
        }
        public List<Weapon> EquippedWeapons
        {
            get { return equippedWeapons; }
            set { equippedWeapons = value; }
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
            this.CharecterState = CharacterState.StandardWalk;

            this.equippedWeapons = new List<Weapon>();

        }


        public void Load(ContentManager content, String path, Space ownspace, GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            base.Load(content, path, graphicsDevice, view, projection);
            float h, r;
            CalculateHeightRadius(out h, out r);
            CharacterController = new CharacterController(Position, height * h, height * h*.7f, radius * r, mass);
            CharacterController.JumpSpeed = jumpSpeed;
            CharacterController.SlidingJumpSpeed= jumpSpeed * .6f;
            CharacterController.HorizontalMotionConstraint.Speed = speed;
            CharacterController.HorizontalMotionConstraint.AirSpeed = speed;
            CharacterController.HorizontalMotionConstraint.MaximumForce *= mass;
            CharacterController.VerticalMotionConstraint.MaximumGlueForce /= mass*mass;
            ownspace.Add(CharacterController);

            TPose = new AnimationController(ModelAnimator.Animations["t_pose"]);
            RifleWalk = new AnimationController(ModelAnimator.Animations["rifle_walk"]);
            RifleRun = new AnimationController(ModelAnimator.Animations["rifle_run"]);
            ShootRifle = new AnimationController(ModelAnimator.Animations["shoot_rifle"]);
            FiringRifle = new AnimationController(ModelAnimator.Animations["firing_rifle"]);
            RifleJumpInPlace = new AnimationController(ModelAnimator.Animations["rifle_jump_in_place"]);
            GreatSwordSlash = new AnimationController(ModelAnimator.Animations["great_sword_slash"]);
            RifleIdle = new AnimationController(ModelAnimator.Animations["rifle_idle"]);
            Reloading = new AnimationController(ModelAnimator.Animations["reloading"]);
            Reload = new AnimationController(ModelAnimator.Animations["reload"]);
            TossGrenade = new AnimationController(ModelAnimator.Animations["toss_grenade"]);
            Dodging = new AnimationController(ModelAnimator.Animations["dodging"]);
            StandardWalk = new AnimationController(ModelAnimator.Animations["standard_walk"]);
            Running = new AnimationController(ModelAnimator.Animations["running"]);
            SprintingFowardRoll = new AnimationController(ModelAnimator.Animations["sprinting_forward_roll"]);
        }
        #endregion

        public void addWeapon(Weapon weapon)
        {
            this.EquippedWeapons.Add(weapon);
            this.activeWeaponIndex = equippedWeapons.Count - 1;
        }

        public void changeToNextWeapon() 
        {
            if (activeWeaponIndex < equippedWeapons.Count)
            {
                activeWeaponIndex += 1;
            }
            else 
            {
                activeWeaponIndex = 0;
            }
        }

        public new void Update(GameTime gameTime)
        {
                switch (CharecterState)
                {
                    case CharacterState.StandardWalk:
                        runAnimationController(ModelAnimator, StandardWalk);
                        break;
                    case CharacterState.Running:
                        runAnimationController(ModelAnimator, RifleWalk);
                        break;
                    case CharacterState.RifleIdle:
                        runAnimationController(ModelAnimator, RifleIdle);
                        break;
                    case CharacterState.RifleRun:
                        runAnimationController(ModelAnimator, RifleRun);
                        break;
                    case CharacterState.RifleWalk:
                        runAnimationController(ModelAnimator, RifleWalk);
                        break;
                    case CharacterState.RifleJumpInPlace:
                        runAnimationController(ModelAnimator, RifleJumpInPlace);
                        break;
                    case CharacterState.FiringRifle:
                        runAnimationController(ModelAnimator, FiringRifle);
                        break;
                    case CharacterState.ShootRifle:
                        runAnimationController(ModelAnimator, ShootRifle);
                        break;
                    case CharacterState.TossGrenad:
                        runAnimationController(ModelAnimator, TossGrenade);
                        break;
                    case CharacterState.SprintingFowardRol:
                        runAnimationController(ModelAnimator, SprintingFowardRoll);
                        break;
                    case CharacterState.Dodging:
                        runAnimationController(ModelAnimator, Dodging);
                        break;
                    case CharacterState.GreatSwordSlash:
                        runAnimationController(ModelAnimator, GreatSwordSlash);
                        break;
                    case CharacterState.Reload:
                        runAnimationController(ModelAnimator, Reload);
                        break;
                    case CharacterState.Reloading:
                        runAnimationController(ModelAnimator, Reloading);
                        break;
                    case CharacterState.TPose:
                        runAnimationController(ModelAnimator, TPose);
                        break;

                    default:
                        runAnimationController(ModelAnimator, StandardWalk);
                        break;
                }
            UpdateState();
            Position = CharacterController.Body.Position;
            CharacterController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;

            base.Update(gameTime);             
        }

        public void UpdateState()
         {
             MovementCurrentDirection = CharacterController.Body.LinearVelocity;

             if (CharacterController.HorizontalMotionConstraint.MovementDirection.LengthSquared() > 0)
             {

                 changeCharacterState(CharacterState.StandardWalk);
             }
             else 
             {
                 changeCharacterState(CharacterState.RifleIdle);
             }
         }

        #endregion
    }
}