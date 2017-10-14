using System.Collections.Generic;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace YourGame.Server.LoadShedding
{
    public class LatencyOperation : DataContract
    {
        #region Constructors and Destructors

        public LatencyOperation(IRpcProtocol protocol, Dictionary<byte, object> @params)
            : base(protocol, @params)
        {
        }

        public LatencyOperation()
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = 10)]
        public long? SentTime ;

        #endregion
    }
}