using System;
using Lidgren.Network;
using plat_kill.Networking.Messages;
using System.Threading;

namespace plat_kill.Networking
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ServerNetworkManager : INetworkManager
    {
        #region Constants and Fields

        /// <summary>
        /// The is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The net server.
        /// </summary>
        private NetServer netServer;

        public NetServer NetServer
        {
            get { return netServer; }
            set { netServer = value; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        public void Connect()
        {
           
            var config = new NetPeerConfiguration("plat-kill")
                {
                    Port = Convert.ToInt32("14242"), 
                    //SimulatedMinimumLatency = 0.2f, 
                    // SimulatedLoss = 0.1f 
                };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            this.NetServer = new NetServer(config);
            this.NetServer.Start();

            //netServer.RegisterReceivedCallback(method);
        }

        /// <summary>
        /// The create message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetOutgoingMessage CreateMessage()
        {
            return this.NetServer.CreateMessage();
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
            this.NetServer.Shutdown("Bye");
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// The read message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetIncomingMessage ReadMessage()
        {
            return this.NetServer.ReadMessage();
        }

        /// <summary>
        /// The recycle.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public void Recycle(NetIncomingMessage im)
        {
            this.NetServer.Recycle(im);
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="gameMessage">
        /// The game message.
        /// </param>
        public void SendMessage(IGameMessage gameMessage)
        {
            NetOutgoingMessage om = this.NetServer.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(om);

            this.NetServer.SendToAll(om, NetDeliveryMethod.ReliableUnordered);
        }

        public void SendMessageToSingleClient(IGameMessage gameMessage, NetConnection recipient)
        {
            NetOutgoingMessage om = this.NetServer.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(om);

            this.NetServer.SendMessage(om, recipient, NetDeliveryMethod.ReliableOrdered);
            //this.NetServer.SendToAll(om, NetDeliveryMethod.ReliableUnordered);
        }
        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Disconnect();
                }

                this.isDisposed = true;
            }
        }

        #endregion
    }
}