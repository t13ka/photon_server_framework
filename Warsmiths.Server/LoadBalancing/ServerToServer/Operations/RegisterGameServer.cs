using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.ServerToServer.Operations
{
    /// <summary>
    ///   Defines the parameters which should be send from game server instances to 
    ///   register at the master application.
    /// </summary>
    public class RegisterGameServer : Operation
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RegisterGameServer" /> class.
        /// </summary>
        /// <param name = "rpcProtocol">
        ///   The rpc Protocol.
        /// </param>
        /// <param name = "operationRequest">
        ///   The operation request.
        /// </param>
        public RegisterGameServer(IRpcProtocol rpcProtocol, OperationRequest operationRequest)
            : base(rpcProtocol, operationRequest)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RegisterGameServer" /> class.
        /// </summary>
        public RegisterGameServer()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the public game server ip address.
        /// </summary>
        [DataMember(Code = 4, IsOptional = false)]
        public string GameServerAddress ;

        //[DataMember(Code = 5, IsOptional = true)]
        //public byte LocalNode ;

        /// <summary>
        ///   Gets or sets a unique server id.
        ///   This id is used to sync reconnects.
        /// </summary>
        [DataMember(Code = 3, IsOptional = false)]
        public string ServerId ;

        /// <summary>
        ///   Gets or sets the TCP port of the game server instance.
        /// </summary>
        /// <value>The TCP port.</value>
        [DataMember(Code = 2, IsOptional = true)]
        public int? TcpPort ;

        /// <summary>
        ///   Gets or sets the UDP port of the game server instance.
        /// </summary>
        /// <value>The UDP port.</value>
        [DataMember(Code = 1, IsOptional = true)]
        public int? UdpPort ;

        /// <summary>
        ///   Gets or sets the port of the game server instance used for WebSocket connections.
        /// </summary>
        [DataMember(Code = 6, IsOptional = true)]
        public int? WebSocketPort ;

        /// <summary>
        ///   Gets or sets the inital server state of the game server instance.
        /// </summary>
        [DataMember(Code = 7, IsOptional = true)]
        public int ServerState ;
        #endregion
    }
}