using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Response
{
    public class AuthenticateResponse
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the queue position.
        ///     0 = The client passed the waiting queue
        ///     > 0 = The server is currently full and the client has been enqueued in the waiting queue.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.Position, IsOptional = false)]
        public int QueuePosition ;

        #endregion
    }
}