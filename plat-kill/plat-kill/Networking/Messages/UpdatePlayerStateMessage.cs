﻿using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using plat_kill.GameModels.Players;
using plat_kill.Helpers;
using System;

namespace plat_kill.Networking.Messages
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UpdatePlayerStateMessage : IGameMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePlayerStateMessage"/> class.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public UpdatePlayerStateMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePlayerStateMessage"/> class.
        /// </summary>
        public UpdatePlayerStateMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePlayerStateMessage"/> class.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        public UpdatePlayerStateMessage(Player player)
        {
            this.Id = player.Id;
            this.Position = player.CharacterController.Body.Position;
            this.Velocity = player.CharacterController.Body.LinearVelocity;
            this.Rotation = player.Rotation;
            this.MessageTime = NetTime.Now;
            this.CharState = player.CharecterState;

            if (player.EquippedWeapons.Count != 0)
            {
               this.CurrentWeaponID = player.EquippedWeapons[player.ActiveWeaponIndex].WeaponID;
            }

            this.Health = player.Health;
        }

        #endregion

        #region Public Properties

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
                return GameMessageTypes.UpdatePlayerState;
            }
        }

        /// <summary>
        /// Gets or sets Position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets Rotation.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets Velocity.
        /// </summary>
        public Vector3 Velocity { get; set; }


        /// <summary>
        /// Gets or sets CharacterState.
        /// </summary>
        public CharacterState CharState { get; set; }


        /// <summary>
        /// Gets or sets CurrentWeaponID
        /// </summary>
        public long CurrentWeaponID { get; set; }

        /// <summary>
        /// Gets or sets CurrentWeaponID
        /// </summary>
        public long Health { get; set; }

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
            this.Rotation = im.ReadVector3(); 
            this.CharState = (CharacterState)Enum.Parse(typeof(CharacterState), im.ReadString());
            this.CurrentWeaponID = im.ReadInt64();
            this.Health = im.ReadInt64();
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
            om.Write(this.Rotation);
            om.Write(this.CharState.ToString());
            om.Write(this.CurrentWeaponID);
            om.Write(this.Health);
            
        }

        #endregion
    }
}