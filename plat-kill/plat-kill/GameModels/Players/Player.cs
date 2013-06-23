using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace plat_kill.GameModels.Players
{
    internal class Player : GameModel
    {
        #region Properties

        private Cylinder body;
        private bool airborne;

        private Vector3 currentVelocity;
        private long defense;
        private long health;
        private long id;
        private float jumpSpeed;
        private long meleePower;


        private Vector3 playerHeadOffset;
        private long rangePower;
        private float speed;
        private long stamina;

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
        #endregion

        #region Methods
        public void Update(GameTime gameTime)
        {

        }
        #endregion
    }
}