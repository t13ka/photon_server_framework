﻿using Photon.SocketServer.Rpc;

namespace YourGame.Server.Operations.Response
{
    public class RedirectRepeatResponse
    {
        #region Properties

        [DataMember(Code = (byte)ParameterCode.Address)]
        public string Address ;

        [DataMember(Code = (byte)ParameterCode.NodeId)]
        public byte NodeId ;

        #endregion
    }
}