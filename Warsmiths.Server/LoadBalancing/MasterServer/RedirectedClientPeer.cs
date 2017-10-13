using System.Net;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using Warsmiths.Common;
using Warsmiths.Server.Common;
using Warsmiths.Server.Operations.Response;

namespace Warsmiths.Server.MasterServer
{
    public class RedirectedClientPeer : PeerBase
    {
        #region Constructors and Destructors

        public RedirectedClientPeer(IRpcProtocol protocol, IPhotonPeer unmanagedPeer)
            : base(protocol, unmanagedPeer)
        {
        }

        #endregion

        #region Methods

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var contract = new RedirectRepeatResponse();

            const byte masterNodeId = 1;

            // TODO: don't lookup for every operation!
            var publicIpAddress =
                PublicIPAddressReader.ParsePublicIpAddress(MasterServerSettings.Default.PublicIPAddress);
            switch (NetworkProtocol)
            {
                case NetworkProtocolType.Tcp:
                    contract.Address =
                        new IPEndPoint(
                            publicIpAddress, MasterServerSettings.Default.MasterRelayPortTcp + masterNodeId - 1)
                            .ToString
                            ();
                    break;
                case NetworkProtocolType.WebSocket:
                    contract.Address =
                        new IPEndPoint(
                            publicIpAddress, MasterServerSettings.Default.MasterRelayPortWebSocket + masterNodeId - 1).
                            ToString();
                    break;
                case NetworkProtocolType.Udp:
                    // no redirect through relay ports for UDP... how to handle? 
                    contract.Address =
                        new IPEndPoint(
                            publicIpAddress, MasterServerSettings.Default.MasterRelayPortUdp + masterNodeId - 1)
                            .ToString
                            ();
                    break;
            }


            var response = new OperationResponse(operationRequest.OperationCode, contract)
            {
                ReturnCode = (short) ErrorCode.RedirectRepeat,
                DebugMessage = "redirect"
            };

            SendOperationResponse(response, sendParameters);
        }

        #endregion
    }
}