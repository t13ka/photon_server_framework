using System.Threading;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using Warsmiths.Server.GameServer;

namespace Warsmiths.Server.LoadShedding
{
    /// <summary>
    ///     Peer implementation to handle requests from the
    ///     <see cref="LatencyMonitor" /> .
    /// </summary>
    public class LatencyPeer : PeerBase
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        public LatencyPeer(IRpcProtocol protocol, IPhotonPeer nativePeer)
            : base(protocol, nativePeer)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Latency monitoring client connected, serverId={0}", GameApplication.ServerId);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Latency monitoring client disconnected: reason={0}, detail{1}", reasonCode,
                    reasonDetail);
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                default:
                {
                    var message = string.Format("Unknown operation code {0}", operationRequest.OperationCode);
                    SendOperationResponse(
                        new OperationResponse
                        {
                            OperationCode = operationRequest.OperationCode,
                            ReturnCode = -1,
                            DebugMessage = message
                        }, sendParameters);
                    break;
                }

                case 1:
                {
                    var pingOperation = new LatencyOperation(Protocol, operationRequest.Parameters);
                    if (pingOperation.IsValid == false)
                    {
                        SendOperationResponse(
                            new OperationResponse
                            {
                                OperationCode = operationRequest.OperationCode,
                                ReturnCode = -1,
                                DebugMessage = pingOperation.GetErrorMessage()
                            }, sendParameters);
                        return;
                    }

                    Thread.Sleep(5);

                    var response = new OperationResponse(operationRequest.OperationCode, pingOperation);
                    SendOperationResponse(response, sendParameters);
                    break;
                }
            }
        }

        #endregion
    }
}