using Photon.SocketServer.Rpc;

namespace Warsmiths.Server.Operations.Response
{
    /// <summary>
    /// Defines the response paramters for create game requests.
    /// </summary>
    public class CreateGameResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the address of the game server.
        /// </summary>
        /// <value>The game server address.</value>
        [DataMember(Code = (byte)ParameterCode.Address)]
        public string Address ;

        /// <summary>
        /// Gets or sets the game id.
        /// </summary>
        /// <value>The game id.</value>
        [DataMember(Code = (byte)ParameterCode.GameId)]
        public string GameId ;

        #endregion
    }
}