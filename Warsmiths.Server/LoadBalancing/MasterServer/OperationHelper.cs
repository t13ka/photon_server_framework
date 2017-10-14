using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

using Warsmiths.Server.Operations;

namespace Warsmiths.Server.MasterServer
{
    using YourGame.Common;

    /// <summary>
    ///     Provides static methods to validate operation requests and create
    ///     operation responses for invalid operation request.
    /// </summary>
    public static class OperationHelper
    {
        public static OperationResponse HandleInvalidOperation(Operation operation, ILogger logger)
        {
            var errorMessage = operation.GetErrorMessage();

            if (logger != null && logger.IsDebugEnabled)
            {
                logger.DebugFormat("Invalid operation: OpCode={0}; {1}", operation.OperationRequest.OperationCode,
                    errorMessage);
            }

            return new OperationResponse
            {
                OperationCode = operation.OperationRequest.OperationCode,
                ReturnCode = (int)ErrorCode.OperationInvalid,
                DebugMessage = errorMessage
            };
        }

        public static OperationResponse HandleUnknownOperationCode(OperationRequest operationRequest, ILogger logger)
        {
            if (logger != null && logger.IsDebugEnabled)
            {
                logger.DebugFormat("Unknown operation code: OpCode={0}", operationRequest.OperationCode);
            }

            return new OperationResponse
            {
                OperationCode = operationRequest.OperationCode,
                ReturnCode = (int)ErrorCode.OperationInvalid,
                DebugMessage = "Invalid operation code"
            };
        }

        public static bool ValidateOperation(Operation operation, ILogger logger, out OperationResponse response)
        {
            if (operation.IsValid)
            {
                response = null;
                return true;
            }

            response = HandleInvalidOperation(operation, logger);
            return false;
        }

        public static OperationResponse ValidateLogin(OperationRequest operationRequest, PeerBase peerBase)
        {
            var peer = (MasterClientPeer) peerBase;
            if (string.IsNullOrEmpty(peer.UserId))
            {
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.InvalidAuthentication,
                    DebugMessage = "User not logged! This operation is forbidden for not logged users"
                };
            }

            return null;
        }
    }
}