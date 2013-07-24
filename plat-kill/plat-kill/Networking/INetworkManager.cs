﻿using System;
using Lidgren.Network;
using plat_kill.Networking.Messages;

namespace plat_kill.Networking
{
    /// <summary>
    /// The i network manager.
    /// </summary>
    public interface INetworkManager : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        void Connect();

        /// <summary>
        /// The create message.
        /// </summary>
        /// <returns>
        /// </returns>
        NetOutgoingMessage CreateMessage();

        /// <summary>
        /// The disconnect.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// The read message.
        /// </summary>
        /// <returns>
        /// </returns>
        NetIncomingMessage ReadMessage();

        /// <summary>
        /// The recycle.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        void Recycle(NetIncomingMessage im);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="gameMessage">
        /// The game message.
        /// </param>
        void SendMessage(IGameMessage gameMessage);

        #endregion
    }
}