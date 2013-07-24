using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using plat_kill.GameModels.Projectiles;

namespace plat_kill.Networking.Messages
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ShotFiredMessage : IGameMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShotFiredMessage"/> class.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public ShotFiredMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShotFiredMessage"/> class.
        /// </summary>
        /// <param name="shot">
        /// The shot.
        /// </param>
        public ShotFiredMessage(Projectile shot)
        {
            this.Id = shot.ProjectileID;
            this.Position = shot.Position;
            this.Velocity = shot.Body.LinearVelocity;
            //this.FiredByPlayer = shot.FiredByPlayer;
            this.MessageTime = NetTime.Now;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets FiredById.
        /// </summary>
        public long FiredById { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether FiredByPlayer.
        /// </summary>
        public bool FiredByPlayer { get; set; }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets MessageTime.
        /// </summary>
        public double MessageTime { get; set; }

        /// <summary>
        /// Gets MessageType.
        /// </summary>
        public GameMessageTypes MessageType
        {
            get
            {
                return GameMessageTypes.ShotFired;
            }
        }

        /// <summary>
        /// Gets or sets Position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets Velocity.
        /// </summary>
        public Vector3 Velocity { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public void Decode(NetIncomingMessage im)
        {
            this.Id = im.ReadInt64();
            this.MessageTime = im.ReadDouble();
            this.Position = im.ReadVector3();
            this.Velocity = im.ReadVector3();
            this.FiredByPlayer = im.ReadBoolean();
            this.FiredById = im.ReadInt64();
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="om">
        /// The om.
        /// </param>
        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Id);
            om.Write(this.MessageTime);
            om.Write(this.Position);
            om.Write(this.Velocity);
            om.Write(this.FiredByPlayer);
            om.Write(this.FiredById);
        }

        #endregion
    }
}