namespace Warsmiths.Server.Operations.Request.GameManagement
{
    using System;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    using YourGame.Server.Framework.Operations;

    public class JoinGameRequest : JoinRequest
    {
        //[DataMember(Code = (byte)ParameterCode.CreateIfNotExists, IsOptional = true)]
        //public bool CreateIfNotExists ;
        private object _internalJoinMode;

        public JoinGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public JoinGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyName ;

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType ;

        [DataMember(Code = (byte)ParameterCode.JoinMode, IsOptional = true)]
        internal object InternalJoinMode
        {
            get { return _internalJoinMode; }
            set
            {
                _internalJoinMode = value;
                var type = value.GetType();
                if (type == typeof(bool))
                {
                    if (JoinMode == JoinMode.Default && (bool)value)
                    {
                        JoinMode = JoinMode.CreateIfNotExists;
                    }
                    return;
                }

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        JoinMode = (JoinMode)(Convert.ToInt32(value));
                        return;
                }
            }
        }

        // no ParameterKey:
        // for backward compatibility this is set through InternalJoinMode
        public JoinMode JoinMode ;
        // no ParameterKey:
        // for backward compatibility this is set through InternalJoinMode
        public bool CreateIfNotExists
        {
            get { return JoinMode > 0; }

            set
            {
                _internalJoinMode = value;
                if (JoinMode == JoinMode.Default && value)
                {
                    JoinMode = JoinMode.CreateIfNotExists;
                }
            }
        }
    }
}