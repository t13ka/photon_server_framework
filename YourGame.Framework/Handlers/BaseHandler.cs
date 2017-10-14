namespace YourGame.Server.Framework.Handlers
{
    using Photon.SocketServer;

    using YourGame.Common;

    public abstract class BaseHandler
    {
        public abstract OperationCode ControlCode { get; }

        public abstract OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase);
    }
}