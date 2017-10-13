using Photon.SocketServer;
using Warsmiths.Common;

namespace Warsmiths.Server.Framework.Handlers
{
    public abstract class BaseHandler
    {
        public abstract OperationCode ControlCode { get; }

        public abstract OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase);
    }
}